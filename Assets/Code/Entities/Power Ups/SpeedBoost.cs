using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Entity
{
    protected override void HandleOverlaps(List<CollideResult> overlaps)
	{
		for (int i = 0; i < overlaps.Count; ++i)
		{
			CollideResult result = overlaps[i];
			Entity target = result.entity;

			if (target != null && target is Player)
			{
				target.speed += 1;
                Destroy(gameObject);
			}
		}
	}
}

