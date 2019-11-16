//
// When We Fell
//

using UnityEngine;
using System.Collections.Generic;

public class DebugHelper : MonoBehaviour
{
	private class Outline
	{
		public float timeLeft;
		public Color color;
		public Vector2 pos;
		public Vector2 size;
	}

	public static bool showOutlines { get; private set; }

	private static List<Outline> outlines = new List<Outline>();

	public static void ShowOutline(Vector2 pos, Vector2 size, Color color, float time = -1.0f)
	{
		Outline o = new Outline();
		o.color = color;
		o.timeLeft = time;
		o.pos = pos;
		o.size = size;

		outlines.Add(o);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
			showOutlines = !showOutlines;

		for (int i = outlines.Count - 1; i >= 0; --i)
		{
			Outline o = outlines[i];

			if (o.timeLeft != -1.0f)
				o.timeLeft -= Time.deltaTime;

			if (o.timeLeft <= 0.0f)
				outlines.RemoveAt(i);
		}
	}

	private void OnDrawGizmos()
	{
		for (int i = outlines.Count - 1; i >= 0; --i)
		{
			Outline o = outlines[i];
			Gizmos.color = o.color;
			Gizmos.DrawWireCube(outlines[i].pos, outlines[i].size);

			if (o.timeLeft == -1.0f)
				outlines.RemoveAt(i);
		}
	}
}
