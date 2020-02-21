// 
// Adventure Maker
//

using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Collections;

public enum ChunkState
{
	Empty,
	Filled
}

// Each SpriteRect maps to one Unity SpriteRenderer.
// This is computed by a greedy algorithm to try to 
// create larger rectangles of the same tile type,
// so we can use a single tiled SpriteRenderer 
// to display all the tiles in the rect.
public sealed class SpriteRect
{
	public int startX, startY;
	public int width, height;
	public Tile tile;
}

// The world is composed of chunks. These allow a spatial
// partitioning of the world. Chunks store tiles and entities
// from the world.
public sealed class Chunk
{
	// Size of the chunk in tiles.
	public const int Size = 16;
	public const int Size2 = Size * Size;

	public const int ObstacleBlockWidth = 8;
	public const int ObstacleBlockHeight = 6;

	// Chunk and world position from the bottom-left corner.
	public Vector2Int cPos { get; private set; }
	public Vector2Int wPos { get; private set; }

	// Stores sprites this chunk is using so they can be returned to
	// the pool when this chunk is out of view.
	private List<SpriteRenderer> rects = new List<SpriteRenderer>();

	private Tile[] tiles = new Tile[Size2];

	// Lists the visible tiles for greedy sprite filling.
	private BitArray mask = new BitArray(Size2);

	// Sprite rectangle used for the greedy sprite algorithm.
	private SpriteRect spriteRect = new SpriteRect();

	private bool pendingUpdate;
	public bool pendingClear;

	public List<Entity> entities { get; private set; } = new List<Entity>();

	private ChunkState state;

	// Creates a new chunk. If fillBackground is true,
	// the chunk will be filled with background tiles. 
	// Otherwise, it is assumed that the chunk will be loaded
	// with data from a room file.
	public Chunk(int cX, int cY, bool fillBackground = false)
	{
		cPos = new Vector2Int(cX, cY);
		wPos = Utils.ChunkToWorldP(cX, cY);

		if (fillBackground)
		{
			for (int i = 0; i < tiles.Length; ++i)
				tiles[i] = TileType.CaveWall;
		}
	}

	public Chunk(int cX, int cY, string dataText) : this(cX, cY)
		=> DecodeData(dataText);

	// Tile data is stored in a 1D array. This collapses a
	// 2D coordinate into a 1D index into the array.
	public static int TileIndex(int x, int y)
		=> y * Size + x;

	public void SetTile(int rX, int rY, Tile tile)
	{
		if (rX >= 0 && rX < Size && rY >= 0 && rY < Size)
		{
			int index = TileIndex(rX, rY);
			tiles[index] = tile;
			TileManager.GetData(tiles[index]).OnSet(this, rX, rY);
		}
	}

	public Tile GetTile(int rX, int rY)
		=> tiles[TileIndex(rX, rY)];

	public void SetTile(int index, Tile tile)
		=> tiles[index] = tile;

	public Tile GetTile(int index)
		=> tiles[index];

	public void SetModified()
		=> pendingUpdate = true;
		
	// Chunk data can be built in the editor and saved into JSON room files.
	// This data can be loaded into a chunk by passing the JSON string here
	// to create the chunk. Tiles will match how they appear in the room editor.
	private void DecodeData(string data)
	{
		ChunkData chunkData = JsonUtility.FromJson<ChunkData>(data);

		int i = 0;
		int loc = 0;

		while (i < Size2)
		{
			int count = chunkData.tiles[loc++];
			TileType tile = (TileType)chunkData.tiles[loc++];

			for (int j = 0; j < count; ++j)
				tiles[i++] = tile;
		}

		for (int y = 0; y < Size; ++y)
		{
			for (int x = 0; x < Size; ++x)
				TileManager.GetData(tiles[TileIndex(x, y)]).OnSet(this, x, y);
		}
	}

	// Loads an obstacle block into this chunk using the dataText provided.
	// It is assumed this text is RLE-encoded tile data saved from the
	// room builder in the editor.
	public void SetObstacleBlock(int x, int y, string dataText)
	{
		ChunkData data = JsonUtility.FromJson<ChunkData>(dataText);

		TileType[] blockTiles = new TileType[ObstacleBlockWidth * ObstacleBlockHeight];

		int i = 0;
		int loc = 0;

		while (i < ObstacleBlockWidth * ObstacleBlockHeight)
		{
			int count = data.tiles[loc++];
			TileType tile = (TileType)data.tiles[loc++];

			for (int j = 0; j < count; ++j)
				blockTiles[i++] = tile;
		}

		int index = 0;

		for (int bY = y; bY < y + ObstacleBlockHeight; ++bY)
		{
			for (int bX = x; bX < x + ObstacleBlockWidth; ++bX)
			{
				if (bX >= 0 && bX < Size && bY >= 0 && bY < Size)
					SetTile(bX, bY, blockTiles[index++]);
			}
		}
	}

	public void SetEntity(Entity entity)
	{
		Assert.IsTrue(entity.chunk == null);
		entities.Add(entity);
		entity.chunk = this;
	}

	public void RemoveEntity(Entity entity)
	{
		entities.Remove(entity);
		entity.chunk = null;
	}

	// Fills an area of the chunk with a tile as determined by
	// the given SpriteRect object.
	private void SetSprite(WorldRender rend, SpriteRect rect)
	{
		Tile tile = rect.tile;

		SpriteRenderer tileRect = rend.GetTileRect();
		tileRect.sprite = tile.data.sprite;
		tileRect.sortingOrder = tile.data.sortingOrder;
		tileRect.color = new Color(1.0f, 1.0f, 1.0f, tile.data.alpha);
		tileRect.drawMode = rect.width > 1 || rect.height > 1 ? SpriteDrawMode.Tiled : SpriteDrawMode.Simple;
		tileRect.size = new Vector2(rect.width, rect.height);

		Transform t = tileRect.transform;
		t.position = new Vector3(wPos.x + rect.startX, wPos.y + rect.startY);
		t.localScale = Vector3.one;

		rects.Add(tileRect);
	}

	// Finds the next tile to begin a rectangle from for our greedy algorithm.
	private SpriteRect GetNextRectStart(int startX, int startY)
	{
		int y = startY, x = startX;

		while (y < Size)
		{
			while (x < Size)
			{
				Tile tile = tiles[TileIndex(x, y)];

				if (tile != TileType.Air)
				{
					spriteRect.startX = x;
					spriteRect.startY = y;
					spriteRect.width = 1;
					spriteRect.height = 1;
					spriteRect.tile = tile;

					return spriteRect;
				}

				++x;
			}

			x = 0;
			++y;
		}

		return null;
	}

	// Attempts to expand the current sprite rectangle
	// to the right. It can expand as far as the same tile
	// type repeats.
	private void RectScanX(SpriteRect rect)
	{
		for (int x = rect.startX + 1; x < Size; ++x)
		{
			int index = TileIndex(x, rect.startY);
			bool visible = mask.Get(index);

			if (visible && tiles[index] == rect.tile)
				++rect.width;

			else return;
		}
	}

	// Attempts to expand the current sprite rectangle
	// upward. It can only expand as long as the same
	// tile appears across the entire row at the higher
	// y position.
	private void RectScanY(SpriteRect rect)
	{
		for (int y = rect.startY + 1; y < Size; ++y)
		{
			for (int x = rect.startX; x < rect.startX + rect.width; ++x)
			{
				int index = TileIndex(x, y);
				bool visible = mask.Get(index);

				if (!visible || tiles[index] != rect.tile)
					return;
			}

			for (int x = rect.startX; x < rect.startX + rect.width; ++x)
				mask.Set(TileIndex(x, y), false);

			++rect.height;
		}
	}

	// Adds sprites that fit the tile data for this chunk,
	// making it able to be rendered.
	private void FillSprites(WorldRender rend)
	{
		if (rects.Count > 0)
			ClearSprites(rend);

		for (int i = 0; i < Size2; ++i)
		{
			Tile tile = tiles[i];

			if (tile != TileType.Air)
				mask.Set(i, tile.data.visible);
			else mask.Set(i, false);
		}

		int x = 0, y = 0;

		while (true)
		{
			SpriteRect rect = GetNextRectStart(x, y);

			if (rect == null)
				break;

			RectScanX(rect);
			RectScanY(rect);

			SetSprite(rend, rect);

			x = rect.startX + rect.width;
			y = rect.startY;

			if (x == Size)
			{
				x = 0;
				++y;
			}
		}

		state = ChunkState.Filled;
	}

	// Clears all SpriteRenderer objects from this
	// chunk and returns them to a pool.
	public void ClearSprites(WorldRender rend)
	{
		for (int i = 0; i < rects.Count; ++i)
			rend.ReturnTileRect(rects[i]);

		rects.Clear();

		state = ChunkState.Empty;
	}

	public void Update(WorldRender rend)
	{
		if (pendingUpdate || state == ChunkState.Empty)
		{
			FillSprites(rend);
			pendingUpdate = false;
		}
	}

	public void Clear()
		=> Array.Clear(tiles, 0, tiles.Length);
}
