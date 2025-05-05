using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;


public class ECSPathPositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<ECSPathPosition>(entity);

    }



}