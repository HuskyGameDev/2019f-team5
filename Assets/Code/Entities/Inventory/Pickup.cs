using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Entity
{
    private static GameObject rewardPopup;
    private Inventory inventory;
    public GameObject itemButton;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    private void Update()
    {
        if (rewardPopup == null)
            rewardPopup = Resources.Load<GameObject>("Prefabs/RewardPopup");
        // Move so that it works with the collision system,
        // even though it doesn't actually move.
        Move(Vector2.zero, -30f);
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    //Picks up item found on ground
    protected override void HandleOverlaps(List<CollideResult> overlaps)
    {
        for (int i = 0; i < overlaps.Count; ++i)
        {
            CollideResult result = overlaps[i];
            Entity target = result.entity;

            if (target != null && target is Player)
            {
                for (int j = 0; j < inventory.slot.Length; j++)
                {
                    if (inventory.full[j] == false)
                    {
                        GameObject points = Instantiate(rewardPopup, transform.position, Quaternion.identity);
                        points.transform.GetComponent<TextMesh>().text = "Item Picked Up!";
                        inventory.full[j] = true;
                        Instantiate(itemButton, inventory.slot[j].transform, false);
                        Destroy(gameObject);
                        break;
                    }

                }
            }
        }
    }

}
