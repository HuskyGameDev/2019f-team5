using System;

public class OverTimeDamage : IEquatable<OverTimeDamage>
{
	public TileType type;
	public float interval;
	public int damage;
	public float timeLeft;
	public bool active;

	public OverTimeDamage(TileType type, float interval, int damage)
	{
		this.type = type;
		this.interval = interval;
		this.damage = damage;
	}

	public bool Equals(OverTimeDamage other)
		=> type == other.type;
}
