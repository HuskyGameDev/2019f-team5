using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionCounter : MonoBehaviour
{
    public int potionCount = 0;
    Text Potions;
    // Start is called before the first frame update

    void Start()
    {
        Potions = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Potions.text = "" + potionCount;
    }
}
