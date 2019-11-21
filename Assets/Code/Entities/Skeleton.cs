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
			accel += Vector2.left;
		} else if (aggro)
		{
			accel += Vector2.right;
		}
		
		if(PlayerY < transform.position.y && aggro)
		{
			accel += Vector2.down;
		} else if (aggro)
		{
			accel += Vector2.up;
		}

		Move(world, accel, -30);
    }

    IEnumerator FireOrNot(){
        if(Time.time > nextFire && aggro){
			PlayAnimation("SkeletonAttack");
			yield return new WaitForSeconds(.5f);
			if(Time.time > nextFire) {
            Instantiate(Arrow, transform.position, Quaternion.identity);
			}
            nextFire = Time.time + fireRate;
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
					target.ApplyForce(0.0f, 7.5f);
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
