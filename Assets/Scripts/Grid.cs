using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Grid : MonoBehaviour
{

    //float gridX = 0.5f;
    //float gridY = 0.5f;
    //float gridZ = 0.5f;

    public float tileSizeX = 3;
    public float tileSizeY = 3;

    public int chunkSize = 12;

    public int totalTilesX = 101;
    public int totalTilesY = 101;

    public Vector3 position = Vector2.zero;

    public Vector3 mousePosition = Vector3.zero;

    //public Vector2 gridPosition = Vector2.zero;

    public Vector2 chunk = Vector2.zero;

    public Vector3 firstPosChunk;

    float maxPlaceDistance = 100;

    public GameObject hoverObject;
    StreetGenerator streetGen;
    Graph graph;
    Builder builder;

    public TileData[,] tileDatas;

    public Dictionary<Vector2, GameObject> chunks = new Dictionary<Vector2, GameObject>();

    public Dictionary<Vector3, TileDataBuildings> buildingDatas = new Dictionary<Vector3, TileDataBuildings>();
    public Dictionary<Vector3, TileDataTrees> treeDatas = new Dictionary<Vector3, TileDataTrees>();

    public GameObject worldParent;


    SaveLoadData saveLoadData;

    //public int[,] array = new int[4, 2] { { 0, 5 }, { 8, 2 }, { 4, 6 }, { 4, 6 } };   //4 Rows à 2 Columns

    // Start is called before the first frame update
    void Start()
    {
        streetGen = GetComponent<StreetGenerator>();
        graph = GetComponent<Graph>();
        saveLoadData = new SaveLoadData();
        builder = GetComponent<Builder>();

        LoadGameData();

    }




    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, maxPlaceDistance))
        {

            hoverObject.transform.position = RoundTransform(hit.point, tileSizeX, tileSizeY);

            mousePosition = hit.point;
            
            /*
            //GET VERTEX COLOR
            Vector2 chunkPos = GetChunkFromPosition(GetGridTileFromPosition(hoverObject.transform.position));
            GameObject chunk = GetChunk(chunkPos);

            if (chunk != null) {

                Mesh streetMesh = chunk.GetComponent<MeshFilter>().mesh;

                var vertIndex = streetMesh.triangles[hit.triangleIndex * 3 + 0];

                print(streetMesh.colors[vertIndex]);
            }

            */

            


        }

        position.x = RoundTransform(hoverObject.transform.position, tileSizeX, tileSizeY).x / tileSizeX + totalTilesX / 2; //+++
        position.y = RoundTransform(hoverObject.transform.position, tileSizeX, tileSizeY).z / tileSizeY + totalTilesY / 2; //+++
        position.z = hoverObject.transform.position.y - 0.25f;

        chunk.x = RoundTransform(hoverObject.transform.position, tileSizeX * chunkSize, tileSizeY * chunkSize).x / (tileSizeX * chunkSize);
        chunk.y = RoundTransform(hoverObject.transform.position, tileSizeX * chunkSize, tileSizeY * chunkSize).z / (tileSizeY * chunkSize);


        //gridPosition = GetGridTileFromPosition(hoverObject.transform.position);

        firstPosChunk = GetFirstWorldPositionFromChunk(chunk);



        if (Input.GetKeyDown(KeyCode.L))
        {

            saveLoadData.SaveGameData(tileDatas, buildingDatas, treeDatas);

        }

    }


    void LoadGameData()
    {

        //LOAD TILES
        tileDatas = saveLoadData.LoadTileData(totalTilesX, totalTilesY);

        buildingDatas = saveLoadData.LoadBuildings();

        treeDatas = saveLoadData.LoadTrees();

        streetGen.StartCoroutine("BuildWholeWorldMesh");

        builder.StartCoroutine(builder.InstantiateAllBuildings(buildingDatas));

        builder.StartCoroutine(builder.InstantiateAllTrees(treeDatas));



    }

    public TileData GetTile(Vector2 pos) {

        int positionX = (int)position.x;
        int positionY = (int)position.y;

        return tileDatas[positionX, positionY];
    }

    public void SetTile(Vector2 position, int type, float rotation)
    {
        //Vector2 position = GetVectorInArray(position1);

        int positionX = (int)position.x;
        int positionY = (int)position.y;

        if (tileDatas[positionX, positionY] == null) tileDatas[positionX, positionY] = new TileData();


        tileDatas[positionX, positionY].TileType = type;
        tileDatas[positionX, positionY].TileRotY = rotation;
    }
    public void SetTileFromTileData(TileData tile, Vector2 position)
    {
        //Vector2 position = GetVectorInArray(position1);

        int positionX = (int)position.x;
        int positionY = (int)position.y;

        if (tileDatas[positionX, positionY] == null) tileDatas[positionX, positionY] = new TileData();


        tileDatas[positionX, positionY] = tile;


    }
    public int GetTileType(Vector2 position)
    {
        //Vector2 position = GetVectorInArray(position1);

        int positionX = (int)position.x;
        int positionY = (int)position.y;



        if (tileDatas[positionX, positionY] == null) return -1;

        return tileDatas[positionX, positionY].TileType;
    }
    public int GetTileSubType(Vector2 position)
    {
        //Vector2 position = GetVectorInArray(position1);

        int positionX = (int)position.x;
        int positionY = (int)position.y;



        if (tileDatas[positionX, positionY] == null) return -1;

        return tileDatas[positionX, positionY].TileSubType;
    }
    public float GetTileRotation(Vector2 position)
    {

        //Vector2 position = GetVectorInArray(position1);

        int positionX = (int)position.x;
        int positionY = (int)position.y;

        return tileDatas[positionX, positionY].TileRotY;
    }

    public void DeleteTile(Vector2 position)
    {

        int positionX = (int)position.x;
        int positionY = (int)position.y;

        if (tileDatas[positionX, positionY] == null) return;

        else
        {

            tileDatas[positionX, positionY].Reset();


        }

    }


    public bool FacingTop(Vector2 position)
    {

        Vector2 otherPosition = position + new Vector2(0, 1);

        
        if (GetTileType(otherPosition) == 1 && GetTileRotation(otherPosition) == 180) return true;
        if (GetTileType(otherPosition) == 2 && GetTileRotation(otherPosition) == 0) return true;
        if (GetTileType(otherPosition) == 3 && GetTileRotation(otherPosition) == 0) return true;
        if (GetTileType(otherPosition) == 3 && GetTileRotation(otherPosition) == 270) return true;
        if (GetTileType(otherPosition) == 4 && GetTileRotation(otherPosition) != 90) return true;
        if (GetTileType(otherPosition) == 5) return true;


        //Length 2 Fix
        if (streetGen.tileCount == 2 && streetGen.direction == 0 && GetTileType(otherPosition) != -1 && (streetGen.startPos.Equals(position) || streetGen.endPos.Equals(position)) && (streetGen.startPos.Equals(otherPosition) || (streetGen.endPos.Equals(otherPosition)))) return true;


        return false;
    }
    public bool FacingDown(Vector2 position)
    {
        Vector2 otherPosition = position + new Vector2(0, -1);

        
        if (GetTileType(otherPosition) == 1 && GetTileRotation(otherPosition) == 0) return true;
        if (GetTileType(otherPosition) == 2 && GetTileRotation(otherPosition) == 0) return true;
        if (GetTileType(otherPosition) == 3 && GetTileRotation(otherPosition) == 90) return true;
        if (GetTileType(otherPosition) == 3 && GetTileRotation(otherPosition) == 180) return true;
        if (GetTileType(otherPosition) == 4 && GetTileRotation(otherPosition) != 270) return true;
        if (GetTileType(otherPosition) == 5) return true;

        //Length 2 Fix
        if (streetGen.tileCount == 2 && streetGen.direction == 2 && GetTileType(otherPosition) != -1 && (streetGen.startPos.Equals(position) || streetGen.endPos.Equals(position)) && (streetGen.startPos.Equals(otherPosition) || (streetGen.endPos.Equals(otherPosition)))) return true;


        return false;
    }
    public bool FacingRight(Vector2 position)
    {
        Vector2 otherPosition = position + new Vector2(1, 0);

        
        if (GetTileType(otherPosition) == 1 && GetTileRotation(otherPosition) == 270) return true;
        if (GetTileType(otherPosition) == 2 && GetTileRotation(otherPosition) == 90) return true;
        if (GetTileType(otherPosition) == 3 && GetTileRotation(otherPosition) == 0) return true;
        if (GetTileType(otherPosition) == 3 && GetTileRotation(otherPosition) == 90) return true;
        if (GetTileType(otherPosition) == 4 && GetTileRotation(otherPosition) != 180) return true;
        if (GetTileType(otherPosition) == 5) return true;

        //Length 2 Fix
        if (streetGen.tileCount == 2 && streetGen.direction == 1 && GetTileType(otherPosition) != -1 && (streetGen.startPos.Equals(position) || streetGen.endPos.Equals(position)) && (streetGen.startPos.Equals(otherPosition) || (streetGen.endPos.Equals(otherPosition)))) return true;


        return false;
    }
    public bool FacingLeft(Vector2 position)
    {
        Vector2 otherPosition = position + new Vector2(-1, 0);

        
        if (GetTileType(otherPosition) == 1 && GetTileRotation(otherPosition) == 90) return true;
        if (GetTileType(otherPosition) == 2 && GetTileRotation(otherPosition) == 90) return true;
        if (GetTileType(otherPosition) == 3 && GetTileRotation(otherPosition) == 180) return true;
        if (GetTileType(otherPosition) == 3 && GetTileRotation(otherPosition) == 270) return true;
        if (GetTileType(otherPosition) == 4 && GetTileRotation(otherPosition) != 0) return true;
        if (GetTileType(otherPosition) == 5) return true;


        //Length 2 Fix
        if (streetGen.tileCount == 2 && streetGen.direction == 3 && GetTileType(otherPosition) != -1 && (streetGen.startPos.Equals(position) || streetGen.endPos.Equals(position)) && (streetGen.startPos.Equals(otherPosition) || (streetGen.endPos.Equals(otherPosition)))) return true;


        return false;
    }







    public Vector2 GetGridTileFromPosition(Vector3 position)
    {

        int positionX = (int)(position.x / tileSizeX + totalTilesX / 2);
        int positionY = (int)(position.z / tileSizeY + totalTilesY / 2);

        return new Vector2(positionX, positionY);

    }
    public Vector3 WorldPositionFromGridTile(Vector2 position)
    {

        int positionX = (int)((position.x - totalTilesX / 2) * tileSizeX);
        int positionY = (int)((position.y - totalTilesX / 2) * tileSizeY);

        return new Vector3(positionX, 0, positionY);

    }



    public Vector2 GetChunkFromPosition(Vector2 position)
    {
        Vector2 chunkPos;

        Vector3 worldpos = WorldPositionFromGridTile(position);

        chunkPos.x = RoundTransform(worldpos, tileSizeX * chunkSize, tileSizeY * chunkSize).x / (tileSizeX * chunkSize);
        chunkPos.y = RoundTransform(worldpos, tileSizeX * chunkSize, tileSizeY * chunkSize).z / (tileSizeY * chunkSize);

        return chunkPos;

    }

    public Vector3 GetFirstWorldPositionFromChunk(Vector2 chunkPosition)
    {
        Vector3 pos;

        pos.x = chunkPosition.x * chunkSize + (totalTilesX - 1) / 2 - chunkSize / 2;
        pos.y = chunkPosition.y * chunkSize + (totalTilesY - 1) / 2 - chunkSize / 2;
        pos.z = 0;


        if (Mathf.Abs(chunkPosition.x) == 1) ++pos.x;
        if (Mathf.Abs(chunkPosition.y) == 1) ++pos.y;

        return pos;

    }





    public GameObject NewChunk(Vector2 pos)
    {

        if (chunks.ContainsKey(pos)) return chunks[pos];

        chunks[pos] = new GameObject();
        chunks[pos].name = "Chunk: " + pos.x + " " + pos.y;
        chunks[pos].AddComponent<MeshFilter>();
        chunks[pos].AddComponent<MeshRenderer>();
        chunks[pos].transform.parent = worldParent.transform;

        return chunks[pos];
    }

    public GameObject GetChunk(Vector2 pos)
    {

        if (chunks.ContainsKey(pos)) return chunks[pos];

        else return null;
    }

    public void DeleteChunk(Vector2 pos)
    {

        if (chunks.ContainsKey(pos)) chunks.Remove(pos);
     
    }




    public void AddBuilding(Vector3 pos, TileDataBuildings buildingData) {

        if (buildingDatas.ContainsKey(pos)) buildingDatas[pos] = buildingData;

        else buildingDatas.Add(pos, buildingData);
        
    }

    public void AddTree(Vector3 pos, TileDataTrees treeData)
    {

        if (treeDatas.ContainsKey(pos)) treeDatas[pos] = treeData;

        else treeDatas.Add(pos, treeData);

    }

    public void DeleteObject(Vector3 pos) {

        if (buildingDatas.ContainsKey(pos)) buildingDatas.Remove(pos);
        if (treeDatas.ContainsKey(pos)) treeDatas.Remove(pos);
    }








    public bool CheckIfTileFree(Vector3 pos) {

        if (buildingDatas.ContainsKey(pos)) return false;
        if (treeDatas.ContainsKey(pos)) return false;

        Vector2 pos2 = GetGridTileFromPosition(pos);

        if (GetTileType(pos2) != -1 && GetTileType(pos2) != 0) return false;

        return true;
    }

















    private Vector3 RoundTransform(Vector3 v, float snapValueX, float snapValueZ)
    {
        return new Vector3
        (
            snapValueX * Mathf.Round(v.x / snapValueX),
            v.y + 0.25f,
            snapValueZ * Mathf.Round(v.z / snapValueZ)

        );
    }










    public Vector2 GetRandomSpawnTile()
    {
        List<Vector2> spawnTiles = new List<Vector2>();

        for (int i = 0; i < totalTilesX; i++)
        {

            for (int j = 0; j < totalTilesY; j++)
            {


                if (tileDatas[i, j] != null && GetTileType(new Vector2(i, j)) == 2)
                {
                    spawnTiles.Add(new Vector2(i, j));

                }
            }
        }

        if (spawnTiles.Count == 0) return new Vector2(-999, -999);

        int random = UnityEngine.Random.Range(0, spawnTiles.Count);

        return spawnTiles[random];

    }
}