// 
// Adventure Maker
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public enum ChunkState
{
	Empty,
	Filled
}

public sealed class SpriteRect
{
	public int startX, startY;
	public int width, height;
	public Tile tile;
}

public sealed class Chunk
{
	public const int Size = 16;
	public const int Shift = 4;
	public const int Mask = Size - 1;
	public const int Size2 = Size * Size;

	public Vector2Int cPos { get; private set; }
	public Vector2Int wPos { get; private set; }

	private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

	private Tile[] tiles = new Tile[Size2];
	private BitArray mask = new BitArray(Size2);

	// Sprite rectangle used for the greedy sprite algorithm.
	private SpriteRect spriteRect = new SpriteRect();

	private bool pendingUpdate;
	public bool pendingClear;

	private ChunkState state;

	public Chunk(int cX, int cY)
	{
		cPos = new Vector2Int(cX, cY);
		wPos = Utils.ChunkToWorldP(cX, cY);
	}

	public static int TileIndex(int x, int y)
		=> y * Size + x;

	public void SetTile(int rX, int rY, Tile tile)
		=> tiles[TileIndex(rX, rY)] = tile;

	public Tile GetTile(int rX, int rY)
		=> tiles[TileIndex(rX, rY)];

	public void SetTile(int index, Tile tile)
		=> tiles[index] = tile;

	public Tile GetTile(int index)
		=> tiles[index];

	public void SetModified()
		=> pendingUpdate = true;

	private void SetSprite(WorldRender rend, SpriteRect rect)
	{
		Tile tile = rect.tile;

		SpriteRenderer spriteRend = rend.GetSpriteRenderer();
		spriteRend.sprite = tile.data.sprite;
		spriteRend.sortingOrder = tile.data.sortingOrder;
		spriteRend.color = new Color(1.0f, 1.0f, 1.0f, tile.data.alpha);
		spriteRend.drawMode = rect.width > 1 || rect.height > 1 ? SpriteDrawMode.Tiled : SpriteDrawMode.Simple;
		spriteRend.size = new Vector2(rect.width, rect.height);

		Transform t = spriteRend.transform;
		t.position = new Vector3(wPos.x + rect.startX, wPos.y + rect.startY);
		t.localScale = Vector3.one;

		sprites.Add(spriteRend);
	}

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

	private void FillSprites(WorldRender rend)
	{
		if (sprites.Count > 0)
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

	public void ClearSprites(WorldRender rend)
	{
		for (int i = 0; i < sprites.Count; ++i)
			rend.ReturnSpriteRenderer(sprites[i]);

		sprites.Clear();

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
