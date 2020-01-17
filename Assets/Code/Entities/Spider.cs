using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spider : Entity
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
    Vector3 rotateAmmount;

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
			}

			SetFacingDirection(true);
		} else if (aggro)
		{
			if(Math.Abs(PlayerX - transform.position.x) >= .9)
			{
				accel = Vector2.right;
			}

			SetFacingDirection(false);
		}
		
		if((colFlags & CollisionFlags.Sides) != 0 && aggro)
		{
			velocity.y = jumpVelocity;
            gravity = 2;
            
		} else if((colFlags & CollisionFlags.Below) != 0 && aggro) 
        {
			collide = false;
            gravity = -30;
            
        } else if((colFlags & CollisionFlags.Above) != 0 && aggro) 
        {
            rotateAmmount.z = 180;
            gravity = 0;
            transform.Rotate(rotateAmmount);
            if((PlayerY - transform.position.y) <= 0) 
            {
                gravity = -60;
                collide = false;
            }
        } else {
            gravity = -30;
        }


		Move(world, accel, gravity);
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

	protected override void OnCollide(CollideResult col)
	{
		if((colFlags & CollisionFlags.Sides) != 0 && (colFlags & CollisionFlags.Below) != 0){
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
