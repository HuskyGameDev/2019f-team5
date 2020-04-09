using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBomb : MonoBehaviour
{
    public float delay = 4f;
    float countdown;
    bool hasExploded = false;
    public World world;
    public GameObject Manager;
    public Player PlayerHealth;
    void Start()
    {
        PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Manager = GameObject.FindWithTag("Manager");
        world = Manager.GetComponent<World>();
        countdown = delay;

    }
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0 && hasExploded == false)
        {
            Explode();
            hasExploded = true;
        }
    }
    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Player")
            {
                PlayerHealth.health = PlayerHealth.health - 10;
            }
            if (colliders[i].gameObject.tag == "Enemy")
            {
                Destroy(colliders[i].gameObject);
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
