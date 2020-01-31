// 
// When We Fell
//

using UnityEngine;
using System;
using System.Collections.Generic;

public enum GameEvent
{
	PlayerSpawned,
	LevelGenerated,
	Count
}

public sealed class EventManager : MonoBehaviour
{
	public static EventManager Instance { get; private set; }

	private List<Action<object>>[] events = new List<Action<object>>[(int)GameEvent.Count];

	private void Awake()
		=> Instance = this;

	public void Subscribe(GameEvent e, Action<object> func)
	{
		List<Action<object>> list = events[(int)e];

		if (list == null)
		{
			list = new List<Action<object>>();
			events[(int)e] = list;
		}

		list.Add(func);
	}

	public void SignalEvent(GameEvent e, object data)
	{
		List<Action<object>> list = events[(int)e];

		if (list != null)
		{
			for (int i = 0; i < list.Count; ++i)
				list[i].Invoke(data);
		}
	}

	public void Clear()
	{
		for (int i = 0; i < events.Length; ++i)
		{
			if (events[i] != null)
				events[i].Clear();
		}
	}
}
