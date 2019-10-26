//
// When We Fell
//

using UnityEngine;

public class Player : Entity
{
	public float jumpVelocity;
	public float gravity;

	[SerializeField]
	public int jumps;

	private void Start() 
	{
		jumps = 0;
	}

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

	protected override void OnCollide(CollideResult result)
	{
		if (result.entity != null)
			Debug.Log("Collided with entity");
	}
}
