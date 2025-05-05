using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Mathematics;

public class Builder : MonoBehaviour
{

    StreetGenerator streetGen;
    Grid grid;
    Graph graph;
    CarSpawner carSpawner;
    public UIControls uiControls;
    public PointAndClick pointAndClick;
    Stats stats;
    Transport transport;

    public GameObject hoverObject3x3;
    public GameObject hoverObject1x1;

    Vector3 position;
    Vector3 startPosition;
    Vector3 endPosition;

    public int type = -1;


    Vector2 startPath;
    Vector2 endPath;

    float buildSpeed = 2f;
    public Material houseBuildMaterial;


    public GameObject ECSManager;


    private GameObject[] Buildings; //Got from SpawnData
    private GameObject[] Trees; //Got from SpawnDataTrees
    public GameObject[] Misc;

    int buildingIndex = -1;
    float buildingRotation = 0;

    int miscIndex = -1;
    float miscRotation = 0;

    public GameObject trees;

    float distanceToSidewalk = 1f;

    public GameObject buildParticles;


    // Start is called before the first frame update
    void Start()
    {
        streetGen = GetComponent<StreetGenerator>();
        grid = GetComponent<Grid>();
        graph = GetComponent<Graph>();
        carSpawner = GetComponent<CarSpawner>();
        stats = GetComponent<Stats>();
        transport = GetComponent<Transport>();

        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            type = -1;
            uiControls.HideInventory();
            transport.ready = false;

            hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            //Delete
            if (type == 0)
            {
                Vector3 pos = new Vector3(hoverObject3x3.transform.position.x, 0, hoverObject3x3.transform.position.z);

                if (grid.buildingDatas.ContainsKey(pos)) return; //Check For Building
                if (grid.treeDatas.ContainsKey(pos)) return; //Check for Tree
                if (pointAndClick.hoverOverEntity != Entity.Null) return;

                position = grid.GetGridTileFromPosition(hoverObject3x3.transform.position);

                streetGen.OnDeleteTile(position);

                stats.UpdateStats();

            }




            //Buildings
            else if (type == 3)
            {

                if (Input.GetMouseButtonDown(0) && buildingIndex != -1)
                {

                    position = hoverObject3x3.transform.position;

                    if (grid.CheckIfTileFree(new Vector3(hoverObject3x3.transform.position.x, 0, hoverObject3x3.transform.position.z)) == false) return;


                    GameObject building = Buildings[buildingIndex];


                    GameObject instHouse = Instantiate(building, position - new Vector3(0, 0.25f, 0), building.transform.rotation * Quaternion.Euler(0, 0, buildingRotation));
                    instHouse.GetComponent<MeshRenderer>().material = new Material(houseBuildMaterial);
                    instHouse.GetComponent<BoxCollider>().enabled = false;

                    TileDataBuildings buildingData = new TileDataBuildings();
                    buildingData.BuildingType = buildingIndex;
                    buildingData.TileRotY = buildingRotation;

                    StartCoroutine(BuildBuilding(instHouse.GetComponent<MeshRenderer>(), buildingData));
                }
            }

            
        }



        //Foundation
        if (type == 1 && !EventSystem.current.IsPointerOverGameObject())
        {


            if (Input.GetMouseButtonDown(0))
            {


                startPosition = hoverObject3x3.transform.position;


            }
            else if (Input.GetMouseButtonUp(0))
            {

                endPosition = hoverObject3x3.transform.position;

                if (startPosition == endPosition) {

                    Vector2 pos = grid.GetGridTileFromPosition(endPosition);

                    if (grid.GetTileType(pos) == -1) streetGen.GenerateStreet(null, 0, endPosition, 0);
                    else if (grid.GetTileType(pos) == 3 && grid.tileDatas[(int)pos.x, (int)pos.y].TileSubType == -1) streetGen.UpdateSubType(pos, 1);
                    return;
                }


                Vector2 startTile = grid.GetGridTileFromPosition(startPosition);
                Vector2 endTile = grid.GetGridTileFromPosition(endPosition);

                if (Vector2.Distance(startTile, endTile) > 8) return;


                int fak1 = 1;
                int fak2 = 1;

                if (startTile.x > endTile.x) fak1 = -1;
                if (startTile.y > endTile.y) fak2 = -1;


                for (int i = (int)startTile.x; i != endTile.x + fak1; i += fak1)
                {


                    for (int j = (int)startTile.y; j != endTile.y + fak2; j += fak2)
                    {
                        Vector2 pos = new Vector2(i, j);

                        if (grid.GetTileType(pos) == -1) streetGen.GenerateStreet(null, 0, grid.WorldPositionFromGridTile(pos), 0);

                        else if (grid.GetTileType(pos) == 3 && grid.tileDatas[(int)pos.x, (int)pos.y].TileSubType == -1) streetGen.UpdateSubType(pos, 1);


                    }

                }
            }

        }

        //Vegetation
        else if (type == 4)
        {


            position = new Vector3(hoverObject3x3.transform.position.x, 0, hoverObject3x3.transform.position.z);

            Vector2 pos = grid.GetGridTileFromPosition(position);


            
            

            if (grid.GetTileType(pos) == 2)
            {

                if (grid.GetTileRotation(pos) == 0 || grid.GetTileRotation(pos) == 180)
                {

                    
                    if (Mathf.Abs(grid.mousePosition.x - (position.x + distanceToSidewalk)) <= 0.8f)
                    {

                        hoverObject1x1.transform.position = new Vector3(position.x + distanceToSidewalk, 0, grid.mousePosition.z);
                        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                    }
                    else if (Mathf.Abs(grid.mousePosition.x - (position.x - distanceToSidewalk)) <= 0.8f)
                    {

                        hoverObject1x1.transform.position = new Vector3(position.x - distanceToSidewalk, 0, grid.mousePosition.z);
                        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                    }
                    else hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;


                }
                else if (grid.GetTileRotation(pos) == 90 || grid.GetTileRotation(pos) == 270)
                {


                    if (Mathf.Abs(grid.mousePosition.z - (position.z + distanceToSidewalk)) <= 0.8f) {

                        hoverObject1x1.transform.position = new Vector3(grid.mousePosition.x, 0, position.z + distanceToSidewalk);
                        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                    }
                    else if (Mathf.Abs(grid.mousePosition.z - (position.z - distanceToSidewalk)) <= 0.8f)
                    {

                        hoverObject1x1.transform.position = new Vector3(grid.mousePosition.x, 0, position.z - distanceToSidewalk);
                        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                    }
                    else hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

                }
                
            }
            else hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;


            if (Input.GetMouseButtonDown(0))
            {

                if (grid.CheckIfTileFree(position) == false)
                {

                    //Place On StreetSide
                    if (grid.GetTileType(pos) == 2 && hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled)
                    {
                        TileDataTrees sidetreeData = new TileDataTrees();
                        sidetreeData.TreeType = 0;

                        ECSManager.GetComponent<UnitGameObject>().ECSSpawnTree(hoverObject1x1.transform.position, sidetreeData);

                        //TODO: TEST MIN DISTANCE

                        grid.AddTree(hoverObject1x1.transform.position, sidetreeData);
                    }


                    return;

                }

                TileDataTrees treeData = new TileDataTrees();
                treeData.TreeType = 1; 
                treeData.TileRotY = UnityEngine.Random.Range(0, 4) * 90;


                ECSManager.GetComponent<UnitGameObject>().ECSSpawnTree(position, treeData);

                grid.AddTree(position, treeData);

                //Instantiate(trees, position - new Vector3(0, 0.25f, 0), trees.transform.rotation);

            }





        }








        //Streets
        else if ((type == 2) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {

                //position = hoverObject.transform.position;
                startPosition = hoverObject3x3.transform.position;


            }

            else if (Input.GetMouseButtonUp(0))
            {

                endPosition = hoverObject3x3.transform.position;

                if (startPosition.x == endPosition.x || startPosition.z == endPosition.z)
                {   //Points on line

                    //GenerateStreet(null, streetType, endPosition, rotation);
                    //return;

                    //Test if line free
                    Vector2 startPosGrid = grid.GetGridTileFromPosition(startPosition);
                    Vector2 endPosGrid = grid.GetGridTileFromPosition(endPosition);

                    streetGen.tileCount = (int)Vector2.Distance(startPosGrid, endPosGrid) + 1;
                    int count = 0;

                    Vector3 nextPos = endPosition;

                    for (int i = 1; i < streetGen.tileCount - 1; i++)
                    {

                        nextPos = startPosition + i * (endPosition - startPosition) / (streetGen.tileCount - 1);
                        int type = grid.GetTileType(grid.GetGridTileFromPosition(nextPos));
                        if (type == -1 || type == 0) count++;
                        else break;

                    }


                    if (count == streetGen.tileCount - 2)
                    {

                        streetGen.GenStreetLine(startPosition, endPosition);

                        stats.UpdateStats();

                    }

                }

            }
        }



        //Generate Misc
        else if (type == 5 && !EventSystem.current.IsPointerOverGameObject()) { 


            if (Input.GetMouseButtonDown(0))
            {

                Vector2 pos = grid.GetGridTileFromPosition(hoverObject3x3.transform.position);
                if (grid.GetTileType(pos) == 2 && grid.GetTileSubType(pos) == -1) {

                    float rot = grid.GetTileRotation(pos);

                    if (rot == 0 || rot == 180) {


                        if (miscRotation == 0) {

                            grid.tileDatas[(int)pos.x, (int)pos.y].TileSubType = 0;
                            streetGen.GenMisc(pos);
                        }
                        if (miscRotation == 180)
                        {

                            grid.tileDatas[(int)pos.x, (int)pos.y].TileSubType = 2;
                            streetGen.GenMisc(pos);
                        }


                    }
                    else if (rot == 90 || rot == 270)
                    {
                        

                        if (miscRotation == 90)
                        {

                            grid.tileDatas[(int)pos.x, (int)pos.y].TileSubType = 1;
                            streetGen.GenMisc(pos);
                        }
                        if (miscRotation == 270)
                        {

                            grid.tileDatas[(int)pos.x, (int)pos.y].TileSubType = 3;

                            streetGen.GenMisc(pos);
                        }


                    }

                }


               


            }




        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Debug.Log("Set Start");
            startPath = grid.GetGridTileFromPosition(hoverObject3x3.transform.position);

            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Debug.Log("Set End");
            endPath = grid.GetGridTileFromPosition(hoverObject3x3.transform.position);


            CarData car = new CarData(0, 2.75f, -1, 0);

            carSpawner.SpawnCar(car, startPath, endPath);

        }


        //Hover Preview
        if (type == 3 && buildingIndex != -1)
        {


            GameObject building = Buildings[buildingIndex];

            hoverObject3x3.transform.GetChild(1).GetComponent<MeshFilter>().mesh = building.GetComponent<MeshFilter>().sharedMesh;
            hoverObject3x3.transform.GetChild(1).transform.rotation = building.transform.rotation * Quaternion.Euler(0, 0, buildingRotation);


            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {

                    if (buildingRotation == 270) buildingRotation = 0;
                    else buildingRotation += 90;
                    //hoverObject.transform.GetChild(1).transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                   

                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {

                    if (buildingRotation == 0) buildingRotation = 270;
                    else buildingRotation -= 90;
                    //hoverObject.transform.GetChild(1).transform.Rotate(new Vector3(0, -90, 0), Space.Self);
                    
                }
            }

        }
        else if (type == 5 && miscIndex != -1) {

            GameObject misc = Misc[miscIndex];
            hoverObject3x3.transform.GetChild(1).GetComponent<MeshFilter>().mesh = misc.GetComponent<MeshFilter>().sharedMesh;
            hoverObject3x3.transform.GetChild(1).transform.rotation = misc.transform.rotation * Quaternion.Euler(0, 0, miscRotation);

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {

                    if (miscRotation == 270) miscRotation = 0;
                    else miscRotation += 90;
                    


                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {

                    if (miscRotation == 0) miscRotation = 270;
                    else miscRotation -= 90;
                    

                }
            }

        }


        else
        {
            hoverObject3x3.transform.GetChild(1).GetComponent<MeshFilter>().mesh.Clear();
        }
        
       
    }


    private IEnumerator BuildBuilding(MeshRenderer meshRenderer, TileDataBuildings buildingData) {

        GameObject particles = Instantiate(buildParticles, meshRenderer.transform.position, Quaternion.identity);

        float i = 1.5f;

        float height = meshRenderer.bounds.extents.y * 2;

        while (i <= 1.5f + height) {

            meshRenderer.material.SetFloat("_CutoffHeight", i);
            i += 0.01f * buildSpeed;
            yield return new WaitForSeconds(0.01f);
            //yield return new WaitForEndOfFrame(); 
        }

        ECSManager.GetComponent<UnitGameObject>().ECSSpawnBuilding(meshRenderer.gameObject.transform.position, buildingData);

        

        Vector3 pos = (meshRenderer.gameObject.transform.position);

        //int2 posInt2 = new int2((int)pos.x, (int)pos.y);

        grid.AddBuilding(pos, buildingData);

        Destroy(meshRenderer.gameObject);
        Destroy(particles);

        stats.UpdateStats();

        yield return new WaitForEndOfFrame();

    }


    public IEnumerator InstantiateAllBuildings(Dictionary<Vector3, TileDataBuildings> buildingDatas) {


        foreach (var key in buildingDatas)
        {

            ECSManager.GetComponent<UnitGameObject>().ECSSpawnBuilding(new Vector3(key.Key.x, key.Key.y, key.Key.z), key.Value);

            yield return null;
        }

        stats.UpdateStats();

        yield return null;

    }

    public IEnumerator InstantiateAllTrees(Dictionary<Vector3, TileDataTrees> treeDatas)
    {


        foreach (var key in treeDatas)
        {

            ECSManager.GetComponent<UnitGameObject>().ECSSpawnTree(new Vector3(key.Key.x, key.Key.y, key.Key.z), key.Value);

            yield return null;
        }

        yield return null;

    }













    public void SetIndexOfBuildingString(string name) {

        for (int i = 0; i < Buildings.Length; ++i) {

            if (Buildings[i].name == name) {

                buildingIndex = i;
                break;
            }
            if (i == Buildings.Length - 1) buildingIndex = -1;
        }
        
    }


    public void SetBuildings(GameObject[] list) {

        Buildings = list;
    }
    public void SetTrees(GameObject[] list) {

        Trees = list;
    }




    public void OnDeleteClick()
    {
        type = 0;
        pointAndClick.type = 1; //Set type to delete

        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

    }
    public void OnFoundationClick()
    {

        type = 1;
        pointAndClick.type = -1;

        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
    }
    public void OnStreetClick()
    {

        type = 2;
        pointAndClick.type = -1;

        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnHouseClick() {

        
        type = 3;
        pointAndClick.type = -1;

        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnTreeClick()
    {

        type = 4;
        pointAndClick.type = -1;

        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnMiscClick()
    {

        type = 5;
        miscIndex = 0;
        pointAndClick.type = -1;

        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnInfoClick()
    {

        type = -1;
        if (pointAndClick.type != 0) {

            pointAndClick.type = 0;
            uiControls.InfoPanelActive();
        }
        else {

            pointAndClick.type = -1;
            uiControls.InfoPanelNotActive();
        }

        hoverObject1x1.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
    }



}
