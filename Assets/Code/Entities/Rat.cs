//
// When We Fell
//

using UnityEngine;
using System;

public class Rat : Entity
{

	public float jumpVelocity;
	public float gravity;
	public bool aggro;
	[SerializeField]
	public GameObject player;
	
	private void Start()
	{
		player = GameObject.Find("Player");
	}

    private void Update()
	{
		float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		if(Math.Abs(PlayerX - transform.position.x) <= 10 && Math.Abs(PlayerY - transform.position.y) < .2)
		{
			aggro = true;
		}

		if(Math.Abs(PlayerX - transform.position.x) >= 15)
		{
			aggro = false;
		}

		Vector2 accel = Vector2.zero;

		if(PlayerX < transform.position.x && aggro)
		{
			accel = Vector2.left;
		} else if (aggro)
		{
			accel = Vector2.right;
		}

		Move(world, accel, gravity);
	}
}
