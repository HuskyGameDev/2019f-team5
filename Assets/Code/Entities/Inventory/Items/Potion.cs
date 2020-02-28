using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    
    private void Update()
    {
       // if (Input.GetKeyDown(KeyCode.) == true)
    }

    public void Use()
	{
		//Entity target = Player;
		//target.health = target.GetComponent<Player>().maxHealth;
		Destroy(gameObject);
	}
}
