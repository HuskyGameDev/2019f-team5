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
    public void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    //Checks to see what key is pressed down when using a bomb. I'm sure there is a much better way to do this, but I wasn't sure how.
    private void Update()
    {
        if (Input.GetKeyDown("1") && inventory.full[0])
        {
            UseBomb();
            inventory.full[0] = false;
        }
        if (Input.GetKeyDown("2") && inventory.full[1])
        {
            if (inventory.full[0])
            {
                inventory.full[0] = false;
                UseBomb();
                inventory.full[0] = true;
                inventory.full[1] = false;
            }
            else if (!inventory.full[0])
            {
                UseBomb();
                inventory.full[1] = false;
            }
        }
        if (Input.GetKeyDown("3") && inventory.full[2])
        {
            if (inventory.full[0] && inventory.full[1])
            {
                inventory.full[0] = false;
                inventory.full[1] = false;
                UseBomb();
                inventory.full[0] = true;
                inventory.full[1] = true;
                inventory.full[2] = false;
            }
            if (!inventory.full[0] && inventory.full[1])
            {
                inventory.full[1] = false;
                UseBomb();
                inventory.full[1] = true;
                inventory.full[2] = false;
            }
            if (inventory.full[0] && !inventory.full[1])
            {
                inventory.full[0] = false;
                UseBomb();
                inventory.full[0] = true;
                inventory.full[2] = false;
            }
            if (!inventory.full[0] && !inventory.full[1])
            {
                UseBomb();
                inventory.full[2] = false;
            }

        }
        if (Input.GetKeyDown("4") && inventory.full[3])
        {
            if (inventory.full[0] && inventory.full[1] && inventory.full[2])
            {
                inventory.full[0] = false;
                inventory.full[1] = false;
                inventory.full[2] = false;
                UseBomb();
                inventory.full[0] = true;
                inventory.full[1] = true;
                inventory.full[2] = true;
                inventory.full[3] = false;
            }
            if (inventory.full[0] && inventory.full[1] && !inventory.full[2])
            {
                inventory.full[0] = false;
                inventory.full[1] = false;
                UseBomb();
                inventory.full[0] = true;
                inventory.full[1] = true;
                inventory.full[3] = false;
            }
            if (!inventory.full[0] && inventory.full[1] && inventory.full[2])
            {
                inventory.full[2] = false;
                inventory.full[1] = false;
                UseBomb();
                inventory.full[2] = true;
                inventory.full[1] = true;
                inventory.full[3] = false;
            }
            if (inventory.full[0] && !inventory.full[1] && inventory.full[2])
            {
                inventory.full[0] = false;
                inventory.full[2] = false;
                UseBomb();
                inventory.full[0] = true;
                inventory.full[2] = true;
                inventory.full[3] = false;
            }
            if (!inventory.full[0] && !inventory.full[1] && inventory.full[2])
            {
                inventory.full[2] = false;
                UseBomb();
                inventory.full[2] = true;
                inventory.full[3] = false;
            }
            if (!inventory.full[0] && inventory.full[1] && !inventory.full[2])
            {
                inventory.full[1] = false;
                UseBomb();
                inventory.full[1] = true;
                inventory.full[3] = false;
            }
            if (inventory.full[0] && !inventory.full[1] && !inventory.full[2])
            {
                inventory.full[0] = false;
                UseBomb();
                inventory.full[0] = true;
                inventory.full[3] = false;
            }
            if (!inventory.full[0] && !inventory.full[1] && !inventory.full[2])
            {
                UseBomb();
                inventory.full[3] = false;
            }
        }

    }
    //Gives the player health and removes the object
    public void UseBomb()
    {
        for (int j = 0; j < inventory.slot.Length; j++)
        {
            if (inventory.full[j])
            {
                player = GameObject.Find("Player");
                Instantiate(ActiveBomb, player.transform.position+(player.transform.forward * 2), transform.rotation);
                slot = GameObject.FindGameObjectWithTag("Slot" + j).GetComponent<Slot>();
                slot.DropItem();
                break;
            }
        }
    }
}
