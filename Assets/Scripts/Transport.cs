using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class Transport : MonoBehaviour
{

    public PointAndClick pointAndClick;

    Vector2 errorTile = new Vector2(-999, -999);

    Vector3 startPosition;
    Vector3 endPosition;

    public bool ready = false;

    CarSpawner carSpawner;
    Grid grid;

    private void Start()
    {
        carSpawner = GetComponent<CarSpawner>();
        grid = GetComponent<Grid>();
    }


    public void SetStart()
    {
        startPosition = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(pointAndClick.lastClickedEntity).Value;
        ready = true;
    }

    public void StartTransport(Vector3 endPosition)
    {
        this.endPosition = endPosition;
        ready = false;

        Vector2 start = grid.GetGridTileFromPosition(startPosition);

        Vector2 end = grid.GetGridTileFromPosition(endPosition);

        Vector2 startStreet = GetStreetTile(start);
        Vector2 endStreet = GetStreetTile(end);

        if (startStreet != errorTile && endStreet != errorTile) {


            CarData car = new CarData(0, 2.75f, 0, 0);
            car.targetX = (int)end.x;
            car.targetY = (int)end.y;

            carSpawner.SpawnCar(car, startStreet, endStreet);
        }




       

    }




    Vector2 GetStreetTile(Vector2 pos) {


        if (grid.GetTileType(pos + new Vector2(1, 0)) == 2) return pos + new Vector2(1, 0);
        else if (grid.GetTileType(pos + new Vector2(-1, 0)) == 2) return pos + new Vector2(-1, 0);
        else if (grid.GetTileType(pos + new Vector2(0, 1)) == 2) return pos + new Vector2(0, 1);
        else if (grid.GetTileType(pos + new Vector2(0, -1)) == 2) return pos + new Vector2(0, -1);
        else return errorTile;

    }

}
