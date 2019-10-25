//
// When We Fell
//

using UnityEngine;

public class Player : Entity
{
	public World world;
	public float jumpVelocity;

	private void Update()
	{
		Vector2 accel = new Vector2(Input.GetAxisRaw("Horiz"), 0.0f);

		float gravity = -30.0f;

		if ((colFlags & CollisionFlags.Below) != 0)
		{
			if (Input.GetKey(KeyCode.Space))
				velocity.y = jumpVelocity;
		}

		Move(world, accel, gravity);
	}
}
