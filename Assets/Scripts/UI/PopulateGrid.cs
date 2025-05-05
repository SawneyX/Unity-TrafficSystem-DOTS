using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateGrid : MonoBehaviour
{
    public GameObject prefab; // This is our prefab object that will be exposed in the inspector

    public List<GameObject> elements = new List<GameObject>();

    public int numberToCreate; // number of objects to create. Exposed in inspector

    void Start()
    {
        //numberToCreate = elements.Capacity;
        Populate();
    }

    void Update()
    {

    }

    void Populate()
    {
        GameObject newObj; // Create GameObject instance

        for (int i = 0; i < elements.Capacity + numberToCreate; i++)
        {

            if (i < elements.Capacity) {
                newObj = (GameObject)Instantiate(elements[i], transform);
            }

            else newObj = (GameObject)Instantiate(prefab, transform);

           
        }

    }
}