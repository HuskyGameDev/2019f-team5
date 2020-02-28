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
        if (transform.childCount <= 0)
        {
            inventory.full[j] = false;
        }
    }
    public void DropItem()
    {
        foreach(Transform child in transform)
        {
            GameObject.Destroy(child.gameObject); 
        }
    }
}
