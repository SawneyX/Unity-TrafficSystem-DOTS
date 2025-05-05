using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

public class UIControls : MonoBehaviour
{

    public GameObject InventoryPanel;
    public GameObject InfoPanel;

    public PointAndClick pointAndClick;

    bool infoPanel = false;

    Vector3 infoPanelOffset = new Vector3(-90, 0, 0);

    EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {

        if (infoPanel) {

             

            if (pointAndClick.lastClickedEntity != Entity.Null) {


                Vector3 entityPos = entityManager.GetComponentData<Translation>(pointAndClick.lastClickedEntity).Value;

                float heigth = entityManager.GetSharedComponentData<RenderMesh>(pointAndClick.lastClickedEntity).mesh.bounds.extents.z;

                Vector2 position = Camera.main.WorldToScreenPoint(entityPos + new Vector3(0, heigth + 1f, 0)); 

                InfoPanel.transform.position = position;

                float dist = Vector3.Distance(Camera.main.transform.position, entityPos);

                InfoPanel.transform.localPosition += infoPanelOffset / Mathf.Clamp(dist/90, 1f, 5);   //90 higher = further away starts getting closer
                InfoPanel.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = entityManager.GetComponentData<ECSBuilding>(pointAndClick.lastClickedEntity).name.ToString();

                InfoPanel.SetActive(true);
            }
            else InfoPanel.SetActive(false);

        }

    }


    public void ShowInventory()
    {
        InventoryPanel.SetActive(true);


    }

    public void HideInventory() {

        InventoryPanel.SetActive(false);

    }

    public void InfoPanelActive()
    {
        
        infoPanel = true;
    }

    public void InfoPanelNotActive()
    {
    
        infoPanel = false;
    }

}
