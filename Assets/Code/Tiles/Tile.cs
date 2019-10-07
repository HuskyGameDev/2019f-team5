//
// When We Fell
// 

using System;

public struct Tile : IEquatable<Tile>
{
	public TileType type;

	public Tile(TileType type)
		=> this.type = type;

	public static implicit operator Tile(TileType type)
		=> new Tile(type);

	public TileData data
		=> TileManager.GetData(type);

	public bool Equals(Tile other)
		=> type == other.type;

	public override bool Equals(object obj)
		=> Equals((Tile)obj);

	public override int GetHashCode()
		=> type.GetHashCode();

	public static bool operator ==(Tile a, Tile b)
		=> a.type == b.type;

	public static bool operator !=(Tile a, Tile b)
		=> a.type != b.type;
}
