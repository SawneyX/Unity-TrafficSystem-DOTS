using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOSpawnTest : MonoBehaviour
{
    public GameObject carPrefab;

    float spawnTimer;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && Time.time <= 30)
        {

            spawnTimer = 0.01f;

            Instantiate(carPrefab, new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)), Quaternion.identity);

        }
    }
}
