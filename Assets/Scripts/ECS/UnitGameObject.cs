using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class UnitGameObject : MonoBehaviour
{
    [SerializeField] private ConvertedEntityHolder convertedEntityHolder;

    [SerializeField] private Authoring.SpawnData spawnDataEntityHolder;

    [SerializeField] private Authoring.SpawnDataTrees spawnDataTreesEntityHolder;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        
    }



    public void ECSSpawnCar(CarData carData, List<Vector3> tilePath) {


        EntityManager entityManager = convertedEntityHolder.GetEntityManager();
        Entity entity = convertedEntityHolder.GetEntity();
        

        Entity spawnedEntity = entityManager.Instantiate(entity);   //TODO: CAR TYPE




        DynamicBuffer<ECSPathPosition> buffer = entityManager.GetBuffer<ECSPathPosition>(spawnedEntity);


        for (int i = tilePath.Count - 1; i > 0; i--) {

            Vector3 vec = tilePath[i];
            float3 pos = new float3(vec.x, vec.y, vec.z);

            buffer.Add(new ECSPathPosition { position = pos });
        }

        entityManager.SetComponentData<ECSPathFollow>(spawnedEntity, new ECSPathFollow { pathIndex = buffer.Length - 1 });

        Vector3 startVec = tilePath[0];
        float3 startPos = new float3(startVec.x, startVec.y, startVec.z);

        entityManager.SetComponentData(spawnedEntity, new Translation { Value = startPos });

        entityManager.AddComponentData(spawnedEntity, new ECSCar{ type = carData.type, speed = carData.speed, good = carData.good, amount = carData.amount });


    }


    public void ECSSpawnBuilding(Vector3 position, TileDataBuildings buildingData)
    {
        EntityManager entityManager = spawnDataEntityHolder.GetEntityManager();
        Entity entity = spawnDataEntityHolder.GetEntity();


        DynamicBuffer<ECSBufferEntity> entityBuffer = entityManager.GetBuffer<ECSBufferEntity>(entity);


        Entity spawnedEntity = entityManager.Instantiate(entityBuffer.ElementAt(buildingData.BuildingType).entity);

        //var dir = Quaternion.AngleAxis(buildingData.TileRotY, Vector3.forward) * Vector3.right;
        //quaternion rotation = quaternion.LookRotation(dir, new float3(0, 1, 0));

        quaternion rotation = entityManager.GetComponentData<Rotation>(spawnedEntity).Value;// quaternion.Euler(0, buildingData.TileRotY, 0);
        rotation = rotation * Quaternion.Euler(0, 0, buildingData.TileRotY);

        entityManager.SetComponentData(spawnedEntity, new Translation { Value = position });
        entityManager.SetComponentData(spawnedEntity, new Rotation { Value = rotation });
        entityManager.AddComponentData(spawnedEntity, new ECSBuilding { name = buildingData.BuildingType.ToString(), inhabitants = 0, energy = 0 });
       

    }

    public void ECSSpawnTree(Vector3 position, TileDataTrees treesData)
    {
        EntityManager entityManager = spawnDataTreesEntityHolder.GetEntityManager();
        Entity entity = spawnDataTreesEntityHolder.GetEntity();


        DynamicBuffer<ECSBufferEntity> entityBuffer = entityManager.GetBuffer<ECSBufferEntity>(entity);


        Entity spawnedEntity = entityManager.Instantiate(entityBuffer.ElementAt(treesData.TreeType).entity);

        var dir = Quaternion.AngleAxis(treesData.TileRotY, Vector3.up) * Vector3.right;
        quaternion rotation = quaternion.LookRotation(dir, new float3(0, 1, 0));

        
        entityManager.SetComponentData(spawnedEntity, new Translation { Value = position });
        entityManager.SetComponentData(spawnedEntity, new Rotation { Value =  rotation });
        //entityManager.SetComponentData(spawnedEntity, new Scale { Value = 1 });

    }

}
