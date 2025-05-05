using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileDataTrees
{
    public int TreeType;
    //public int TileSubType;
    public float TileRotY;

    public int color;


    public TileDataTrees()
    {

        TreeType = -1;
        TileRotY = 0;
        color = 0;
    }

    public void Reset()
    {

        TreeType = -1;
        //TileSubType = 0;
        TileRotY = 0;
        color = 0;
    }


}


