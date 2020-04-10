using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SkeletonBoss : Entity
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
        fireRate = 1.0f;
        nextFire = Time.time;
		isBoss = true;
	}

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine(FireOrNot());

        float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		aggro = true;

		Vector2 accel = Vector2.zero;

		if(PlayerX < transform.position.x && aggro)
		{
			if(!Eyes)
			{
				accel = Vector2.left;
				Facing = true;
			}

			SetFacingDirection(true);

		}
		else if (aggro)
		{
			if (!Eyes)
			{
				accel = Vector2.right;
				Facing = false;
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
					target.Damage(3);
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
