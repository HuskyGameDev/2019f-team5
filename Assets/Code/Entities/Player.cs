//
// When We Fell
//

using UnityEngine;

public class Player : Entity
{
	public float jumpVelocity;
	public float gravity;

	public bool flying;
	public int jumps;

	private void Start() 
	{
		jumps = 0;
	}

	private void Update()
	{
		Vector2 accel;
		float currentGravity = gravity;

		if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.Tab))
			flying = !flying;

		if (flying)
		{
			accel = new Vector2(Input.GetAxisRaw("Horiz"), Input.GetAxisRaw("Vert"));

			if (accel != Vector2.zero)
				accel = accel.normalized;

			currentGravity = 0.0f;
		}
		else
		{
			accel = new Vector2(Input.GetAxisRaw("Horiz"), 0.0f);

			if (jumps < 1)
			{
				if (Input.GetButtonDown("jump"))
				{
					velocity.y = jumpVelocity;
					jumps++;
				}
			}

			if ((colFlags & CollisionFlags.Below) != 0)
			{
				jumps = 0;
			}
		}

		Move(world, accel, currentGravity);

		if(accel != Vector2.zero) {
			PlayAnimation("Walking animation");
		} else {
			PlayAnimation("Static animation");
		}
	}

	protected override void OnCollide(CollideResult result)
	{

	}

}
