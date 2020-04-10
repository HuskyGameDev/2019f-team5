using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBomb : Entity
{
    public float delay = 5f;
    float countdown;
    bool hasExploded = false;
    public World world;
    public GameObject Manager;
    public Player PlayerHealth;
    public Entity entity;
    public SpriteRenderer sr;
    private Sprite red, black;
    public float gravity;
    void Start()
    {
        PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Manager = GameObject.FindWithTag("Manager");
        world = Manager.GetComponent<World>();
        countdown = delay;
        sr = GetComponent<SpriteRenderer>();
        red = Resources.Load<Sprite>("Items/red");
        black = Resources.Load<Sprite>("Items/black");

    }
    void Update()
    {
        //The timer for the bomb
        countdown -= Time.deltaTime;
        if (countdown <= 0 && hasExploded == false)
        {
            Explode();
            hasExploded = true;
        }

          if (countdown < 5 && countdown > 3.5)
           {
            sr.sprite = black;
           }
          else if(countdown < 2.5 && countdown > 2)
            {
            sr.sprite = black;
            }
            else if (countdown < 1.2 && countdown > 1)
            {
            sr.sprite = black;
             }
            else
            {
            sr.sprite = red;
            }
        Move(Vector2.zero, -30f);
    }
    //This searches for all objects within a radius of the bomb and then decides how to deal with that object based on it's tag.
    void Explode()
    {
        // The number in this method is the radius of explosion.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Player")
            {
                entity = colliders[i].GetComponent<Entity>();
                entity.Damage(10f);
            }
            if (colliders[i].gameObject.tag == "Enemy")
            {
               entity = colliders[i].GetComponent<Entity>();
                entity.Damage(10f);
            }
            if (colliders[i].gameObject.tag == "Tile")
            {
                Tile tile = world.GetTile((int)colliders[i].transform.position.x, (int)colliders[i].transform.position.y);
                print(colliders[i]);
                if (tile != TileType.Air && tile != TileType.CaveWall)
                {
                    Chunk chunk = world.SetTile((int)colliders[i].transform.position.x, (int)colliders[i].transform.position.y, TileType.CaveWall);
                    chunk.SetModified();
                }

            }
        }
        Destroy(gameObject);
    }
}
