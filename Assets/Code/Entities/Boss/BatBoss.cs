//
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BatBoss : Entity
{
	public float jumpVelocity;
	public float gravity;
	public bool aggro;
	public GameObject player;

	private int i = 0;
	private Stack <Vector2> path = new Stack<Vector2>();
	private Vector2 NextPos;

	protected override void Awake()
	{
		base.Awake();

		player = GameObject.Find("Player");
		EventManager.Instance.Subscribe(GameEvent.LevelGenerated, InvokePath);
	}

    private void Update()
	{
		float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		aggro = true;

		Vector2 accel = Vector2.zero;

		if (aggro)
			accel = (NextPos - Position).normalized;

		if ((NextPos - Position).sqrMagnitude <= 1.0f && path.Count > 0)
			NextPos = path.Pop();

		if (aggro && i == 0)
		{
			i++;
			audioManager.Play("Bat Cry");
		}

		Move(accel, gravity);
    }

	private void InvokePath(object Obj)
		=> InvokeRepeating("FindPath", 0, 0.5f);

	private void FindPath()
	{
		world.FindPath(Utils.TilePos(Position), Utils.TilePos(player.transform.position), path);

		if (path.Count > 0)
		{
			NextPos = path.Pop();
		}
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
					float direction = 15f;
					float PosoNeg = Random.Range(0,2)*2-1;
					direction = direction * PosoNeg;
					target.Damage(3);
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
}
