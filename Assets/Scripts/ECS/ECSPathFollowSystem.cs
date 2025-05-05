using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Jobs;

public class ECSPathFollowSystem : SystemBase
{

    BeginInitializationEntityCommandBufferSystem mSystem;

    protected override void OnCreate()
    {
        mSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>(); 
    }


    [BurstCompatible]
    protected override void OnUpdate()
    {
        EntityManager entityManager = World.EntityManager;
        float time = Time.DeltaTime;

        
        float rotSpeed = 6f;

        BuildPhysicsWorld buildphysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildphysicsWorld.PhysicsWorld.CollisionWorld;

        

        //BeginInitializationEntityCommandBufferSystem 
        EntityCommandBuffer.ParallelWriter entityCommandBuffer = mSystem.CreateCommandBuffer().AsParallelWriter();
        
        


        //var entityCommandBuffer = mSystem.CreateCommandBuffer();

        Entities.WithReadOnly(collisionWorld).ForEach((Entity entity,  ref ECSPathFollow pathFollow, ref DynamicBuffer<ECSPathPosition> pathPositionBuffer, ref Translation translation, ref Rotation rotation, ref ECSCar ecsCar) =>
        {
            //DynamicBuffer<ECSPathPosition> pathPositionBuffer = entityManager.GetBuffer<ECSPathPosition>(entity);

            //GetComponentDataFromEntity<ECSPathFollow>()[entity].pathIndex = 2;

            float MaxMoveSpeed = ecsCar.speed;//2.75f;

            if (pathFollow.pathIndex >= 0)
            {

                float3 targetPosition = pathPositionBuffer[pathFollow.pathIndex].position;

                //float3 targetPosition = new float3(pathPosition.x, 0, pathPosition.z);
                float3 moveDir = math.normalizesafe(targetPosition - translation.Value);
                float moveSpeed = MaxMoveSpeed;


                //RayCast Check Distance

                float raydistance = 2f;

                //Entity hitEntity;// = RayCast(translation.Value + moveDir * 0.5f, translation.Value + moveDir * raydistance);



                float3 fromPosition = translation.Value + moveDir * 0.5f;
                float3 toPosition = translation.Value + moveDir * raydistance;


                //BuildPhysicsWorld buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();//World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
                //CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = fromPosition,
                    End = toPosition,
                    Filter = new CollisionFilter  //Layers
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1 << 6,//~0u,
                        GroupIndex = 0,
                    }
                };
                Unity.Physics.RaycastHit raycastHit = new Unity.Physics.RaycastHit();

                if (collisionWorld.CastRay(raycastInput, out raycastHit))  //Hit Something?
                {
                    float3 hitEntityPosition = raycastHit.Position;
                    //Entity hitEntityResult = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;

                    //hitEntity = hitEntityResult;

                    //-----

                    //LocalToWorld targetTransform = GetComponent<LocalToWorld>(hitEntity);
                    //float3 hitEntityPosition = targetTransform.Position;

                    float distance = math.distance(translation.Value, hitEntityPosition);

                    moveSpeed = distance / 2;
                    if (distance <= 0.7f) moveSpeed = 0;

                    moveSpeed = math.clamp(moveSpeed, 0, MaxMoveSpeed);




                }
                else
                {
                    //hitEntity = Entity.Null;
                }


                /*
                if (hitEntityPos != null) {

                   

                }
                */
                //else moveSpeed = MaxMoveSpeed;

                //Rotation
                quaternion targetRotation = quaternion.LookRotation(moveDir, new float3(0, 1, 0));

                rotation.Value = math.slerp(rotation.Value, targetRotation, rotSpeed * time);


                translation.Value += moveDir * moveSpeed * time;



                if (math.distance(translation.Value, targetPosition) < 0.02f)
                {

                    pathFollow.pathIndex--;


                    if (pathFollow.pathIndex == 0) { //Set Last rotation (parking)

                        rotation.Value = quaternion.LookRotation(moveDir, new float3(0, 1, 0));
                    }

                }
            }

            else {


                
                //entityCommandBuffer.DestroyEntity(0, entity);
                
            }
            


        }).WithBurst().ScheduleParallel();

        //mSystem.AddJobHandleForProducer(this.Dependency);

    }


    /*
    private Entity RayCast(float3 fromPosition, float3 toPosition) {



        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        RaycastInput raycastInput = new RaycastInput
        {
            Start = fromPosition,
            End = toPosition,
            Filter = new CollisionFilter  //Layers
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0,
            }
        };
        Unity.Physics.RaycastHit raycastHit = new Unity.Physics.RaycastHit();

        if (collisionWorld.CastRay(raycastInput, out raycastHit))  //Hit Something?
        {
            Entity hitEntity = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;

            return hitEntity;
        }
        else
        {
            return Entity.Null;
        }
    }
    */

}
