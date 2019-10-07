//
// When We Fell
//

using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public float speed;

	private void Update()
	{
		Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		move *= (speed * Time.deltaTime);

		transform.Translate(move);
	}
}
