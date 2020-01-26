//
// Quest Maker
//

using UnityEngine;
using System.Collections.Generic;

public sealed class DebugServices : MonoBehaviour
{
	public struct Outline
	{
		public Mesh mesh;
		public float timeLeft;

		public Outline(Mesh mesh, float timeLeft)
		{
			this.mesh = mesh;
			this.timeLeft = timeLeft;
		}
	}

	public Material mat;

	private List<Vector3> vertices;
	private List<int> indices;
	private List<Color> colors;

	private Dictionary<AABB, Mesh> outlinePool = new Dictionary<AABB, Mesh>();

	private List<Outline> outlines = new List<Outline>();

	public static DebugServices Instance;

	public bool ShowDebug { get; private set; }

	private void Awake()
	{
		Instance = this;
		vertices = new List<Vector3>(16);
		indices = new List<int>();
		colors = new List<Color>();

		if (!Debug.isDebugBuild)
			enabled = false;
	}

	private void MakeEdge(Vector2 start, Vector2 end, float thickness, bool vertical)
	{
		float half = thickness * 0.5f;
		int offset = vertices.Count;

		indices.Add(offset + 2);
		indices.Add(offset + 1);
		indices.Add(offset);

		indices.Add(offset + 3);
		indices.Add(offset + 2);
		indices.Add(offset);

		if (vertical)
		{
			vertices.Add(new Vector3(end.x + half, start.y - half, -1.0f));
			vertices.Add(new Vector3(end.x + half, end.y + half, -1.0f));
			vertices.Add(new Vector3(start.x - half, end.y + half, -1.0f));
			vertices.Add(new Vector3(start.x - half, start.y - half, -1.0f));
		}
		else
		{
			vertices.Add(new Vector3(end.x + half, start.y - half, -1.0f));
			vertices.Add(new Vector3(end.x + half, end.y + half, -1.0f));
			vertices.Add(new Vector3(start.x - half, end.y + half, -1.0f));
			vertices.Add(new Vector3(start.x - half, start.y - half, -1.0f));
		}
	}

	private void MakeOutline(ref Mesh mesh, Vector2 min, Vector2 max, Color color)
	{
		float thickness = 0.05f;

		vertices.Clear();
		indices.Clear();
		colors.Clear();

		Vector2 bl = new Vector2(min.x - thickness, min.y - thickness);
		Vector2 tr = new Vector2(max.x + thickness, max.y + thickness);
		Vector2 br = new Vector2(tr.x, bl.y);
		Vector2 tl = new Vector2(bl.x, tr.y);

		MakeEdge(bl, br, thickness, false);
		MakeEdge(tl, tr, thickness, false);
		MakeEdge(bl, tl, thickness, true);
		MakeEdge(br, tr, thickness, true);

		for (int i = 0; i < 16; ++i)
			colors.Add(color);

		mesh = new Mesh();

		mesh.SetVertices(vertices);
		mesh.SetTriangles(indices, 0);
		mesh.SetColors(colors);
	}

	private Mesh GetMesh(AABB bb, Color color)
	{
		if (!outlinePool.TryGetValue(bb, out Mesh mesh))
			MakeOutline(ref mesh, bb.BottomLeft, bb.TopRight, color);

		return mesh;
	}

	// Draw an outline for the specified period of time.
	public void DrawOutline(AABB bb, Color color, float time)
	{
		if (ShowDebug)
		{
			Mesh mesh = GetMesh(bb, color);
			Outline outline = new Outline(mesh, time);
			outlines.Add(outline);
		}
	}

	// Draw an outline for a single frame.
	public void DrawOutline(AABB bb, Color color)
	{
		if (ShowDebug)
		{
			Mesh mesh = GetMesh(bb, color);
			Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, mat, 0);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
			ShowDebug = !ShowDebug;

		if (ShowDebug)
		{
			for (int i = outlines.Count - 1; i >= 0; --i)
			{
				Outline outline = outlines[i];
				Graphics.DrawMesh(outline.mesh, Vector3.zero, Quaternion.identity, mat, 0);
				outline.timeLeft -= Time.deltaTime;

				if (outline.timeLeft <= 0.0f)
					outlines.RemoveAt(i);
				else outlines[i] = outline;
			}
		}
	}
}