//
// When We Fell
//

using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

public sealed class Pathfinder
{
	private PathCellInfo[,] grid;

	private Vector2Int start;
	private Vector2Int target;
	private Stack<Vector2> path;

	private World world;
	private PathNode[,] nodes;

	private SortedList<Vector2Int, PathNode> openList = new SortedList<Vector2Int, PathNode>();
	private HashSet<PathNode> closedList = new HashSet<PathNode>();

	private int successorCount = 0;
	private PathNode[] successors = new PathNode[8];

	private Vector2Int[] directions = new Vector2Int[4];

	public Pathfinder(World world, PathCellInfo[,] grid)
	{
		this.world = world;
		this.grid = grid;

		directions[0] = Vector2Int.left;
		directions[1] = Vector2Int.right;
		directions[2] = Vector2Int.up;
		directions[3] = Vector2Int.down;

		RectInt bounds = world.GetBounds();
		nodes = new PathNode[bounds.width, bounds.height];
	}

	private bool InBounds(Vector2Int p)
	{
		RectInt bounds = world.GetBounds();
		return p.x >= bounds.min.x && p.y >= bounds.min.y && p.x <= bounds.max.x && p.y <= bounds.max.y;
	}

	// Fills the path to follow into a stack.
	private void TracePath(PathNode dest)
	{
		PathNode current = dest;

		while (current.pos != start)
		{
			path.Push(new Vector2(current.pos.x + 0.5f, current.pos.y + 0.5f));
			current = current.parent;
			Assert.IsNotNull(current);
		}
	}

	// Compute the estimated number of cells to reach the destination 
	// using Manhattan distance.
	private int ComputeHeuristic(Vector2Int start, Vector2Int end)
		=> Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);

	// An implementation of the A* pathfinding algorithm.
	public void FindPath(Vector2Int start, Vector2Int target, Stack<Vector2> path)
	{
		this.start = start;
		this.target = target;
		this.path = path;

		openList.Add(start, GetNode(start));

		while (openList.Count > 0)
		{
			PathNode current = openList.First;
			openList.RemoveFirst(current.pos, current);
			closedList.Add(current);

			if (current.pos == target)
			{
				TracePath(current);
				return;
			}

			GetSuccessors(current, current.pos);

			for (int i = 0; i < successorCount; i++)
			{
				PathNode next = successors[i];

				if (closedList.Contains(next))
					continue;

				Vector2Int nP = next.pos;
				int cost = grid[nP.x, nP.y].cost;
				int newG = current.g + cost;

				if (!openList.TryGetValue(next.pos, out PathNode node))
				{
					next.g = newG;
					next.h = ComputeHeuristic(nP, target);
					next.f = next.g + next.h;
					next.parent = current;
					openList.Add(next.pos, next);
				}
				else
				{
					if (newG < node.g)
					{
						openList.Remove(node);
						node.g = newG;
						node.f = node.g + node.h;
						node.parent = current;
						openList.Add(node);
					}
				}
			}
		}
	}

	private PathNode GetNode(Vector2Int p)
	{
		PathNode node = nodes[p.x, p.y];

		if (node == null)
		{
			node = new PathNode();
			nodes[p.x, p.y] = node;
		}

		node.pos = p;
		return node;
	}

	private void GetSuccessors(PathNode current, Vector2Int pos)
	{
		successorCount = 0;

		for (int i = 0; i < 4; i++)
		{
			Vector2Int next = pos + directions[i];

			if (InBounds(next) && grid[next.x, next.y].passable)
				successors[successorCount++] = GetNode(next);
		}
	}

	public void Clear()
	{
		openList.Clear();
		closedList.Clear();
	}
}