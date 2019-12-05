using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Item
{
    //bomb is both an active and a collectible
    // Start is called before the first frame update
    void Start()
    {
        
        rarity = 0;
        description = "Boom.";
        id = 0;
        isActive = true;
        onCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //throw the bomb! right now this is right click, soon it will be a designated bomb key
    public override void activeAbility(){
         if(Input.GetMouseButtonDown(1) && play.collectables[0] > 0 && !onCooldown){

        }
    }

}
