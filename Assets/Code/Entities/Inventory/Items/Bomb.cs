using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject player;
    public GameObject ActiveBomb;
    private Slot slot;
    private Inventory inventory;
    public int j;
    public float gravity;
    private Vector2 moveDirection;
    public BombCounter counter;
    public void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        counter = GameObject.FindGameObjectWithTag("BombCounter").GetComponent<BombCounter>();
    }
    //Checks to see what key is pressed down when using a bomb. I'm sure there is a much better way to do this, but I wasn't sure how.
    private void Update()
    {
        if (Input.GetKeyDown("2") && counter.bombCount !=0) {
            UseBomb();

        }

    }
    //Gives the player health and removes the object
    public void UseBomb()
    {
              //  player = GameObject.Find("Player");
                Instantiate(ActiveBomb, player.transform.position+(player.transform.forward * 2), transform.rotation);
                counter.bombCount++;

    }
}
