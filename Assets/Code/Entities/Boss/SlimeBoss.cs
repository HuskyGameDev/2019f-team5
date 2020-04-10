 //
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
		isBoss = true;
	}

	private void Update()
	{
		jumpTime -= Time.deltaTime;

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
		EventManager.Instance.SignalEvent(GameEvent.BossKilled, null);
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
					float direction = 20f;
					float PosoNeg = Random.Range(0,2)*2-1;
					direction = direction * PosoNeg;
					Damage(3);
					target.ApplyKnockback( direction, 20f);
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
