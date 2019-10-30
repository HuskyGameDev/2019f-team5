//
// When We Fell
//

using UnityEngine;
using System;

public class Slime : Entity
{
	public float jumpVelocity;
	public float gravity;
	public bool aggro;
	public float jumpTime = 1.5f;
	[SerializeField]
	public GameObject player;
	
	private void Start()
	{
		player = GameObject.Find("Player");
	}
    private void Update()
	{	
		jumpTime -= Time.deltaTime;

		float PlayerY = player.transform.position.y;
		float PlayerX = player.transform.position.x;

		if(Math.Abs(PlayerX - transform.position.x) <= 8 && Math.Abs(PlayerY - transform.position.y) < 8)
		{
			aggro = true;
		}

		if(Math.Abs(PlayerX - transform.position.x) >= 15 && Math.Abs(PlayerY - transform.position.y) >= 15)
		{
			aggro = false;
		}

		Vector2 accel = Vector2.zero;

		if(PlayerX < transform.position.x && aggro && (colFlags & CollisionFlags.Below) == 0)
		{
			accel += Vector2.left;
		} else if (PlayerX > transform.position.x && aggro && (colFlags & CollisionFlags.Below) == 0)
		{	
			accel += Vector2.right;
		}
		
		if(aggro && (colFlags & CollisionFlags.Below) != 0)
		{
			if(jumpTime <= 0) {
				velocity.y = jumpVelocity;
				jumpTime = 1.5f;
			}
		}

		Move(world, accel, gravity);
	}
}
