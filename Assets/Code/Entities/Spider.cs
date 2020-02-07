using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spider : Entity
{
    public float jumpVelocity;
	public float gravity;
	public bool aggro;
	[SerializeField]
	public GameObject player;
	public bool collide;
	public bool facing;
    float rotation;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		rotation = 0.0f;

		if(Math.Abs(PlayerX - transform.position.x) <= 6 && Math.Abs(PlayerY - transform.position.y) < 6)
		{
            aggro = true;
		}

		if(Math.Abs(PlayerX - transform.position.x) >= 12 || Math.Abs(PlayerY - transform.position.y) >= 12)
		{
            aggro = false;
		}

		Vector2 accel = Vector2.zero;

        if(PlayerX < transform.position.x && aggro)
		{
			if(Math.Abs(PlayerX - transform.position.x) >= .9)
			{
				accel = Vector2.left;
				facing = true;
			}

			SetFacingDirection(true);
		} else if (aggro)
		{
			if(Math.Abs(PlayerX - transform.position.x) >= .9)
			{
				accel = Vector2.right;
				facing = false;
			}

			SetFacingDirection(false);
		}
		
		if (CollidedLeft() || CollidedRight() && aggro)
		{
			velocity.y = jumpVelocity;
            gravity = 0;
			if( facing) {
				rotation = -90f;
			} else {
				rotation = 90f;
			}
            
		} 
		else if (CollidedBelow() && aggro) 
        {
			collide = false;
            gravity = -30;
            
        } 
		else if (CollidedAbove() && aggro) 
        {
            rotation = 180.0f;
            gravity = 0;
         
             
        } 
		else 
		{
            gravity = -30;
        }

		Vector3 pivot = Position;
		pivot.y += 0.5f;

		transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
		Move(world, accel, gravity);
    }

	protected override void OnCollide(CollideResult col)
	{
		if (CollidedSides() && CollidedBelow())
		{
			collide = true;
		}
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
					Damage(4);
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
