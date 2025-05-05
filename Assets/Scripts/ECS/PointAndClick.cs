using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.EventSystems;

public class PointAndClick : MonoBehaviour
{
    public static int RAYCAST_DISTANCE = 150;

    public int type = -1;


    public Grid grid;

    public UnityEngine.Material buildingMat;
    public UnityEngine.Material transparentMat;


    public Entity lastClickedEntity = Entity.Null;

    public Entity hoverOverEntity = Entity.Null;

    PhysicsWorld physicsWorld => World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
    EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public Transport transport;

    void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //Hover Over Entity
        if (type == 1) {
            var screenPointToRay1 = Camera.main.ScreenPointToRay(Input.mousePosition);

            var rayInput1 = new RaycastInput
            {
                Start = screenPointToRay1.origin,
                End = screenPointToRay1.GetPoint(RAYCAST_DISTANCE),
                Filter = CollisionFilter.Default
            };

            if (!physicsWorld.CastRay(rayInput1, out Unity.Physics.RaycastHit hit1)) {
                hoverOverEntity = Entity.Null;
                return;
            }
            

            hoverOverEntity = physicsWorld.Bodies[hit1.RigidBodyIndex].Entity;
        }


        if (!Input.GetMouseButtonDown(0)) return;


        if (lastClickedEntity != Entity.Null) {

            var renderMesh = entityManager.GetSharedComponentData<RenderMesh>(lastClickedEntity);
            renderMesh.material = buildingMat;

            entityManager.SetSharedComponentData(lastClickedEntity, renderMesh);
            lastClickedEntity = Entity.Null;
        }

        if (type == -1) return;



        var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        var rayInput = new RaycastInput
        {
            Start = screenPointToRay.origin,
            End = screenPointToRay.GetPoint(RAYCAST_DISTANCE),
            Filter = CollisionFilter.Default
        };

        if (!physicsWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit)) return;

        var selectedEntity = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;

        //if (selectedEntity == Entity.Null) type = -1;

        //SELECT
        if (type == 0)
        {

            //BUILDING
            if (entityManager.HasComponent<ECSBuilding>(selectedEntity))
            {

                var renderMesh = entityManager.GetSharedComponentData<RenderMesh>(selectedEntity);
                //var mat = new UnityEngine.Material(renderMesh.material);
                //mat.SetFloat("__Surface", 2);
                //mat.SetColor("_BaseColor", mat.color + new Color(4, 0, 0, -0.7f));

                renderMesh.material = transparentMat;

                entityManager.SetSharedComponentData(selectedEntity, renderMesh);

                lastClickedEntity = selectedEntity;

                if (transport.ready) transport.StartTransport(entityManager.GetComponentData<Translation>(selectedEntity).Value); //Start transport

            }

            //CAR
            if (entityManager.HasComponent<ECSCar>(selectedEntity))
            {
                //TODO

            }
        }


        //DESTROY
        if (type == 1) {

            grid.DeleteObject(entityManager.GetComponentData<Translation>(selectedEntity).Value);
            entityManager.DestroyEntity(selectedEntity);
            
        }
       
        
    }
}