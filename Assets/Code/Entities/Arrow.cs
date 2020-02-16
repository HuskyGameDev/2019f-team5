using System.Collections.Generic;
using UnityEngine;

public class Arrow : Entity
{
    public float gravity;
    private static Player target;
    private Vector2 moveDirection;

    private void Start()
	{
        audioManager.Play("Skeleton Attack");

        if (target == null)
			target = GameObject.FindWithTag("Player").GetComponent<Player>();

		Vector3 targetP = target.transform.position;
		Vector3 pos = transform.position;

		moveDirection = (targetP - pos).normalized * speed;
        float angle;

        if (targetP.x < pos.x)
			angle = 180 + Mathf.Atan2 (targetP.y - pos.y, targetP.x - pos.x) * Mathf.Rad2Deg;
        else angle = Mathf.Atan2 (targetP.y - pos.y, targetP.x - pos.x) * Mathf.Rad2Deg;
      
		transform.rotation = Quaternion.Euler (new Vector3(0, 0, angle));
    }

    private void Update()
		=> Move(moveDirection, gravity);

    protected override void OnCollide(CollideResult col)
		=> Destroy(gameObject);

    protected override void HandleOverlaps(List<CollideResult> overlaps)
	{
		for (int i = 0; i < overlaps.Count; ++i)
		{
			CollideResult result = overlaps[i];
			Entity target = result.entity;

			if (target != null && target is Player)
			{
				Vector2 diff = (target.Position - Position).normalized;

				if (diff.y > 0.4f)
				{
					Damage(5);
					target.ApplyKnockback(0.0f, 7.5f);
				}
				else
				{
					Vector2 force = diff * knockbackForce;
					target.Damage(3, force);
				}
			}
		}
	}
}
