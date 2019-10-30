using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
   World world;
    void Start(){
        world = GameObject.FindWithTag("Manager").GetComponent<World>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Swing();
        }
    }

    private void Swing(){
        Vector3 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //this will currently hit wherever the cursor is, going to change it to
        //hit a set amount from the player in the direction clicked next sprint
        AABB box = AABB.FromCenter(cursor , new Vector2(0.35f, 0.35f));
       
        List<Entity> entities = world.GetOverlappingEntities(box);

        if (entities.Count > 0)
        {
            for(int i = 0; i < entities.Count; i++){
                if(!entities[i].name.Equals("Arrow")){
                    Debug.Log("Hit");
                }
            }
        }

    }

    
}
