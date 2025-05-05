using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{

    public int population;
    public int energyProduction;
    public int energyConsumption;

    public int streetTiles;

    Grid grid;

    public TMPro.TMP_Text populationText;
    public TMPro.TMP_Text energyText;

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();
    }



    public void UpdateStats() {


        //StreetTiles
        streetTiles = 0;

        for (int i = 0; i < grid.totalTilesX; i++)
        {
            for (int j = 0; j < grid.totalTilesY; j++)
            {

                if (grid.tileDatas[i, j] != null && grid.GetTileType(new Vector2(i, j)) != -1)
                {

                    ++streetTiles;
                }
            }
        }


        //Population

        population = 0;

        foreach (TileDataBuildings data in grid.buildingDatas.Values) {

            ++population; //TODO: Actual amount of people
        }

        //Energy

        energyProduction = 300; //TODO
        energyConsumption = population; //TODO





        populationText.text = "Population: " + population;

        energyText.text = "Energy: " + energyConsumption + "/" + energyProduction;
    }
}
