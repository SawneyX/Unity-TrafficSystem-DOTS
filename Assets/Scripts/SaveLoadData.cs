using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.Mathematics;

public class SaveLoadData : MonoBehaviour
{
    

    public string worldName = "Test1";

    private string tileDataFileName = "TilesData.json";
    private string buildingDataFileName = "BuildingData.json";
    private string treeDataFileName = "TreeData.json";

    //private string worldPath;

    public GameData gameData;
  

    private TileData[,] tileDatas;
    private Dictionary<Vector3, TileDataBuildings> buildingDatas;
    private Dictionary<Vector3, TileDataTrees> treeDatas;








    public void SaveGameData(TileData[,] tiles, Dictionary<Vector3, TileDataBuildings> buildings, Dictionary<Vector3, TileDataTrees> trees)
    {



        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, worldName))) Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, worldName));


        //TILES


        string tileDataAsJson = JsonConvert.SerializeObject(tiles, Formatting.None);
        string tileFilePath = Path.Combine(Application.persistentDataPath, worldName + "/" + tileDataFileName);

        File.WriteAllText(tileFilePath, tileDataAsJson); // Tiles



        //BUILDINGS

        Dictionary<PosVector3, TileDataBuildings> buildingDatasPos2 = new Dictionary<PosVector3, TileDataBuildings>();

        foreach (var key in buildings)
        {

            buildingDatasPos2.Add(new PosVector3(key.Key.x, key.Key.y, key.Key.z), key.Value);
        }



        string buildingDataAsJson = JsonConvert.SerializeObject(buildingDatasPos2.ToArray(), Formatting.None);
        string buildingFilePath = Path.Combine(Application.persistentDataPath, worldName + "/" + buildingDataFileName);

        File.WriteAllText(buildingFilePath, buildingDataAsJson); // Buildings


        //TREES

        Dictionary<PosVector3, TileDataTrees> treeDatasPos2 = new Dictionary<PosVector3, TileDataTrees>();

        foreach (var key in trees)
        {
            
            treeDatasPos2.Add(new PosVector3(key.Key.x, key.Key.y, key.Key.z), key.Value);
        }



        string treeDataAsJson = JsonConvert.SerializeObject(treeDatasPos2.ToArray(), Formatting.None);
        string treeFilePath = Path.Combine(Application.persistentDataPath, worldName + "/" + treeDataFileName);

        File.WriteAllText(treeFilePath, treeDataAsJson); // Trees


        Debug.Log("Saved Data");
    }




















    public TileData[,] LoadTileData(int width, int height)
    {

        string fileTilePath = Path.Combine(Application.persistentDataPath, worldName + "/" + tileDataFileName);
        



        if (File.Exists(fileTilePath))
        {
            Debug.Log("Load Streets");

            // Read the json from the file into a string
            string tileDataAsJson = File.ReadAllText(fileTilePath);
            

            tileDatas = JsonConvert.DeserializeObject<TileData[,]>(tileDataAsJson);

            return tileDatas;
        }
        else
        {
            return new TileData[width, height];
        }
    }



    public Dictionary<Vector3, TileDataBuildings> LoadBuildings() {

        string fileBuildingPath = Path.Combine(Application.persistentDataPath, worldName + "/" + buildingDataFileName);


        if (File.Exists(fileBuildingPath))
        {
            Debug.Log("Load Buildings");

           
            string buildingDataAsJson = File.ReadAllText(fileBuildingPath);

            JsonSerializerSettings set = new JsonSerializerSettings();

            //Dictionary<PosVector2, TileDataBuildings> buildingDatasPos2 = JsonConvert.DeserializeObject<Dictionary<PosVector2, TileDataBuildings>>(buildingDataAsJson).ToDictionary(i => i.Key.x);

            Dictionary<PosVector3, TileDataBuildings> buildingDatasPos2 = JsonConvert.DeserializeObject<KeyValuePair<PosVector3, TileDataBuildings>[]>(buildingDataAsJson).ToDictionary(kv => kv.Key, kv => kv.Value);

            buildingDatas = new Dictionary<Vector3, TileDataBuildings>();

            foreach (var key in buildingDatasPos2) {

                buildingDatas.Add(new Vector3(key.Key.x, key.Key.y, key.Key.z), key.Value);
            }
            

            return buildingDatas;
        }
        else
        {
            return new Dictionary<Vector3, TileDataBuildings>();
        }

    }

    public Dictionary<Vector3, TileDataTrees> LoadTrees()
    {

        string fileTreePath = Path.Combine(Application.persistentDataPath, worldName + "/" + treeDataFileName);


        if (File.Exists(fileTreePath))
        {
            Debug.Log("Load Trees");


            string treeDataAsJson = File.ReadAllText(fileTreePath);


           

            Dictionary<PosVector3, TileDataTrees> treeDatasPos2 = JsonConvert.DeserializeObject<KeyValuePair<PosVector3, TileDataTrees>[]>(treeDataAsJson).ToDictionary(kv => kv.Key, kv => kv.Value);

            treeDatas = new Dictionary<Vector3, TileDataTrees>();

            foreach (var key in treeDatasPos2)
            {

                treeDatas.Add(new Vector3(key.Key.x, key.Key.y, key.Key.z), key.Value);
            }


            return treeDatas;
        }
        else
        {
            return new Dictionary<Vector3, TileDataTrees>();
        }

    }





    



    
    




















    private static T[,] Make2DArray<T>(T[] input, int height, int width)
    {
        T[,] output = new T[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                output[j, i] = input[i * width + j];
            }
        }
        return output;
    }



}

[System.Serializable]
public struct PosVector3 {

    public float x;
    public float y;
    public float z;


    public PosVector3(float x, float y, float z) {

        this.x = x;
        this.y = y;
        this.z = z;
    }

}
