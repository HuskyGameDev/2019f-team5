//
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;

public class Rat : Entity
{

	public float jumpVelocity;
	public float gravity;
	public bool aggro;
	public bool movingRight, movingLeft;
	[SerializeField]
	public GameObject player;
	
	private void Start()
	{
		player = GameObject.Find("Player");
	}

    private void Update()
	{
		float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		if(Math.Abs(PlayerX - transform.position.x) <= 10 && Math.Abs(PlayerY - transform.position.y) < 1.2)
		{
			aggro = true;
		}

		if(Math.Abs(PlayerX - transform.position.x) >= 15)
		{
			aggro = false;
		}

		Vector2 accel = Vector2.zero;

		if(PlayerX < transform.position.x && aggro)
		{
			if(Math.Abs(PlayerY - transform.position.y) > 1 && movingRight) {
				accel = Vector2.right;
			} else {
				accel = Vector2.left;
				movingRight = false;
			}
			
			movingLeft = true;
		} else if (aggro)
		{
			if(Math.Abs(PlayerY - transform.position.y) > 1 && movingLeft) {
				accel = Vector2.left;
			} else {
				accel = Vector2.right;
				movingLeft = false;
			}
			movingRight = true;
			
		}

		Move(world, accel, gravity);
	}

	protected override void HandleOverlaps(List<CollideResult> overlaps)
	{
		for (int i = 0; i < overlaps.Count; ++i)
		{
			CollideResult result = overlaps[i];
			Entity target = result.entity;

			if (target != null && target is Player)
			{
				Vector2 diff = (target.Position - Position).normalized;

				if (Mathf.Abs(diff.y) > 0.4f)
				{
					Damage(5);
					target.ApplyForce(0.0f, 7.5f);
				}
				else
				{
					Vector2 force = diff * 20.0f;
					force.y = Mathf.Max(force.y, 2.0f);
					target.Damage(3, force);
				}
			}
		}
	}
}
