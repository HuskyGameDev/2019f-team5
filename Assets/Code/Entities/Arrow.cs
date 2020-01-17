using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Entity
{
    public float gravity;
    static Player target;
    Vector2 moveDirection;

    void Start()
	{
		if (target == null)
			target = GameObject.FindWithTag("Player").GetComponent<Player>();

        moveDirection = (target.transform.position - transform.position).normalized * speed;
        float angle;
        if(target.transform.position.x < transform.position.x)
        {
            angle = 180 + Mathf.Atan2 (target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        } else
        {
            angle = Mathf.Atan2 (target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        }
        this.transform.rotation = Quaternion.Euler (new Vector3(0, 0, angle));
    }

    void Update()
	{
        Move(world, moveDirection, gravity);
    }

    protected override void OnCollide(CollideResult col)
	{
		Destroy(gameObject);
    }

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
