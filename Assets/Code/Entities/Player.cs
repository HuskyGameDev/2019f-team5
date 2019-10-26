//
// When We Fell
//

using UnityEngine;

public class Player : Entity
{
	public World world;

	public float jumpVelocity;
	public float gravity;

	[SerializeField]
	public int jumps = 0;

	private void Update()
	{
		Vector2 accel = new Vector2(Input.GetAxisRaw("Horiz"), 0.0f);

		if (jumps < 1) {
			if (Input.GetButtonDown("jump")) {
				velocity.y = jumpVelocity;
				jumps++;
			}	
		}

		if ((colFlags & CollisionFlags.Below) != 0)
		{
			jumps=0;
		}

		Move(world, accel, gravity);
	}
}
