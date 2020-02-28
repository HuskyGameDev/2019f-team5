//
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BackgroundBat : Entity
{
	public float jumpVelocity;
	public float gravity;
	public bool aggro;

	private int i = 0;
	private Stack <Vector2> path = new Stack<Vector2>();
	private Vector2 NextPos;
	
	protected override void Awake()
	{
		base.Awake();
		EventManager.Instance.Subscribe(GameEvent.LevelGenerated, InvokePath);
	}

    private void Update()
	{
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
		RectInt bounds = world.GetBounds();

		int destX = Random.Range(bounds.xMin, bounds.xMax);
		int destY = Random.Range(bounds.yMin, bounds.yMax);

		Vector2Int target = new Vector2Int(destX, destY);
		world.FindPath(Utils.TilePos(transform.position), target, path);

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
