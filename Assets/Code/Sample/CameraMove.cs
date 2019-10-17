//
// When We Fell
//

using UnityEngine;
using Luminosity.IO;

public class CameraMove : MonoBehaviour
{
	public float speed;

	private void Update()
	{
		float x = 0.0f;
		float y = 0.0f;

		if (InputManager.GetKey(KeyCode.A)) x -= 1.0f;
		if (InputManager.GetKey(KeyCode.D)) x += 1.0f;
		if (InputManager.GetKey(KeyCode.W)) y += 1.0f;
		if (InputManager.GetKey(KeyCode.S)) y -= 1.0f;

		Vector2 move = new Vector2(x, y);
		move *= (speed * Time.deltaTime);

		transform.Translate(move);
	}
}
