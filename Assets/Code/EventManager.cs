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
	BossKilled,
	Count
}

// Allows the game to listen for events. Call EventManager.Instance.Subscribe to listen for an event.
public sealed class EventManager : MonoBehaviour
{
	public static EventManager Instance { get; private set; }

	// Stores the list of event callbacks associated with each event.
	private List<Action<object>>[] events = new List<Action<object>>[(int)GameEvent.Count];

	private void Awake()
		=> Instance = this;

	// Listen for the event 'e'. 'func' will be called when event 'e' is signaled.
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

	// Signal event 'e'. All subscribed functions for this event will be
	// called.
	public void SignalEvent(GameEvent e, object data)
	{
		List<Action<object>> list = events[(int)e];

		if (list != null)
		{
			for (int i = 0; i < list.Count; ++i)
				list[i].Invoke(data);
		}
	}

	// Clear all registered events.
	public void Clear()
	{
		for (int i = 0; i < events.Length; ++i)
		{
			if (events[i] != null)
				events[i].Clear();
		}
	}
}
