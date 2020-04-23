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
	Normal = 0,
	Flying = 1,
	Climbing = 2,
	Swimming = 4
}

public class Player : Entity
{
	public float jumpVelocity;
	public float gravity;
	public float maxHealth = 20;
	public float enemiesKilled = 0;
	public PotionCounter counter;
	int potions;
	public BombCounter counter2;
	int bombs;

	public bool flying;
	public int jumps;

	public int[] collectables = new int[3];

	private PlayerAttack attack;
	 private Inventory inventory;
	private Coroutine loadRoutine;
	private static GameObject damagePopup;

	private void Start()
	{
		counter = GameObject.FindGameObjectWithTag("PotionCounter").GetComponent<PotionCounter>();
		counter2 = GameObject.FindGameObjectWithTag("BombCounter").GetComponent<BombCounter>();
		damage = 5;
		jumps = 0;
		invincibleWait = new WaitForSeconds(0.5f);

		attack = GetComponent<PlayerAttack>();

		for(int i = 0; i < collectables.Length; i++)
			collectables[i] = 0;

		if (PlayerPrefs.HasKey("PlayerData"))
		{
			enemiesKilled = PlayerPrefs.GetFloat("EnemiesKilled");
			health = PlayerPrefs.GetFloat("Health");
			speed = PlayerPrefs.GetFloat("Speed");
			defense = PlayerPrefs.GetFloat("Defense");
			damage = PlayerPrefs.GetFloat("Damage");
			maxHealth = PlayerPrefs.GetFloat("Max Health");
			attack.swingRate = PlayerPrefs.GetFloat("Swing Rate");
			jumpVelocity = PlayerPrefs.GetFloat("Jump Velocity");
			potions = PlayerPrefs.GetInt("Potions");
			bombs = PlayerPrefs.GetInt("Bombs");

		}
		counter.potionCount = potions;
		counter2.bombCount = bombs;
		if (PlayerPrefs.HasKey("Level"))
		{
			int level = PlayerPrefs.GetInt("Level");

			bool skipAnim = false;

			if (PlayerPrefs.HasKey("SkipAnimation"))
			{
				int skipVal = PlayerPrefs.GetInt("SkipAnimation");
				skipAnim = skipVal == 1;
			}

			if (level != 0 && !skipAnim)
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

	private Vector2 SetSwimming()
	{
		Vector2 accel = new Vector2(Input.GetAxisRaw("Horiz"), Input.GetAxisRaw("Vert")) * 0.6f;

		if (accel != Vector2.zero)
			accel = accel.normalized;

		if (Input.GetButtonDown("jump"))
			velocity.y = jumpVelocity * 0.75f;

		friction = -10.0f;
		swimming = true;

		return accel;
	}

	private void Update()
	{
		Vector2 accel;
		float currentGravity = gravity;
		friction = -16.0f;
		swimming = false;

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
			else if ((moveState & MoveState.Swimming) != 0)
			{
				accel = SetSwimming();
				currentGravity = 0.0f;
			}
			else accel = SetNormal();
		}

		Move(accel, currentGravity);
		SetFacingFromAccel(accel);

		if(accel != Vector2.zero)
            PlayAnimation("Walking animation");
		else
		{
            audioManager.Play("Walk");
            PlayAnimation("Static animation");
		}

		if (Debug.isDebugBuild)
		{
			if (Input.GetKeyDown(KeyCode.N))
			{
				SaveData();
				world.NextLevel(true);
			}
		}
		potions = counter.potionCount;
		bombs = counter2.bombCount;
	}

	private void SaveData()
	{
		PlayerPrefs.SetFloat("Health", health);
		PlayerPrefs.SetFloat("Defense", defense);
		PlayerPrefs.SetFloat("Speed", speed);
		PlayerPrefs.SetFloat("Max Health", maxHealth);
		PlayerPrefs.SetFloat("Damage", damage);
		PlayerPrefs.SetFloat("Swing Rate", attack.swingRate);
		PlayerPrefs.SetFloat("Jump Velocity", jumpVelocity);
		PlayerPrefs.SetFloat("EnemiesKilled", enemiesKilled);
		PlayerPrefs.SetInt("PlayerData", 1);
		PlayerPrefs.SetInt("Potions", potions);
		PlayerPrefs.SetInt("Bombs", bombs);
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
				else if (data.overlapType == TileOverlapType.Swim)
					moveState |= MoveState.Swimming;

				if (result.tile == TileType.Lava)
					AddOverTimeDamage(data.otDamage);

				if (result.tile == TileType.EndLevelTile)
					LoadNextLevel();
			}
		}
	}

	public void LoadNextLevel()
	{
		if (loadRoutine == null)
			loadRoutine = StartCoroutine(LoadNextLevelRoutine());
	}

	private IEnumerator LoadNextLevelRoutine()
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

	private void OnApplicationQuit()
		=> PlayerPrefs.DeleteAll();
}
