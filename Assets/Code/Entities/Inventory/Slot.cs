using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private Inventory inventory;
    public int j;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void Update()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

    }
    //Destroys the item after it's been used/dropped. It's in it's own script becuase of the way the pickup script is set up.
    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            for (int j = 0; j < inventory.slot.Length; j++)
            {
                if (inventory.full[j])
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        }
    }
}
