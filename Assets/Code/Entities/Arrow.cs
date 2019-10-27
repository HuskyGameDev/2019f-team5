using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Entity
{
    public float gravity;
    Player target;
    Vector2 moveDirection;
    void Start(){
        target = GameObject.FindObjectOfType<Player>();
        moveDirection = (target.transform.position - transform.position).normalized * speed;
        Move(world, moveDirection, gravity);
      
    }

    void Update(){
        Move(world, moveDirection, gravity);
    }

    protected override void OnCollide(CollideResult col){
            Destroy(gameObject);
    }
    
}
