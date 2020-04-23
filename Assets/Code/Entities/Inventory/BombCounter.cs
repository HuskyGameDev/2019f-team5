using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombCounter : MonoBehaviour
{
    public int bombCount = 0;
    Text Bombs;
    // Start is called before the first frame update

    void Start()
    {
        Bombs = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
       Bombs.text = "" + bombCount;
    }
}

