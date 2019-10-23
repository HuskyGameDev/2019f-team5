//
// When We Fell
//

using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public float speed;

	private void Update()
	{
		float x = 0.0f;
		float y = 0.0f;

		if (Input.GetKey(KeyCode.A)) x -= 1.0f;
		if (Input.GetKey(KeyCode.D)) x += 1.0f;
		if (Input.GetKey(KeyCode.W)) y += 1.0f;
		if (Input.GetKey(KeyCode.S)) y -= 1.0f;

		Vector2 move = new Vector2(x, y);
		move *= (speed * Time.deltaTime);

		transform.Translate(move);
	}
}
