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
        Destroy (gameObject, 3f);
    }

    void Update(){
        Move(world, moveDirection, gravity);
    }

    void OnTriggerEnter2D(Collider2D hitInfo){
       
            Destroy(gameObject);
        
        
    }
    
}
