//
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;

public class BackgroundBat : Entity
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

		if(Math.Abs(PlayerX - transform.position.x) <= 8 && Math.Abs(PlayerY - transform.position.y) < 50)
			aggro = true;

		Position = Vector3.MoveTowards(transform.position, NextPos, Time.deltaTime * speed);

		if (Position == NextPos && path.Count > 0)
			NextPos = path.Pop();

        if (aggro && i == 0)
        {
            i++;
            audioManager.Play("Bat Cry");
        }
    }

	private void InvokePath(object Obj) {
		InvokeRepeating("FindPath", 0, 0.5f);
	}

	private void FindPath() 
	{
		world.FindPath(Utils.TilePos(transform.position), Utils.TilePos(player.transform.position), path);

		if(path.Count > 0)
			NextPos = path.Pop();
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
