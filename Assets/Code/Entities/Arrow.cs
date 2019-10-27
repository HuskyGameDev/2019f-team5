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
    }

    void Update()
	{
        Move(world, moveDirection, gravity);
    }

    protected override void OnCollide(CollideResult col)
	{
		Destroy(gameObject);
    }
    
}
