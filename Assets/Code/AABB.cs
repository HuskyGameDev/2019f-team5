// 
// When We Fell
//

using UnityEngine;

// Axis-Aligned Bounding Box. Represents a rectangular area in space.
// Used primarily for collision detection. AABB is defined as a center
// and a radius. 'radius' really refers to half-extents - the distance
// from the center to the edge of the box on any given axis, not at an angle.
public struct AABB
{
	public Vector2 center;
	public Vector2 radius;

	public Vector2 BottomLeft => center - radius;
	public Vector2 TopRight => center + radius;

	public void Expand(Vector2 amount)
		=> radius += amount;

	public void Shrink(Vector2 amount)
		=> radius -= amount;

	public static AABB FromCorner(Vector2 corner, Vector2 size)
	{
		AABB bb = new AABB();
		bb.radius = size * 0.5f;
		bb.center = corner + bb.radius;
		return bb;
	}

	public static AABB FromMinMax(Vector2 min, Vector2 max)
		=> FromCorner(min, max - min);

	public static AABB FromCenter(Vector2 center, Vector2 radius)
	{
		AABB bb = new AABB();
		bb.center = center;
		bb.radius = radius;
		return bb;
	}

	public static AABB FromBottomCenter(Vector2 bc, Vector2 size)
	{
		AABB bb = new AABB();
		bb.radius = size * 0.5f;
		bb.center = new Vector2(bc.x, bc.y + bb.radius.y);
		return bb;
	}

	public static bool TestOverlap(AABB a, AABB b)
	{
		Vector2 minA = a.center - a.radius, maxA = a.center + a.radius;
		Vector2 minB = b.center - b.radius, maxB = b.center + b.radius;

		bool overlapX = minA.x <= maxB.x && maxA.x >= minB.x;
		bool overlapY = minA.y <= maxB.y && maxA.y >= minB.y;

		return overlapX && overlapY;
	}

	public override int GetHashCode()
		=> center.GetHashCode();

	public void Draw(Color color, float time)
		=> DebugServices.Instance.DrawOutline(this, color, time);

	public void Draw(Color color)
		=> DebugServices.Instance.DrawOutline(this, color);
}