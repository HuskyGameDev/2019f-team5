using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skeleton : Entity
{
    public GameObject Arrow;
    float fireRate;
    float nextFire;
    public float jumpVelocity;
	public float gravity;
	public bool aggro;
	[SerializeField]
	public GameObject player;
	public bool collide;
	public bool Facing = false;
	public bool Eyes = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        fireRate = 3f;
        nextFire = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(FireOrNot());

        float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		if(Math.Abs(PlayerX - transform.position.x) <= 8 && Math.Abs(PlayerY - transform.position.y) < 8)
		{
			aggro = true;
		}

		if(Math.Abs(PlayerX - transform.position.x) >= 16 || Math.Abs(PlayerY - transform.position.y) >= 16)
		{
			aggro = false;
		}

		Vector2 accel = Vector2.zero;

		if(PlayerX < transform.position.x && aggro)
		{
			if(!Eyes) {
				if((Math.Abs(PlayerX - transform.position.x) >= 5))
				{
					accel = Vector2.left;
					Facing = true;
				}
			}
			SetFacingDirection(true);

		} else if (aggro)
		{
			if(!Eyes) {
				if((Math.Abs(PlayerX - transform.position.x) >= 5))
				{
					accel = Vector2.right;
					Facing = false;
				}
			}
			SetFacingDirection(false);
		}
		
		if(collide && aggro)
		{
			velocity.y = jumpVelocity;
			collide = false;
		}


		Move(world, accel, -30);
    }

    IEnumerator FireOrNot()
	{
		bool InView = false;
		Vector2 Skele = Position;
		Vector2 Play;
		Skele.y+=.5f;
		AABB rad = AABB.FromCenter(Position, new Vector2 (9, 9));
		List<Entity> list = world.GetOverlappingEntities(rad);
		for( int i = 0; i < list.Count; i++) {
			if(list[i] is Player) {
				Play = list[i].Position;
				Play.y+=.5f;
				Ray ray = new Ray(Skele, (Skele - Play).normalized);
				InView = world.TileRaycast(ray, 10, out Vector2 result);
				Eyes = false;
			}
		}
		
        if(Time.time > nextFire && aggro && InView)
		{
			Eyes = true;
			PlayAnimation("SkeletonAttack");
			yield return new WaitForSeconds(.5f);

			if(Time.time > nextFire) 
			{
				Vector2 arrowS ;
				arrowS = transform.position;
				arrowS.y += .25f;
				if(Facing) {
					arrowS.x -= .5f;
				} else {
					arrowS.x += .5f;
				}
				
				Instantiate(Arrow, arrowS, Quaternion.identity);
			}

            nextFire = Time.time + fireRate;
        }
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
