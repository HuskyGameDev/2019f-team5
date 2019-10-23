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

		if (InputManager.GetAxis("CamLeft") > Mathf.Epsilon) x -= 1.0f;
		if (InputManager.GetAxis("CamRight") > Mathf.Epsilon) x += 1.0f;
		if (InputManager.GetAxis("CamUp") > Mathf.Epsilon) y += 1.0f;
		if (InputManager.GetAxis("CamDown") > Mathf.Epsilon) y -= 1.0f;

		Vector2 move = new Vector2(x, y);
		move *= (speed * Time.deltaTime);

		transform.Translate(move);
	}
}
