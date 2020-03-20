using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private float scale;
    public Player player;
    public GameObject bar;
    private float position;
    private float width;
    private float height;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rt = bar.GetComponent<RectTransform>();
        width = rt.rect.width;
        height = rt.rect.height;
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, width);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, height);
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform rt = bar.GetComponent<RectTransform>();
        scale = player.health / player.maxHealth; 
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, width*scale);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, height);
    }
}
