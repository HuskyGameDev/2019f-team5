//
// When We Fell
//

using UnityEngine;
using System;

public class TestCameraMove : MonoBehaviour
{
	public float speed;

	private void Update()
	{
		float x = 0.0f;
		float y = 0.0f;

		if (Input.GetAxis("CamHoriz") > Mathf.Epsilon) x=speed;
		if (Input.GetAxis("CamHoriz") < -Mathf.Epsilon) x=-speed;
		if (Input.GetAxis("CamVert") > Mathf.Epsilon) y=speed;
		if (Input.GetAxis("CamVert") < -Mathf.Epsilon) y=-speed;

		Vector2 move = new Vector2(x, y);

		transform.Translate(move);
	}
}
