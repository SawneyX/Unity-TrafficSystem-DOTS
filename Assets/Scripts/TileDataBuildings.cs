using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileDataBuildings
{
    public int BuildingType;
    public int TileSubType;
    public float TileRotY;
    //public float TileX;
    //public float TileY;


    public TileDataBuildings()
    {

        BuildingType = -1;
    }

    public void Reset()
    {

        BuildingType = -1;
        TileSubType = 0;
        TileRotY = 0;
    }


}


