using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

/*

public class SpawnData : MonoBehaviour, IConvertGameObjectToEntity
{
    

    public static Entity[] entities;
    public GameObject[] prefabs;

    private EntityManager entityManager;

    public EntityManager GetEntityManager()
    {
        return entityManager;
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        entities = new Entity[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            Entity prefabEntity = conversionSystem.GetPrimaryEntity(prefabs[i]);
            entities[i] = prefabEntity;
        }


        this.entityManager = dstManager;

    }


    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            referencedPrefabs.Add(prefabs[i]);
        }
    }


}

*/




 /*
//[ChunkSerializable]
struct SpawnData : IComponentData
{

    //public Entity buildingEntity;

    
    public DynamicBuffer<ECSBufferEntity> entities;
    



}
*/

namespace Authoring
{
    [DisallowMultipleComponent]
    public class SpawnData : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
       
        public GameObject[] prefabs;
        private EntityManager entityManager;
        private Entity entity;

        public Builder Builder;
        

        public Entity GetEntity()
        {
            return entity;
        }
        public EntityManager GetEntityManager()
        {
            return entityManager;
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            DynamicBuffer<ECSBufferEntity> buffer = dstManager.AddBuffer<ECSBufferEntity>(entity);

            entityManager = dstManager;
            this.entity = entity;

            Builder.SetBuildings(prefabs); //Set Buildings to Builder

            for (int i = 0; i < prefabs.Length; i++)
            {
                Entity prefabEntity = conversionSystem.GetPrimaryEntity(prefabs[i]);

                buffer.Add(new ECSBufferEntity { entity = prefabEntity });

                /*
                
                */
                

                //buffer.Add(new ECSBufferEntity { entity = entity});

                //dstManager.AddComponentData<SpawnData>(entity, new ECSBufferEntity { entity = entity})

            }
            /*
            
            dstManager.AddComponentData(entity, new global::SpawnData
            {
                entities = buffer,

            });
            

            */




        }
        
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.AddRange(prefabs);
                
        }



       



    }
}

 