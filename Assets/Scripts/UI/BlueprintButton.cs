using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintButton : MonoBehaviour
{
    Builder builder;

    // Start is called before the first frame update
    void Start()
    {
        builder = GameObject.Find("Manager").GetComponent<Builder>();
    }

   
    public void OnBlueprintPress(Transform trans)
    {



        string buildingString = trans.name.Substring(0, 2);

        builder.SetIndexOfBuildingString(buildingString);
    }


    /*
    public void OnClicked(Button button)
    {
        string buildingString = button.name.Substring(0, 2);

        builder.SetIndexOfBuildingString(buildingString);
    }
    */




}
