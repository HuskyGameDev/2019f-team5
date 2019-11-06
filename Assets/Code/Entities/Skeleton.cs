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
        fireRate = 3f;
        nextFire = Time.time;
        health = 15;
    }

    // Update is called once per frame
    void Update()
    {
        FireOrNot();

        float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		if(Math.Abs(PlayerX - transform.position.x) <= 14 && Math.Abs(PlayerY - transform.position.y) < 8)
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

    void FireOrNot(){
        if(Time.time > nextFire && aggro){
            Instantiate(Arrow, transform.position, Quaternion.identity);
            nextFire = Time.time + fireRate;
        }
    }
}
