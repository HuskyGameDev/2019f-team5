 //
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;

public class SlimeBoss : Entity
{
	public float jumpVelocity;
	public float gravity;
	public bool aggro;
	public float jumpTime = 1.5f;
	public GameObject player;

	private Vector2 accel;
	public bool isJumping;

	private void Start()
	{
		player = GameObject.Find("Player");
		accel = Vector2.zero;
	}

	private void Update()
	{
		jumpTime -= Time.deltaTime;

		float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		aggro = true;

		if (aggro && (colFlags & CollideFlags.Below) != 0)
		{
			if (isJumping)
			{
                isJumping = false;
				accel = Vector2.zero;
			}

			if (jumpTime <= 0)
			{
                audioManager.Play("Slime Cry");
                velocity.y = jumpVelocity;
				jumpTime = 1.5f;

			}
		}
		else if (aggro && !isJumping)
		{
			isJumping = true;

			if (PlayerX < Position.x && aggro && !CollidedBelow())
			{
				accel = Vector2.left;
				SetFacingDirection(false);
			}
			else if (PlayerX > Position.x && aggro && !CollidedBelow())
			{
				accel = Vector2.right;
				SetFacingDirection(true);
			}
		}

		Move(accel, gravity);
	}

protected override void OnKill()
	{
		player.GetComponent<Player>().LoadNextLevel();
		base.OnKill();
	}

	protected override void HandleOverlaps(List<CollideResult> overlaps)
	{
		for (int i = 0; i < overlaps.Count; ++i)
		{
			CollideResult result = overlaps[i];
			Entity target = result.entity;

			if (target != null && target is Player)
			{
				Vector2 diff = PositionDifference(target);

				if (diff.y > 0.4f)
				{
					Vector2 force = diff * knockbackForce;
					Damage(3, force);
					target.ApplyKnockback(0.0f, 12f);
				}
				else
				{
					Vector2 force = diff * knockbackForce;
					target.Damage(3, force);
				}
			}
		}
	}
}
