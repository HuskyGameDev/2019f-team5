using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private Slot slot;
    private Inventory inventory;
    public int j;
    public Player PlayerHealth;
    public void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    //Checks to see what key is pressed down when using a potion. I'm sure there is a much better way to do this, but I wasn't sure how.
        private void Update()
    {
        if (Input.GetKeyDown("1") && inventory.full[0])
        {
            UsePotion();
            inventory.full[0] = false;
        }
        if (Input.GetKeyDown("2") && inventory.full[1])
        {
            if (inventory.full[0])
            {
                inventory.full[0] = false;
                UsePotion();
                inventory.full[0] = true;
                inventory.full[1] = false;
            }
            else if(!inventory.full[0])
            {
                UsePotion();
                inventory.full[1] = false;
            }
        }
        if (Input.GetKeyDown("3") && inventory.full[2])
        {
            if (inventory.full[0] && inventory.full[1])
            {
                inventory.full[0] = false;
                inventory.full[1] = false;
                UsePotion();
                inventory.full[0] = true;
                inventory.full[1] = true;
                inventory.full[2] = false;
            }
             if (!inventory.full[0] && inventory.full[1])
            {
                inventory.full[1] = false;
                UsePotion();
                inventory.full[1] = true;
                inventory.full[2] = false;
            }
            if (inventory.full[0] && !inventory.full[1])
            {
                inventory.full[0] = false;
                UsePotion();
                inventory.full[0] = true;
                inventory.full[2] = false;
            }
            if (!inventory.full[0] && !inventory.full[1])
            {
                UsePotion();
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
                UsePotion();
                inventory.full[0] = true;
                inventory.full[1] = true;
                inventory.full[2] = true;
                inventory.full[3] = false;
            }
            if (inventory.full[0] && inventory.full[1] && !inventory.full[2])
            {
                inventory.full[0] = false;
                inventory.full[1] = false;
                UsePotion();
                inventory.full[0] = true;
                inventory.full[1] = true;
                inventory.full[3] = false;
            }
            if (!inventory.full[0] && inventory.full[1] && inventory.full[2])
            {
                inventory.full[2] = false;
                inventory.full[1] = false;
                UsePotion();
                inventory.full[2] = true;
                inventory.full[1] = true;
                inventory.full[3] = false;
            }
            if (inventory.full[0] && !inventory.full[1] && inventory.full[2])
            {
                inventory.full[0] = false;
                inventory.full[2] = false;
                UsePotion();
                inventory.full[0] = true;
                inventory.full[2] = true;
                inventory.full[3] = false;
            }
            if (!inventory.full[0] && !inventory.full[1] && inventory.full[2])
            {
                inventory.full[2] = false;
                UsePotion();
                inventory.full[2] = true;
                inventory.full[3] = false;
            }
            if (!inventory.full[0] && inventory.full[1] && !inventory.full[2])
            {
                inventory.full[1] = false;
                UsePotion();
                inventory.full[1] = true;
                inventory.full[3] = false;
            }
            if (inventory.full[0] && !inventory.full[1] && !inventory.full[2])
            {
                inventory.full[0] = false;
                UsePotion();
                inventory.full[0] = true;
                inventory.full[3] = false;
            }
            if (!inventory.full[0] && !inventory.full[1] && !inventory.full[2])
            {
                UsePotion();
                inventory.full[3] = false;
            }
        }

    }
    //Gives the player health and removes the object
    public void UsePotion()
    {
        for (int j = 0; j < inventory.slot.Length; j++)
        {
            if (inventory.full[j])
            {
                if(PlayerHealth.maxHealth <= PlayerHealth.health + 5)
                {
                    PlayerHealth.health = PlayerHealth.maxHealth;
                }
                else
                {
                    PlayerHealth.health = PlayerHealth.health + 10.0f;
                }
                slot = GameObject.FindGameObjectWithTag("Slot"+j).GetComponent<Slot>();
                slot.DropItem();
                break;
            }
        }
    }

}
