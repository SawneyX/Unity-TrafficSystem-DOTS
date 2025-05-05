using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public int TileType;
    public int TileSubType;
    public float TileRotY;
    public int ParkingSpace;
    


    public TileData() {

        TileType = -1;
        TileSubType = -1;
        ParkingSpace = 0;
    }

    public void Reset() {

        TileType = -1;
        TileSubType = -1;
        TileRotY = 0;
        ParkingSpace = 0;
    }


}


