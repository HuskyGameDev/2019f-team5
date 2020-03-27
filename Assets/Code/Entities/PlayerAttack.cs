using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	World world;
	Entity ent;

	public float swingRate = 0.75f;
	float nextSwing = 0;
	[SerializeField]
	float knockbackAmount = 30;

	private float animDuration = 0.5f;

	Player play;

	private Audiomanager audioManager;

	void Start()
	{
		world = GameObject.FindWithTag("Manager").GetComponent<World>();
		ent = GetComponent<Entity>();
		play = GetComponent<Player>();

		audioManager = GameObject.FindWithTag("Audio").GetComponent<Audiomanager>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && Time.time > nextSwing)
		{
			audioManager.Play("PlayerAttack");
			nextSwing = Time.time + swingRate;
			Swing();
		}
	}

	private void Swing()
	{
		Vector2 cursor = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - play.transform.position)).normalized;
		Vector2 hitloc = (play.Position + cursor) + (Vector2.up / 2);
		AABB box = AABB.FromCenter(hitloc, new Vector2(0.6f, 0.6f));
		List<Entity> entities = world.GetOverlappingEntities(box);
		box.Draw(Color.red, 0.5f);

		if (entities.Count > 0)
		{
			for (int i = 0; i < entities.Count; i++)
			{
				Vector2 knockbackdir = (entities[i].Position - play.Position) * knockbackAmount;
				if (entities[i].gameObject.layer == 10)
				{
					entities[i].Damage(play.damage);
					entities[i].ApplyKnockback(knockbackdir);
				}
			}
		}

		ent.PlayAnimation("Attack Animation");
		ent.SetFacingDirection(cursor.x < 0.0f, animDuration);
	}
}
