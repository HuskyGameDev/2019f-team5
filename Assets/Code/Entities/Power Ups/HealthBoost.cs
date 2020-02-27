//
// When we fell
//

using UnityEngine;
using System.Collections.Generic;

public class HealthBoost : Entity
{
	private static GameObject damagePopup;

	private void Update()
	{
		if (damagePopup == null)
			damagePopup = Resources.Load<GameObject>("Prefabs/DamagePopup");
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
				target.health += 2;
				target.GetComponent<Player>().maxHealth += 2;
				GameObject points = Instantiate(damagePopup, transform.position, Quaternion.identity);
				points.transform.GetComponent<TextMesh>().text = "Max Health Up";
				audioManager.Play("Magic");
				Destroy(gameObject);
			}
		}
	}
}
