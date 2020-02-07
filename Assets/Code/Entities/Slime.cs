﻿ //
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;

public class Slime : Entity
{
	public float jumpVelocity;
	public float gravity;
	public bool aggro;
	public float jumpTime = 1.5f;
	[SerializeField]
	public GameObject player;

	Vector2 accel;

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

		if (Math.Abs(PlayerX - transform.position.x) <= 8 && Math.Abs(PlayerY - transform.position.y) < 8)
		{
            FindObjectOfType<Audiomanager>().Play("Slime Cry");
            aggro = true;
		}

		if (Math.Abs(PlayerX - transform.position.x) >= 15 || Math.Abs(PlayerY - transform.position.y) >= 15)
		{
			aggro = false;
		}

		if (aggro && (colFlags & CollideFlags.Below) != 0)
		{
			if (isJumping)
			{
                FindObjectOfType<Audiomanager>().Play("Slime Cry");
                isJumping = false;
				accel = Vector2.zero;
			}
			if (jumpTime <= 0)
			{
                velocity.y = jumpVelocity;
				jumpTime = 1.5f;

			}
		}
		else if (aggro && !isJumping)
		{
			isJumping = true;
			if (PlayerX < transform.position.x && aggro && !CollidedBelow())
			{
				accel = Vector2.left;
				SetFacingDirection(false);
			}
			else if (PlayerX > transform.position.x && aggro && !CollidedBelow())
			{
				accel = Vector2.right;
				SetFacingDirection(true);
			}
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

				if (diff.y > 0.4f)
				{
                    Damage(5);
					target.ApplyKnockback(0.0f, 7.5f);
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
