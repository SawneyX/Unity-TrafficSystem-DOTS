using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;



namespace Authoring
{
    [DisallowMultipleComponent]
    public class SpawnDataTrees : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
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

            Builder.SetTrees(prefabs); //Set Trees to Builder

            for (int i = 0; i < prefabs.Length; i++)
            {
                Entity prefabEntity = conversionSystem.GetPrimaryEntity(prefabs[i]);

                buffer.Add(new ECSBufferEntity { entity = prefabEntity });

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

