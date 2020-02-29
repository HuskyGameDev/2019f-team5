//
// When We Fell
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Flags]
public enum MoveState
{
	Normal,
	Flying,
	Climbing
}

public class Player : Entity
{
	public float jumpVelocity;
	public float gravity;
	public float maxHealth = 20;

	public bool flying;
	public int jumps;

	public int[] collectables = new int[3];

	private PlayerAttack attack;
	private Coroutine loadRoutine;

	private void Start()
	{
		damage = 5;
		jumps = 0;
		invincibleWait = new WaitForSeconds(0.5f);

		attack = GetComponent<PlayerAttack>();

		for(int i = 0; i < collectables.Length; i++)
			collectables[i] = 0;

		if (PlayerPrefs.HasKey("PlayerData"))
		{
			health = PlayerPrefs.GetFloat("Health");
			speed = PlayerPrefs.GetFloat("Speed");
			defense = PlayerPrefs.GetFloat("Defense");
			damage = PlayerPrefs.GetFloat("Damage");
			maxHealth = PlayerPrefs.GetFloat("Max Health");
			attack.swingRate = PlayerPrefs.GetFloat("Swing Rate");
		}

		if (PlayerPrefs.HasKey("Level"))
		{
			int level = PlayerPrefs.GetInt("Level");

			if (level != 0)
			{
				Disable();
				invincible = true;

				t.localScale = Vector3.zero;
				StartCoroutine(LoadIn());
			}
		}
	}

	private Vector2 SetNormal()
	{
		Vector2 accel = new Vector2(Input.GetAxisRaw("Horiz"), 0.0f);

		if (jumps < 1)
		{
			if (Input.GetButtonDown("jump"))
			{
				audioManager.Play("Jump");
                velocity.y = jumpVelocity;
				jumps++;
			}
		}

		if (CollidedBelow())
			jumps = 0;

		return accel;
	}

	private Vector2 SetFlying()
	{
		Vector2 accel = new Vector2(Input.GetAxisRaw("Horiz"), Input.GetAxisRaw("Vert"));

		if (accel != Vector2.zero)
			accel = accel.normalized;

		return accel;
	}

	private Vector2 SetClimbing()
	{
		Vector2 accel = new Vector2(Input.GetAxisRaw("Horiz"), Input.GetAxisRaw("Vert"));

		if (accel != Vector2.zero)
			accel = accel.normalized;

		jumps = 0;
		return accel;
	}

	private void Update()
	{
		Vector2 accel;
		float currentGravity = gravity;

		if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.Tab))
			flying = !flying;

		if (flying)
		{
			accel = SetFlying();
			currentGravity = 0.0f;
		}
		else
		{
			if ((moveState & MoveState.Climbing) != 0)
			{
                accel = SetClimbing();
				currentGravity = 0.0f;
			}
			else accel = SetNormal();
		}

		Move(accel, currentGravity);

		if(accel != Vector2.zero)
            PlayAnimation("Walking animation");
		else
		{
            audioManager.Play("Walk");
            PlayAnimation("Static animation");
		}
	}

	private void SaveData()
	{
		PlayerPrefs.SetFloat("Health", health);
		PlayerPrefs.SetFloat("Defense", defense);
		PlayerPrefs.SetFloat("Speed", speed);
		PlayerPrefs.SetFloat("Max Health", maxHealth);
		PlayerPrefs.SetFloat("Damage", damage);
		PlayerPrefs.SetFloat("Swing Rate", attack.swingRate);
		PlayerPrefs.SetInt("PlayerData", 1);
	}

	protected override void HandleOverlaps(List<CollideResult> overlaps)
	{
		for (int i = 0; i < overlaps.Count; ++i)
		{
			CollideResult result = overlaps[i];

			if (result.entity == null)
			{
				TileData data = TileManager.GetData(result.tile);

				if (data.overlapType == TileOverlapType.Climb)
					moveState |= MoveState.Climbing;

				if (result.tile == TileType.EndLevelTile)
				{
					if (loadRoutine == null)
						loadRoutine = StartCoroutine(LoadNextLevel());
				}
			}
		}
	}

	private IEnumerator LoadNextLevel()
	{
		Disable();
		invincible = true;

		float timeLeft = 1.0f;
		Vector3 start = t.localScale;
		Vector3 end = Vector3.zero;

		while (timeLeft >= 0.0f)
		{
			t.localScale = Vector3.Lerp(end, start, timeLeft);
			timeLeft -= Time.deltaTime;
			yield return null;
		}

		SaveData();
		world.NextLevel();
	}

	private IEnumerator LoadIn()
	{
		float timeLeft = 1.0f;
		Vector3 start = t.localScale;
		Vector3 end = new Vector3(1.0f, 1.0f);

		while (timeLeft >= 0.0f)
		{
			t.localScale = Vector3.Lerp(end, start, timeLeft);
			timeLeft -= Time.deltaTime;
			yield return null;
		}

		enabled = true;
		GetComponent<PlayerAttack>().enabled = true;
		invincible = false;
	}

	private void Disable()
	{
		enabled = false;
		GetComponent<PlayerAttack>().enabled = false;

		if (chunk != null)
			chunk.RemoveEntity(this);
	}

	protected override void OnKill()
	{
		rend.enabled = false;
		Disable();
		StartCoroutine(LoadGameOver());
	}

	private IEnumerator LoadGameOver()
	{
		yield return new WaitForSeconds(3.0f);
		SceneManager.LoadScene("Lose Menu");
	}
}
