﻿//
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Random = UnityEngine.Random;

public class Rat : Entity
{

	public float jumpVelocity;
	public float gravity;
	public bool aggro;
	public bool movingRight, movingLeft;
	public GameObject player;
	public float count = 0;
	public Vector2 stay;
	public Vector2Int tilePos;
	public bool facing;

    private int i = 0;

    private void Start()
		=> player = GameObject.Find("Player");

    private void Update()
	{
		float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		if (Math.Abs(PlayerX - Position.x) <= 10 && Math.Abs(PlayerY - Position.y) < 3.2f)
			aggro = true;

		if (Math.Abs(PlayerX - Position.x) >= 15)
			aggro = false;

		Vector2 accel = Vector2.zero;

		if (PlayerX < Position.x && aggro)
		{
            if (Math.Abs(PlayerX - Position.x) >= .9)
			{
                accel = Vector2.left;
				facing = true;
            }

			SetFacingDirection(true);
		}
		else if (aggro)
		{
            if (Math.Abs(PlayerX - Position.x) >= 0.9f)
			{
				accel = Vector2.right;
				facing = false;
			}

			SetFacingDirection(false);
		}

		tilePos = Utils.TilePos(Position);

		if (TileManager.GetData(world.GetTile(tilePos.x + 1, tilePos.y - 1)).passable && CollidedBelow() && aggro && !facing)
			velocity.y = jumpVelocity;

		else if(TileManager.GetData(world.GetTile(tilePos.x - 1, tilePos.y - 1)).passable && CollidedBelow() && aggro && facing)
			velocity.y = jumpVelocity;

		if ((PlayerY - Position.y) > 1.99f)
		{
			Move(-accel, gravity);
			StartCoroutine(wait());
			Move(accel, gravity);
		}
		else Move(accel, gravity);

        if (aggro && i==0)
        {
            i++;
            audioManager.Play("Rat Cry");
        }
    }

		protected override void OnKill() {
			player.GetComponent<Player>().enemiesKilled += 1;
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
					float direction = 15f;
					float PosoNeg = Random.Range(0,2)*2-1;
					direction = direction * PosoNeg;
					Damage(3);
					target.ApplyKnockback( direction, 8f);
				}
				else
				{
					Vector2 force = diff * knockbackForce;
					target.Damage(3, force);
				}
			}
		}
	}

	private IEnumerator wait()
	{
		yield return new WaitForSeconds(5.0f);
	}
}
