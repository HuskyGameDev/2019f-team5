
using System.Collections.Generic;
using UnityEngine;
/*
public class Item : Entity
{
	private void Update()
	{
		// Move so that it works with the collision system,
		// even though it doesn't actually move.
		Move(Vector2.zero, 0.0f);
	}
	protected override void HandleOverlaps(List<CollideResult> overlaps)
	{
		for (int i = 0; i < overlaps.Count; ++i)
		{
			CollideResult result = overlaps[i];
			Entity target = result.entity;

			if (target != null && target is Player)
			{
				target.GetComponent<PlayerAttack>().swingRate -= 0.05f;
				Destroy(gameObject);
			}
		}
	}

}
*/