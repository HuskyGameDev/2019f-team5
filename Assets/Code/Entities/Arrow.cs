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
    
}
