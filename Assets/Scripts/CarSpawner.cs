using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    StreetGenerator streetGen;
    Graph graph;
    Grid grid;
    GenTilePath pathBuilder;


    public UnitGameObject carEntityHolder;

    public GameObject car;


    Node startNode = null;
    Node endNode = null;

    float spawnTimer = 2f;

    // Start is called before the first frame update
    void Start()
    {
        graph = GetComponent<Graph>();
        grid = GetComponent<Grid>();
        streetGen = GetComponent<StreetGenerator>();
        pathBuilder = GetComponent<GenTilePath>();
    }

    // Update is called once per frame
    void Update()
    {


        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0) {


            Vector2 start = new Vector2(0, 0);
            Vector2 end = new Vector2(0, 0); ;
            Vector2 errorVector = new Vector2(-999, -999);

            while (start != errorVector && (start == new Vector2(0, 0) || end == new Vector2(0, 0))) {

                start = grid.GetRandomSpawnTile();
                end = grid.GetRandomSpawnTile();
            }
            if (start != errorVector && start != end) {

                CarData car = new CarData(0, 2.75f, -1, 0);

                SpawnCar(car, start, end);
            }

            spawnTimer = 2f;


        }


    }


    public void SpawnCar(CarData carData, Vector2 startTile, Vector2 endTile) {

        

        graph.DeleteNodeData();
        graph.DeleteTempNodes(startNode, endNode);

        if (startTile == null || endTile == null) return;
        if (startTile == endTile) return;
        

        if (grid.GetTileType(startTile) == -1 || grid.GetTileType(endTile) == -1 || grid.GetTileType(startTile) == 4 || grid.GetTileType(endTile) == 4 || grid.GetTileType(startTile) == 5 || grid.GetTileType(endTile) == 5)
        {
            print("bad start or end");
            return;
        }

        //Check if on same connection or not
        if (streetGen.CheckConnectionToOtherTile(startTile, startTile, 0, -1, -1, endTile))
        {

            streetGen.CheckConnectionToOtherTile(startTile, startTile, 0, -1, -1, endTile);

            startNode = graph.GetNode(startTile);
            endNode = graph.GetNode(endTile);
        }
        else //Not on same connection
        {
            graph.DeleteTempNodes(startNode, endNode);

            streetGen.MakeNodeAndConnectionToNearestNodes(startTile, startTile, 0, -1, -1, 1);  //Last = 1 = StartNode
            startNode = graph.GetNode(startTile);
            streetGen.MakeNodeAndConnectionToNearestNodes(endTile, endTile, 0, -1, -1, 2);  //Last = 2 = EndNode
            endNode = graph.GetNode(endTile);

        }

        

        if (startNode == null || endNode == null) {
            if (startNode == null) Debug.Log("No start Node found");
            if (endNode == null) Debug.Log("No end Node found");

            graph.DeleteTempNodes(startNode, endNode);
            return;
        }



        //bool right = false;


        int startDir = startNode.pathStartDirection;

        if (startDir == 0 || startDir == 2)
        {

            //right = true;
        }
        else if (startDir == -1) Debug.Log("start Error");


        

        //GameObject newCar = Instantiate(car, grid.WorldPositionFromGridTile(startTile), Quaternion.identity);
        List<Vector3> list = Pathfind(carData, startNode, endNode);


        //ECS
        if (list.Count > 0) carEntityHolder.ECSSpawnCar(carData, list); //SPAWN


        
       
        return;
    }




    public List<Vector3> Pathfind(CarData carData, Node startNode, Node endNode)
    {

        LinkedList<Node> nodePath = new LinkedList<Node>();
        
        nodePath = graph.FindPath(startNode, endNode);

        

        //Visualize
        /*
        foreach (Node node in nodePath)
        {

            //Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), grid.WorldPositionFromGridTile(node.GetPosition()), Quaternion.identity);

        }
        */

        List<Vector3> tilePath = new List<Vector3>(); ;
        
        tilePath = pathBuilder.GeneratePath(carData, nodePath);


        graph.DeleteTempNodes(startNode, endNode);

        return tilePath;
    }

}
