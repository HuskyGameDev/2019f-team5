// 
// When We Fell
//

using UnityEngine;

public struct AABB
{
	public Vector2 center;
	public Vector2 radius;

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
	
	public void Draw(Color color, float time)
		=> DebugHelper.ShowOutline(center, radius * 2.0f, color, time);
}