using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Entity
{
    public GameObject Arrow;
    float fireRate;
    float nextFire;
    // Start is called before the first frame update
    void Start()
    {
        fireRate = 3f;
        nextFire = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        FireOrNot();
    }

    void FireOrNot(){
        if(Time.time > nextFire){
            Instantiate(Arrow, transform.position, Quaternion.identity);
            nextFire = Time.time + fireRate;
        }
    }
}
