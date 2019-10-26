//
// When We Fell
//

using UnityEngine;

public class Slime : Entity
{

	public float jumpVelocity;
	public float gravity = 0;

	private void Update()
	{
		Vector2 accel = new Vector2(Input.GetAxisRaw("Horiz"), 0.0f);

		if ((colFlags & CollisionFlags.Below) != 0)
		{
			if (Input.GetKey(KeyCode.Space))
				velocity.y = jumpVelocity;
		}

	//	Move(world, accel, gravity);
	}
}
