using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
   World world;
   Entity ent;

    float swingRate = 0.75f;
    float nextSwing = 0;
    [SerializeField]
    float knockbackAmount = 30;
    SpriteRenderer spr;
   Player play;
    void Start(){
        world = GameObject.FindWithTag("Manager").GetComponent<World>();
        ent = GetComponent<Entity>();
        play = GetComponent<Player>();
        spr = GetComponent<SpriteRenderer>();
        
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && Time.time > nextSwing){
            nextSwing = Time.time + swingRate;
            Swing();
            ent.PlayAnimation("Attack Animation");
        }
        
        
    }

    

    private void Swing(){
        Vector2 cursor = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - play.transform.position)).normalized;
        Vector2 hitloc = (play.Position + cursor) + (Vector2.up/2); 
        AABB box = AABB.FromCenter(hitloc , new Vector2(1.25f, 1.25f));
        List<Entity> entities = world.GetOverlappingEntities(box);
        box.Draw(Color.red, 0.5f);
        if (entities.Count > 0)
        {
            for(int i = 0; i < entities.Count; i++){
                Vector2 knockbackdir = (entities[i].Position - play.Position) * knockbackAmount;
                Debug.Log(knockbackdir);
                if(entities[i].gameObject.layer == 10){
                    entities[i].Damage(play.damage);
                    entities[i].ApplyForce(knockbackdir);
                    
                }
            }
        }
       
    }

    
}
