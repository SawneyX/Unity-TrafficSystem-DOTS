using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //public TileData[,] tiles2D;

    public TileData[] tiles1D;

    public Node[] connections;

   

    public GameData(TileData[,] allTiles) {

       

        int width = allTiles.GetLength(0);
        int height = allTiles.GetLength(1);
        tiles1D = new TileData[height * width];
        


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                TileData tile = allTiles[j, i];

                //if (tile.TileType == -1) tile = null; //SAVE SPACE IN JSON

                tiles1D[i * height + j] = tile;

            }
        }

    }
    
}


