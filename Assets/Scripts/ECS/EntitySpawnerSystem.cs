using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;


public class EntitySpawnerSystem : ComponentSystem
{

    private float spawnTimer;
    private Random random;

    
    

    protected override void OnCreate()
    {
        //base.OnCreate();
        random = new Random(56);

    }

    

    protected override void OnUpdate() {

        /*
        
        EntityManager entityManager = World.EntityManager;
        spawnTimer -= Time.DeltaTime;

        if (spawnTimer <= 0f && Time.ElapsedTime <= 30) {

            spawnTimer = 0.01f;

            Entities.ForEach((ref ECSPathFollow pathFollow, ref SpawnData spawnData) =>  //
            {

                Entity spawnedEntity = entityManager.Instantiate(spawnData.carEntity);

                //SET PATH
                //DynamicBuffer<ECSPathPosition> buffer = EntityManager.AddBuffer<ECSPathPosition>(spawnedEntity);


                DynamicBuffer<ECSPathPosition> buffer = entityManager.GetBuffer<ECSPathPosition>(spawnedEntity);


                

                buffer.Add(new ECSPathPosition { position = new float3(50, 0, 20) });
                buffer.Add(new ECSPathPosition { position = new float3(10, 0, -30) });
                buffer.Add(new ECSPathPosition { position = new float3(10, 0, 10) });

                ComponentDataFromEntity<ECSPathFollow> pathFollowComponentDataFromEntity = GetComponentDataFromEntity<ECSPathFollow>();

                pathFollowComponentDataFromEntity[spawnedEntity] = new ECSPathFollow { pathIndex = buffer.Length - 1 };
                //




                //buffer.Add();

                EntityManager.SetComponentData(spawnedEntity, new Translation { Value = new float3(random.NextFloat(-10, 10), 0, random.NextFloat(-10, 10)) });


            });

            


            
        }
        
        */
    
    
    }
    
    
}








/*
public class EntitySpawnerSystem : ComponentSystem
{

    private float spawnTimer;
    private Random random;

    protected override void OnCreate()
    {
        //base.OnCreate();
        random = new Random(56);

    }



    protected override void OnUpdate()
    {


        spawnTimer -= Time.DeltaTime;

        if (spawnTimer <= 0f && Time.ElapsedTime <= 30)
        {

            spawnTimer = 0.01f;

           

            Entity spawnedEntity = EntityManager.Instantiate(SpawnData.carPrefab);

            EntityManager.SetComponentData(spawnedEntity, new Translation { Value = new float3(random.NextFloat(-5, 5), 0, random.NextFloat(-5, 5)) });


            
        }



    }


}
*/