using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarData
{
    public int type;
    public float speed;
    public int good;
    public int amount;

    public int targetX;
    public int targetY;



    public CarData(int type, float speed, int good, int amount)
    {

        this.type = type;
        this.speed = speed;
        this.good = good;
        this.amount = amount;
       
    }

   


}


