using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetGenerator : MonoBehaviour
{

    public int streetType = 0;

    Grid grid;
    Graph graph;
    CarSpawner carSpawner;
    Builder builder;

    public MeshFoundation foundation;
    public MeshStreetFlat flatStreet;
    public MeshStreetFlat flatStreetWide;
    public MeshStreetEnd endStreet;
    public MeshStreetCurve curveStreet;
    public MeshStreet3Cross cross3Street;
    public MeshStreet4Cross cross4Street;

    //public GameObject house;


    public GameObject hoverObject;

    Vector3 position;
    public float rotation;

    public Vector2 startPos;
    public Vector2 endPos;



    public bool loaded = false;
    
    public int tileCount = 0;

    public int direction = 0;


   

    // Start is called before the first frame update
    void Start()
    {

        grid = GetComponent<Grid>();
        graph = GetComponent<Graph>();
        carSpawner = GetComponent<CarSpawner>();
        builder = GetComponent<Builder>();
    }

   


    public void GenStreetLine(Vector3 pos1, Vector3 pos2) {

        Vector2 startPosGrid = grid.GetGridTileFromPosition(pos1);
        Vector2 endPosGrid = grid.GetGridTileFromPosition(pos2);

        startPos = startPosGrid;
        endPos = endPosGrid;
        
        if (startPosGrid.x < endPosGrid.x) direction = 1;
        if (startPosGrid.x > endPosGrid.x) direction = 3;
        if (startPosGrid.y < endPosGrid.y) direction = 0;
        if (startPosGrid.y > endPosGrid.y) direction = 2;

        tileCount = (int) Vector2.Distance(startPosGrid, endPosGrid) + 1;

       

        if (tileCount > 1 && tileCount <= 10) {

            
            if (grid.GetTileType(startPosGrid) == -1)
            {
                GenerateStreet(null, 1, pos1, direction * 90);
            }

            for (int i = 1; i < tileCount-1; i++)
            {

                //Vector2 nextPos = startPosGrid + (endPosGrid - startPosGrid) / tileCount;
                Vector3 nextPos = pos1 + i * (pos2 - pos1) / (tileCount-1);
        

                if (direction == 1 || direction == 3) {
                    GenerateStreet(null, 2, nextPos, 90);
                }
                else GenerateStreet(null, 2, nextPos, 0);

            }


            
            if ((grid.GetTileType(endPosGrid) == -1))
            {

                int directionTemp = direction * 90 + 180;
                if (directionTemp == 360) directionTemp = 0;
                else if (directionTemp == 450) directionTemp = 90;

                GenerateStreet(null, 1, pos2, directionTemp);
            }
            
            if ((grid.GetTileType(startPosGrid) != -1)) UpdateTile(startPosGrid); //Update Start
            else ReloadChunkMesh(startPosGrid);

            if ((grid.GetTileType(endPosGrid) != -1)) UpdateTile(endPosGrid); //Update End
            else ReloadChunkMesh(endPosGrid);




            //CheckConnection from Start Tile
            if ((grid.GetTileType(startPosGrid) != -1))
            {
                SearchAndUpdateNearestNodes(startPosGrid, startPosGrid, 0, -1, -1, false);
                CheckConnection(startPosGrid, startPosGrid, 0, -1, -1, true);

            }



            
            



            tileCount = 0;

        }



    }










    public void GenerateStreet(TileData tile, int streetType, Vector3 position, float rotation) {

        int subType = 0;

        if (tile != null) {

            streetType = tile.TileType;
            rotation = tile.TileRotY;

            subType = tile.TileSubType;

            if (streetType == -1) return;

        }
        Vector2 gridPosition = grid.GetGridTileFromPosition(position);

        grid.SetTile(gridPosition, streetType, rotation);  //Add it to SAVE-ARRAY   


        GameObject obj = new GameObject();
        obj.transform.position = new Vector3(position.x, 0.01f, position.z); //new Vector3(position.x + 0.25f, 0.01f, position.z + 0.25f);
        obj.transform.Rotate(0, rotation, 0);
        //MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Shader Graphs/RoadShader")); //"Universal Render Pipeline/Lit"
        MeshFilter objmeshFilter = obj.AddComponent<MeshFilter>();

        

        switch (streetType) {

            //Foundation
            case 0:

                objmeshFilter.mesh = foundation.GenerateMesh(obj);

                AddMeshToChunk(obj);
                return;

            //EndStreet
            case 1:

                objmeshFilter.mesh = endStreet.GenerateMesh(obj);

                AddMeshToChunk(obj);
                return;
            //FlatStreet
            case 2: 
      
                objmeshFilter.mesh = flatStreet.GenerateMesh(obj);

                AddMeshToChunk(obj);
                return;

            //Curve Street
            case 3:

                if (subType == 1) objmeshFilter.mesh = curveStreet.GenerateMeshWithFoundation(obj); //Foundation

                else objmeshFilter.mesh = curveStreet.GenerateMesh(obj);

                AddMeshToChunk(obj);
                return;

            //3Cross
            case 4:

                //if (loaded) CheckConnection(gridPosition, gridPosition, 0, -1);

                objmeshFilter.mesh = cross3Street.GenerateMesh(obj);

                AddMeshToChunk(obj);
                return;

            //4Cross
            case 5:

                //if (loaded) CheckConnection(gridPosition, gridPosition, 0, -1);

                objmeshFilter.mesh = cross4Street.GenerateMesh(obj);

                AddMeshToChunk(obj);
                return;

            //FlatStreetWide
            case 6:

                objmeshFilter.mesh = flatStreetWide.GenerateMesh(obj);

                AddMeshToChunk(obj);
                return;






            default: return;
        }
        

    }


    

    public void UpdateTile(Vector2 gridPosition) {

        TileData oldTile = grid.GetTile(gridPosition);

        TileData newTile = new TileData();

        Vector4 facingPos = new Vector4(0, 0, 0, 0);


        if (grid.FacingRight(gridPosition)) facingPos.x = 1;
        if (grid.FacingLeft(gridPosition)) facingPos.y = 1;
        if (grid.FacingTop(gridPosition)) facingPos.z = 1;
        if (grid.FacingDown(gridPosition)) facingPos.w = 1;

        int amountFacing = (int)(facingPos.x + facingPos.y + facingPos.z + facingPos.w);



        if (amountFacing == 0) {

            newTile = null;
            grid.DeleteTile(gridPosition);
            ReloadChunkMesh(gridPosition);
            return;
        }

        //End Street
        if (amountFacing == 1) {

            if (facingPos == new Vector4(1, 0, 0, 0)) {

                newTile.TileType = 1;
                newTile.TileRotY = 90;
            }
            else if (facingPos == new Vector4(0, 1, 0, 0))
            {

                newTile.TileType = 1;
                newTile.TileRotY = 270;
            }
            else if (facingPos == new Vector4(0, 0, 1, 0))
            {

                newTile.TileType = 1;
                newTile.TileRotY = 0;
            }
            else if (facingPos == new Vector4(0, 0, 0, 1))
            {

                newTile.TileType = 1;
                newTile.TileRotY = 180;
            }

        }

        else if (facingPos == new Vector4(1, 1, 0, 0) || facingPos == new Vector4(1, 0, 0, 0) || facingPos == new Vector4(0, 1, 0, 0)) //Flat Right/Left
        {
            newTile.TileType = 2;
            newTile.TileRotY = 90;
        }
        else if (facingPos == new Vector4(0, 0, 1, 1) || facingPos == new Vector4(0, 0, 1, 0) || facingPos == new Vector4(0, 0, 0, 1)) //Flat Top/Down
        {
            newTile.TileType = 2;
            newTile.TileRotY = 0;
        }

        else if (facingPos == new Vector4(1, 0, 1, 0)) //Curve Top Right
        {
            newTile.TileType = 3;
            newTile.TileRotY = 180;
        }
        else if (facingPos == new Vector4(1, 0, 0, 1)) //Curve Down Right
        {
            newTile.TileType = 3;
            newTile.TileRotY = 270;
        }
        else if (facingPos == new Vector4(0, 1, 1, 0)) //Curve Top Left
        {
            newTile.TileType = 3;
            newTile.TileRotY = 90;
        }
        else if (facingPos == new Vector4(0, 1, 0, 1)) //Curve Down Left
        {
            newTile.TileType = 3;
            newTile.TileRotY = 0;
        }

        else if (facingPos == new Vector4(0, 1, 1, 1)) //3 Cross Left
        {
            newTile.TileType = 4;
            newTile.TileRotY = 0;
        }
        else if (facingPos == new Vector4(1, 0, 1, 1)) //3 Cross Right
        {
            newTile.TileType = 4;
            newTile.TileRotY = 180;
        }
        else if (facingPos == new Vector4(1, 1, 1, 0)) //3 Cross Up
        {
            newTile.TileType = 4;
            newTile.TileRotY = 90;
        }
        else if (facingPos == new Vector4(1, 1, 0, 1)) //3 Cross Down
        {
            newTile.TileType = 4;
            newTile.TileRotY = 270;
        }
        else if (facingPos == new Vector4(1, 1, 1, 1)) //4 Cross Down
        {
            newTile.TileType = 5;
            newTile.TileRotY = 0;
        }

        if (facingPos.x + facingPos.y + facingPos.z + facingPos.w >= 3) CheckConnection(gridPosition, gridPosition, 0, -1, -1, true);

        else if (amountFacing > 0)
        {
            SearchAndUpdateNearestNodes(gridPosition, gridPosition, 0, -1, -1, true);
            
        }


        if (newTile.TileType == 3 && oldTile.TileType == 3 && oldTile.TileSubType == 1) newTile.TileSubType = 1;   //Check Foundation for curve
        


        GenerateStreet(newTile, 0, grid.WorldPositionFromGridTile(gridPosition), 0);


        ReloadChunkMesh(gridPosition);
        /*
        if (oldTile != null && oldTile.TileType != -1)
        {

            
        }
        */
    }


    public void UpdateSubType(Vector2 gridPosition, int subType) {

        

        grid.tileDatas[(int)gridPosition.x, (int)gridPosition.y].TileSubType = subType;

        //GenerateStreet(newTile, 0, grid.WorldPositionFromGridTile(gridPosition), 0);


        ReloadChunkMesh(gridPosition);


    }



    public void AddMeshToChunk(GameObject obj)
    {

        Vector2 gridPosition = grid.GetGridTileFromPosition(new Vector3(obj.transform.position.x, 0, obj.transform.position.z));

        GameObject chunk = grid.NewChunk(grid.GetChunkFromPosition(gridPosition)); //Get existing or make new Chunk

        AddChunkMesh(chunk, obj);    
    }


    private void AddChunkMesh(GameObject chunk, GameObject obj) {

        List<GameObject> objects = new List<GameObject>();
        objects.Add(chunk);
        objects.Add(obj);

        

        MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
        meshFilter.mesh.MarkDynamic();

        Mesh combinedMesh = CombineMeshes.Combine(objects); //Combines Meshes

        meshFilter.mesh = combinedMesh;
        
        chunk.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Shader Graphs/RoadShader"));

        DestroyImmediate(obj);

    }



    


    public void ReloadChunkMesh(Vector2 tilePos) {   //Update Whole Chunk that tile lies in

        //Debug.Log("Reload " + grid.GetChunkFromPosition(tilePos) + "startPos: " + tilePos);


        
        StartCoroutine("BuildChunkMesh", grid.GetChunkFromPosition(tilePos));

    }


    private IEnumerator BuildChunkMesh(Vector2 chunkPos)
    {

        //Vector2 chunkPos = grid.GetChunkFromPosition(tilePos);

        Vector2 firstTile = grid.GetFirstWorldPositionFromChunk(chunkPos);


        GameObject chunk = grid.GetChunk(chunkPos); //Get existing Chunk

        //if (chunk == null) yield return new WaitForEndOfFrame();


        grid.DeleteChunk(chunkPos);

        grid.NewChunk(chunkPos);

        for (int i = (int) firstTile.x; i < firstTile.x + grid.chunkSize + 1; i++)
        {

            for (int j = (int)firstTile.y; j < firstTile.y + grid.chunkSize + 1; j++)
            {


                if (grid.tileDatas[i, j] != null || grid.GetTileType(new Vector2(i, j)) != -1)
                {

                    GenerateStreet(grid.tileDatas[i, j], 0, grid.WorldPositionFromGridTile(new Vector2(i, j)), 0);

                    if (grid.GetTileType(new Vector2(i, j)) == 2 && grid.tileDatas[i, j].TileSubType != -1) GenMisc(new Vector2(i, j));



                    if (j % 10 == 0) yield return null;
                    //yield return null;

                }
                if (i == firstTile.x + grid.chunkSize / 2 && j == firstTile.y + grid.chunkSize / 2)
                {

                    //yield return null;

                }

            }
            //yield return null;
        }




        if (chunk != null)
        {
            chunk.GetComponent<MeshFilter>().mesh.Clear();
            Destroy(chunk);
        }

        yield return new WaitForEndOfFrame();
        
    }



    public IEnumerator BuildWholeWorldMesh() {

        if (grid.tileDatas.GetLength(0) == 0) yield return new WaitForEndOfFrame();

        for (int i = 0; i < grid.totalTilesX; i++)
        {

            for (int j = 0; j < grid.totalTilesY; j++)
            {


                if (grid.tileDatas[i, j] != null || grid.GetTileType(new Vector2(i, j)) != -1)
                {

                    GenerateStreet(grid.tileDatas[i, j], 0, grid.WorldPositionFromGridTile(new Vector2(i, j)), 0);

                    Vector2 pos = new Vector2(i, j);

                    if (grid.GetTileType(pos) == 4 || grid.GetTileType(pos) == 5) { //When crossing

                        graph.NewNode(pos);
                        CheckConnection(pos, pos, 0, -1, -1, true);

                    }

                    if (grid.GetTileType(pos) == 2 && grid.GetTileSubType(pos) != -1) {  // Add Misc to Mesh

                        //grid.tileDatas[(int)pos.x, (int)pos.y].TileSubType  = -1;
                        GenMisc(pos);
                        

                    }

                    


                    yield return null;

                }

            }
        }

        
        loaded = true;
        if (graph.debug) graph.DrawConnectionGraph();

        yield return new WaitForEndOfFrame();
        

     }



    public void GenMisc(Vector2 pos) {

        float miscRot = 0;

        int miscType = -1;

        if (grid.GetTileSubType(pos) == 0) {
            miscRot = 0;
            miscType = 0;
        }
        if (grid.GetTileSubType(pos) == 1)
        {
            miscRot = 90;
            miscType = 0;
        }
        if (grid.GetTileSubType(pos) == 2)
        {
            miscRot = 180;
            miscType = 0;
        }
        if (grid.GetTileSubType(pos) == 3)
        {
            miscRot = 270;
            miscType = 0;
        }
        

        Vector3 position = grid.WorldPositionFromGridTile(pos);

        GameObject obj = Instantiate(builder.Misc[miscType], position, builder.Misc[miscType].transform.rotation * Quaternion.Euler(0, 0, miscRot));

        

      
        AddMeshToChunk(obj);
        
    }





    public void OnDeleteTile(Vector2 position) {

        

        int typetile = grid.GetTileType(position);
        int subtypetile = grid.GetTileSubType(position);

        //Delete Subtype
        if (subtypetile != -1) {

            grid.tileDatas[(int)position.x, (int)position.y].TileSubType = -1;
            ReloadChunkMesh(position);
            return;
        }



        if (typetile == -1) return;

        if (typetile == 4 || typetile == 5)
        {

            graph.DeleteNode(position);

            grid.DeleteTile(position);

            SearchAndUpdateNearestNodes(position, position, 0, -1, -1, true);

        }

        else if (typetile == 1)
        {

            UpdateTile(position);
            grid.DeleteTile(position);
        }

        else if (typetile == 0) {

            grid.DeleteTile(position);
            UpdateTile(position);
            return;
        }


        else grid.DeleteTile(position);

        if (grid.FacingRight(position)) {
            
            int type = grid.GetTileType(position + new Vector2(1, 0));         
            UpdateTile(position + new Vector2(1, 0));
            if (type == 4 || type == 5) if (CheckConnection(position + new Vector2(1, 0), position + new Vector2(1, 0), 0, -1, -1, true) == false) graph.DeleteNode(position + new Vector2(1, 0));
        }
        if (grid.FacingLeft(position))
        {

            int type = grid.GetTileType(position + new Vector2(-1, 0));
            
            UpdateTile(position + new Vector2(-1, 0));
            if (type == 4 || type == 5) if (CheckConnection(position + new Vector2(-1, 0), position + new Vector2(-1, 0), 0, -1, -1, true) == false) graph.DeleteNode(position + new Vector2(-1, 0));
        }
        if (grid.FacingTop(position))
        {

            int type = grid.GetTileType(position + new Vector2(0, 1));
            
            UpdateTile(position + new Vector2(0, 1));
            if (type == 4 || type == 5) if (CheckConnection(position + new Vector2(0, 1), position + new Vector2(0, 1), 0, -1, -1, true) == false) graph.DeleteNode(position + new Vector2(0, 1));
        }
        if (grid.FacingDown(position))
        {

            int type = grid.GetTileType(position + new Vector2(0, -1));
            
            UpdateTile(position + new Vector2(0, -1));
            if (type == 4 || type == 5) if (CheckConnection(position + new Vector2(0, -1), position + new Vector2(0, -1), 0, -1, -1, true) == false) graph.DeleteNode(position + new Vector2(0, -1));
        }

        


        if (graph.debug) graph.DrawConnectionGraph();

        return;
    }




    bool CheckConnection(Vector2 startPosition, Vector2 gridPosition, int length, int pathStartDirection, int maskDirection, bool repeat) {

        //if (pathStartDirection == -1)Debug.Log("check");

        if (graph.NodeExist(startPosition) && length == 0) {

            graph.GetNode(startPosition).DeleteAllConnections();
        }

        if (grid.GetTileType(gridPosition) == -1) return false;


        Vector4 facingPos = new Vector4(0, 0, 0, 0);

        if (grid.FacingRight(gridPosition)) facingPos.x = 1; //dir = 0
        if (grid.FacingLeft(gridPosition)) facingPos.y = 1; //dir = 1
        if (grid.FacingTop(gridPosition)) facingPos.z = 1; //dir = 2
        if (grid.FacingDown(gridPosition)) facingPos.w = 1; //dir = 3

        //No checking for connections if tile not a crossing
        if (startPosition == gridPosition && (facingPos.x + facingPos.y + facingPos.z + facingPos.w) < 3) return false;
        

        //Roundabout (Same start & ending)
        
        if (startPosition == gridPosition && length > 0) return false;
        

       

        //Normal path between 2 crossings
        if (gridPosition != startPosition && (facingPos.x + facingPos.y + facingPos.z + facingPos.w) >= 3) {

            Connection connection = new Connection(null, pathStartDirection, length, 0);

            graph.AddConnectionToNode(startPosition, gridPosition, connection, 0);

            if (repeat == true) {
                if (CheckConnection(gridPosition, gridPosition, 0, -1, -1, false) == false) {
                    graph.DeleteNode(gridPosition);
                }
            }

            
            return true;
        }

        //Street Finished
        if (facingPos.x + facingPos.y + facingPos.z + facingPos.w == 0) {

            return false;
        }

        bool found = false;

        if (facingPos.x == 1 && maskDirection != 0) {

            if (length == 0) pathStartDirection = 0; 

            found = CheckConnection(startPosition, gridPosition + new Vector2(1, 0), length + 1, pathStartDirection, 1, repeat) || found;
        }
        if (facingPos.y == 1 && maskDirection != 1)
        {
            if (length == 0) pathStartDirection = 1;

            found = CheckConnection(startPosition, gridPosition + new Vector2(-1, 0), length + 1, pathStartDirection, 0, repeat) || found;
        }
        if (facingPos.z == 1 && maskDirection != 2)
        {
            if (length == 0) pathStartDirection = 2;

            found = CheckConnection(startPosition, gridPosition + new Vector2(0, 1), length + 1, pathStartDirection, 3, repeat) || found;
        }
        if (facingPos.w == 1 && maskDirection != 3)
        {
            if (length == 0) pathStartDirection = 3;

            found = CheckConnection(startPosition, gridPosition + new Vector2(0, -1), length + 1, pathStartDirection, 2, repeat) || found;
        }

        return found;

    }



    public void SearchAndUpdateNearestNodes(Vector2 startPosition, Vector2 gridPosition, int length, int pathStartDirection, int maskDirection, bool repeat)
    {

        

        Vector4 facingPos = new Vector4(0, 0, 0, 0);

        if (grid.FacingRight(gridPosition)) facingPos.x = 1; //dir = 0
        if (grid.FacingLeft(gridPosition)) facingPos.y = 1; //dir = 1
        if (grid.FacingTop(gridPosition)) facingPos.z = 1; //dir = 2
        if (grid.FacingDown(gridPosition)) facingPos.w = 1; //dir = 3


        //Roundabout (Same start & ending)
        
        if (startPosition == gridPosition && length > 0)
        {
            return;
        }
        

        //Normal path to crossing
        if (gridPosition != startPosition && (facingPos.x + facingPos.y + facingPos.z + facingPos.w) >= 3)
        {

           
            graph.NewNode(gridPosition);
            CheckConnection(gridPosition, gridPosition, 0, -1, -1, repeat);

                
                   
            return;
        }

        //Street Finished
        if (facingPos.x + facingPos.y + facingPos.z + facingPos.w == 0)
        {
            return;
        }


        if (facingPos.x == 1 && maskDirection != 0)
        {

            if (length == 0) pathStartDirection = 0;

            SearchAndUpdateNearestNodes(startPosition, gridPosition + new Vector2(1, 0), length + 1, pathStartDirection, 1, repeat);
        }
        if (facingPos.y == 1 && maskDirection != 1)
        {
            if (length == 0) pathStartDirection = 1;

            SearchAndUpdateNearestNodes(startPosition, gridPosition + new Vector2(-1, 0), length + 1, pathStartDirection, 0, repeat);
        }
        if (facingPos.z == 1 && maskDirection != 2)
        {
            if (length == 0) pathStartDirection = 2;

            SearchAndUpdateNearestNodes(startPosition, gridPosition + new Vector2(0, 1), length + 1, pathStartDirection, 3, repeat);
        }
        if (facingPos.w == 1 && maskDirection != 3)
        {
            if (length == 0) pathStartDirection = 3;

            SearchAndUpdateNearestNodes(startPosition, gridPosition + new Vector2(0, -1), length + 1, pathStartDirection, 2, repeat);
        }

        return;

    }


    public void MakeNodeAndConnectionToNearestNodes(Vector2 startPosition, Vector2 gridPosition, int length, int pathStartDirection, int maskDirection, int startOrEndNode) //startOrEndNode 1 = start, 2 = end
    {



        Vector4 facingPos = new Vector4(0, 0, 0, 0);

        if (grid.FacingRight(gridPosition)) facingPos.x = 1; //dir = 0
        if (grid.FacingLeft(gridPosition)) facingPos.y = 1; //dir = 1
        if (grid.FacingTop(gridPosition)) facingPos.z = 1; //dir = 2
        if (grid.FacingDown(gridPosition)) facingPos.w = 1; //dir = 3


        //Clicking already on crossing
        if (startPosition == gridPosition && (facingPos.x + facingPos.y + facingPos.z + facingPos.w) >= 3 && length == 0)
        {
            return;
            //return graph.NewNode(startPosition);

        }


        //Roundabout (Same start & ending)

        if (startPosition == gridPosition && length > 0)
        {
            return;
        }


        //Normal path to crossing
        if (gridPosition != startPosition && (facingPos.x + facingPos.y + facingPos.z + facingPos.w) >= 3)
        {

           
            if (startOrEndNode == 2)
            {
                pathStartDirection = maskDirection; //When End node: take last direction
            }

            Connection connection = new Connection(null, pathStartDirection, length, 0);

            Node node = graph.NewNode(startPosition);
            node.pathStartDirection = pathStartDirection;

            
            

            if (startOrEndNode == 2) {  //Change start/end

                if (node.endConnectionNode == null)
                {
                    node.endConnectionNode = new List<Node>();
                }
                node.endConnectionNode.Add(graph.NewNode(gridPosition)); //Add endConnection Node

                Vector2 temp = startPosition;
                
                startPosition = gridPosition;
                gridPosition = temp;

            }

            graph.AddConnectionToNode(startPosition, gridPosition, connection, startOrEndNode); //startNode == 1, endNode == 2




            return;
        }

        //Street Finished
        if (facingPos.x + facingPos.y + facingPos.z + facingPos.w == 0)
        {
            return;
        }

        

        if (facingPos.x == 1 && maskDirection != 0)  
        {

            if (length == 0) pathStartDirection = 0;
       
            MakeNodeAndConnectionToNearestNodes(startPosition, gridPosition + new Vector2(1, 0), length + 1, pathStartDirection, 1, startOrEndNode);
        }
        if (facingPos.y == 1 && maskDirection != 1)
        {
            if (length == 0) pathStartDirection = 1;

            MakeNodeAndConnectionToNearestNodes(startPosition, gridPosition + new Vector2(-1, 0), length + 1, pathStartDirection, 0, startOrEndNode);
        }
        if (facingPos.z == 1 && maskDirection != 2)
        {
            if (length == 0) pathStartDirection = 2;

            MakeNodeAndConnectionToNearestNodes(startPosition, gridPosition + new Vector2(0, 1), length + 1, pathStartDirection, 3, startOrEndNode);
        }
        if (facingPos.w == 1 && maskDirection != 3)
        {
            if (length == 0) pathStartDirection = 3;

            MakeNodeAndConnectionToNearestNodes(startPosition, gridPosition + new Vector2(0, -1), length + 1, pathStartDirection, 2, startOrEndNode);
        }

        return;

    }

    public bool CheckConnectionToOtherTile(Vector2 startPosition, Vector2 gridPosition, int length, int pathStartDirection, int maskDirection, Vector2 otherTile) //startOrEndNode 1 = start, 2 = end
    {



        Vector4 facingPos = new Vector4(0, 0, 0, 0);

        if (grid.FacingRight(gridPosition)) facingPos.x = 1; //dir = 0
        if (grid.FacingLeft(gridPosition)) facingPos.y = 1; //dir = 1
        if (grid.FacingTop(gridPosition)) facingPos.z = 1; //dir = 2
        if (grid.FacingDown(gridPosition)) facingPos.w = 1; //dir = 3


        //Clicking already on crossing
        if (startPosition == gridPosition && (facingPos.x + facingPos.y + facingPos.z + facingPos.w) >= 3 && length == 0)
        {
            return false;
            //return graph.NewNode(startPosition);

        }


        //Roundabout (Same start & ending)

        if (startPosition == gridPosition && length > 0)
        {
            return false;
        }


        //Normal path to crossing
        if (gridPosition != startPosition && (facingPos.x + facingPos.y + facingPos.z + facingPos.w) >= 3)
        {
            return false;

        }

        //Street Finished
        if (facingPos.x + facingPos.y + facingPos.z + facingPos.w == 0)
        {
            return false;
        }

        //Other Tile
        if (gridPosition == otherTile && length >= 0) {


            Connection connection = new Connection(null, pathStartDirection, length, 0);

            Node node = graph.NewNode(startPosition);
            node.pathStartDirection = pathStartDirection;

            graph.NewNode(gridPosition);

            graph.AddConnectionToNode(startPosition, gridPosition, connection, 0); //startNode == 1, endNode == 2

            return true;
        }


       bool found = false;

        if (facingPos.x == 1 && maskDirection != 0 && found == false)
        {

            if (length == 0) pathStartDirection = 0;

            found = CheckConnectionToOtherTile(startPosition, gridPosition + new Vector2(1, 0), length + 1, pathStartDirection, 1, otherTile);
        }
        if (facingPos.y == 1 && maskDirection != 1 && found == false)
        {
            if (length == 0) pathStartDirection = 1;

            found = CheckConnectionToOtherTile(startPosition, gridPosition + new Vector2(-1, 0), length + 1, pathStartDirection, 0, otherTile);
        }
        if (facingPos.z == 1 && maskDirection != 2 && found == false)
        {
            if (length == 0) pathStartDirection = 2;

            found = CheckConnectionToOtherTile(startPosition, gridPosition + new Vector2(0, 1), length + 1, pathStartDirection, 3, otherTile);
        }
        if (facingPos.w == 1 && maskDirection != 3 && found == false)
        {
            if (length == 0) pathStartDirection = 3;

            found = CheckConnectionToOtherTile(startPosition, gridPosition + new Vector2(0, -1), length + 1, pathStartDirection, 2, otherTile);
        }

        return found;

    }





}
