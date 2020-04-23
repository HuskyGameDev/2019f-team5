using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private Slot slot;
    private Inventory inventory;
    public int j;
    public Player PlayerHealth;
    public PotionCounter counter;
    public void Start()

    {
        counter = GameObject.FindGameObjectWithTag("PotionCounter").GetComponent<PotionCounter>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
       // PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    //Checks to see what key is pressed down when using a potion.
        private void Update()
    {
        if (counter.potionCount != 0 && Input.GetKeyDown("1"))
        {
            UsePotion();
        }

    }
    //Gives the player health and removes the object
    public void UsePotion()
    {
                if(PlayerHealth.maxHealth <= PlayerHealth.health + 5)
                {
                    PlayerHealth.health = PlayerHealth.maxHealth;
                }
                else
                {
                    PlayerHealth.health = PlayerHealth.health + 10.0f;
                }
                counter.potionCount--;
      
    }
}
