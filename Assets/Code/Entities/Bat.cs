//
// When We Fell
//

using UnityEngine;
using System;

public class Bat : Entity
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

		if(Math.Abs(PlayerX - transform.position.x) <= 6 && Math.Abs(PlayerY - transform.position.y) < 6)
		{
			aggro = true;
		}

		if(Math.Abs(PlayerX - transform.position.x) >= 10 || Math.Abs(PlayerY - transform.position.y) >= 10)
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

		Move(world, accel, 0);
	}
}