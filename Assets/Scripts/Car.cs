using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    public float tileSizeX = 3;
    public float tileSizeY = 3;


    public int totalTilesX = 101;
    public int totalTilesY = 101;

    int dirNow;
    int dirNext;


    public List<Vector3> TileList = new List<Vector3>();

    //private List<Vector3> tileWaypoints = new List<Vector3>();

    

    Vector2 startPosition;

    bool drive = false;

    //Vector3 currentTile;

    //int currentNode = 0;

   
    float speed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (drive)
        {
            float step = speed * Time.deltaTime;

            

            if (transform.position != TileList[0])
            {

                //transform.position = Vector3.Lerp(transform.position, TileList[0], Timer);
                transform.position = Vector3.MoveTowards(transform.position, TileList[0], step);

                Quaternion targetRotation = Quaternion.LookRotation(TileList[0] - transform.position + new Vector3(0.001f, 0, 0));

                
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * speed * Time.deltaTime);
                
            }

            

            if (Vector3.Distance(transform.position, TileList[0]) < 0.01f)
            {
                if (TileList.Count > 0) TileList.RemoveAt(0);
                if (TileList.Count == 0) DestroyImmediate(gameObject);

            }


            
        }
    }
  

    public void StartDriving(List<Vector3> TileList)
    {


        transform.position =  TileList[0];

        this.TileList = TileList;
       
        if (TileList.Count > 0) drive = true;
        


    }


    /*
    public Vector3 WorldPositionFromGridTile(Vector2 position)
    {

        int positionX = (int)((position.x - totalTilesX / 2) * tileSizeX);
        int positionY = (int)((position.y - totalTilesX / 2) * tileSizeY);

        return new Vector3(positionX, 0, positionY);

    }
    */
}
