using UnityEngine;
using Unity.Entities;
using Unity.Entities.Serialization;
using Newtonsoft.Json;
using Unity.Scenes;

public class ECSSaveSystem : MonoBehaviour
{
    

    Data dataHold;

    ReferencedUnityObjects g;



    private void Start()
    {
        dataHold = new Data();
        g = new ReferencedUnityObjects();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            save();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            load();
        }

        

    }
    public void save()
    {

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        using (var writer = new StreamBinaryWriter(Application.persistentDataPath + "/save")) //Application.dataPath + "/save"))
        {
            //SerializeUtility.SerializeWorld(entityManager, writer, out g);



            SerializeUtilityHybrid.Serialize(entityManager, writer, out g);
            print("save");
        }

        

        dataHold.Array = g.Array;
       

        
        //print(dataHold.Array[0]);
        //var data = JsonConvert.SerializeObject(dataHold);

       
       

        var data = JsonUtility.ToJson(dataHold);

        

        PlayerPrefs.SetString("Data", data);

    }
    public void load()
    {

        dataHold = new Data();

        var data = PlayerPrefs.GetString("Data");

        dataHold = JsonUtility.FromJson<Data>(data);

        

        g = new ReferencedUnityObjects();
        g.Array = dataHold.Array;

        //Data f = new Data();


        EntityManager main = World.DefaultGameObjectInjectionWorld.EntityManager;




        

        //var transaction = entityManager.BeginExclusiveEntityTransaction();
        //ExclusiveEntityTransaction e = new ExclusiveEntityTransaction();


        World world = new World("svet");

        //world.GetOrCreateSystem<ECSPathFollowSystem>();

        EntityManager entityManager = world.EntityManager;
        
        //main.MoveEntitiesFrom(entityManager);

        //entityManager.MoveEntitiesFrom(main);
        //entityManager.CopyAndReplaceEntitiesFrom(main);

        using (var reader = new StreamBinaryReader(Application.persistentDataPath + "/save"))
        {
            //var t = entityManager.BeginExclusiveEntityTransaction();

            //EntityManager e2 = tempWorld.EntityManager;//  GetOrCreateManager<EntityManager>();

            

            /*
            
            SerializeUtilityHybrid.DeserializeObjectReferences(g, out Object[] onh);


            print(onh.Length);
            print(onh[0]);
            print(onh[1]);
            print(onh[2]);

    */
            SerializeUtilityHybrid.Deserialize(entityManager, reader, g);

            //SerializeUtilityHybrid.Deserialize(e2, reader, onh);


            //entityManager.MoveEntitiesFrom(e2);


            //SerializeUtilityHybrid.Deserialize(entityManager, reader, g);
            //SerializeUtility.DeserializeWorld(transaction, reader, g);
            print("load");
        }
        //entityManager.EndExclusiveEntityTransaction();
        main.MoveEntitiesFrom(entityManager);
        
    }
}

[System.Serializable]
public class Data
{
    public Object[] Array;

}