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
	public GameObject player;
	public bool collide;
	public bool Facing = false;
	public bool Eyes = false;
    int i = 0;

    private void Start()
    {
        player = GameObject.Find("Player");
        fireRate = 3f;
        nextFire = Time.time;
    }

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine(FireOrNot());

        float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		if (Math.Abs(PlayerX - transform.position.x) <= 9 && Math.Abs(PlayerY - transform.position.y) < 9)
			aggro = true;

		if (Math.Abs(PlayerX - transform.position.x) >= 17 || Math.Abs(PlayerY - transform.position.y) >= 17)
			aggro = false;

		Vector2 accel = Vector2.zero;

		if(PlayerX < transform.position.x && aggro)
		{
			if(!Eyes)
			{
				if ((Math.Abs(PlayerX - transform.position.x) >= 3))
				{
					accel = Vector2.left;
					Facing = true;
				}
			}

			SetFacingDirection(true);

		}
		else if (aggro)
		{
			if (!Eyes)
			{
				if((Math.Abs(PlayerX - transform.position.x) >= 3))
				{
					accel = Vector2.right;
					Facing = false;
				}
			}

			SetFacingDirection(false);
		}

		if (collide && aggro)
		{
			velocity.y = jumpVelocity;
			collide = false;
		}

		Move(accel, -30);

        if (aggro && i ==0)
        {
            i++;
            audioManager.Play("Skeleton Cry");
        }
    }

    private IEnumerator FireOrNot()
	{
		bool InView = false;
		Vector2 Skele = Position;
		Vector2 Play;
		Skele.y+=.5f;
		AABB rad = AABB.FromCenter(Position, new Vector2 (16, 16));
		List<Entity> list = world.GetOverlappingEntities(rad);
		for( int i = 0; i < list.Count; i++) {
			if(list[i] is Player) {
				Play = list[i].Position;
				Play.y+=.5f;
				Vector2 dist = Play - Skele;
				Ray ray = new Ray(Skele, dist.normalized);
				InView = !world.TileRaycast(ray, dist.magnitude, out Vector2 result);
				Eyes = InView;
			}
		}

        if (Time.time > nextFire && aggro && InView)
		{
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
			collide = true;
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
          GameObject points = Instantiate(DamagePopup, transform.position, Quaternion.identity) as GameObject;
					String damage = target.damage.ToString();
					points.transform.GetChild(0).GetComponent<TextMesh>().text = damage;
					target.ApplyKnockback(0.0f, 7.5f);
				}
				else
				{
					Vector2 force = diff * knockbackForce;
					target.Damage(3, force);
          GameObject points = Instantiate(DamagePopup, transform.position, Quaternion.identity) as GameObject;
					points.transform.GetChild(0).GetComponent<TextMesh>().text = "3";
				}
			}
		}
	}
}
