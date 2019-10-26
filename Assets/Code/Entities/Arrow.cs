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
        moveDirection = (target.transform.position - transform.position).normalized;
        Move(world, moveDirection, gravity);
        Destroy (gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo){
        if(hitInfo.gameObject.name.Equals("Player") ){
            Destroy(gameObject);
        }
        
    }
    
}
