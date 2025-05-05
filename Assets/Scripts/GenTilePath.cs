using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenTilePath : MonoBehaviour
{
    Grid grid;

    int nowDir;
    int lastDir;

    Vector2 errorTile = new Vector2(-999, -999);

    public float widthStreet = 2.5f;
    public float lengthStreet = 6;

    float parkingDistanceX = 0.6f;
    float parkingDistanceY = 1f;


    //public float heightStreet = 0.3f;
    //public float widthSideStreet = 1.6f;

    List<Vector3> smallCurve;
    List<Vector3> bigCurve;

    static float pi = Mathf.PI;

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();

        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width
                                         



        smallCurve = new List<Vector3>() { new Vector3(-width / 2, 0, -length + 0.2f), curveVec(pi / 6, length - (width / 2)), curveVec(pi / 3, length - (width / 2)), new Vector3(-length + 0.2f, 0, -width / 2) };
        bigCurve = new List<Vector3>() { new Vector3(width / 2, 0, -length + 0.2f), curveVec(pi / 6, length + (width / 2)), curveVec(pi / 3, length + (width / 2)), new Vector3(-length + 0.2f, 0, width / 2) };


    }

    Vector3 curveVec(float angle, float radius)
    {

        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width

        return new Vector3(-length + (radius * Mathf.Cos(angle)), 0, -length + (radius * Mathf.Sin(angle)));
    }


    public List<Vector3> GeneratePath(CarData carData, LinkedList<Node> nodes) //components = position
    {  

        //List<Vector2> TileList = new List<Vector2>();

        bool transporter = carData.good != -1;

        List<Vector3> TileListWaypoints = new List<Vector3>();

        if (nodes.Count == 0) return TileListWaypoints;


        Vector2 startTile = nodes.First.Value.GetPosition();
        Vector2 endTile = nodes.Last.Value.GetPosition();

        Vector2 lastTile = startTile;
        Vector2 currentTile = startTile;



        nowDir = GetDirFromNextNode(nodes);

        lastDir = nowDir;


        while (currentTile != endTile)
        {

            //TileList.Add(currentTile);


            lastTile = currentTile;
            lastDir = nowDir;

            currentTile = GetNextTile(currentTile);

            TileListWaypoints = AddtoList(TileListWaypoints, lastTile, lastDir, nowDir);


            if (currentTile == errorTile)
            {

                Debug.Log("error");
                break;  //Test if all right
            }


            while (grid.GetTileType(currentTile) == 4 || grid.GetTileType(currentTile) == 5)  //3-Cross or 4-Cross
            {
                nodes.RemoveFirst();   //Remove first Node



                //TileList.Add(currentTile);
                lastTile = currentTile;




                if (nodes.Count >= 2)
                {
                    lastDir = nowDir;

                    currentTile = GetNextTileInDirection(currentTile, GetDirFromNextNode(nodes));

                    TileListWaypoints = AddtoList(TileListWaypoints, lastTile, lastDir, nowDir);

                }

                else if (nodes.Count == 1)
                {
                    lastDir = nowDir;

                    currentTile = GetNextTileInDirection(currentTile, nodes.Last.Value.pathStartDirection);

                    TileListWaypoints = AddtoList(TileListWaypoints, lastTile, lastDir, nowDir);

                }
                else
                {
                    TileListWaypoints = AddtoList(TileListWaypoints, lastTile, lastDir, nowDir);
                    break;
                }






            }

        }



        //TileListWaypoints = AddtoList(TileListWaypoints, endTile, nowDir, nowDir); //add last tile


        if (transporter) // Transporter add Building position
        {
            Vector3 end = grid.WorldPositionFromGridTile(endTile);

            float width = widthStreet / 4;  //width = half of actual width
            float length = lengthStreet / 4; //length = half of actual width


            if (nowDir == 3) TileListWaypoints.Add(end + new Vector3(-width / 2, 0, 0));
            else if (nowDir == 2) TileListWaypoints.Add(end + new Vector3(width / 2, 0, 0));
            else if (nowDir == 1) TileListWaypoints.Add(end + new Vector3(0, 0, width / 2));
            else if (nowDir == 0) TileListWaypoints.Add(end + new Vector3(0, 0, -width / 2));

            TileListWaypoints.Add(grid.WorldPositionFromGridTile(new Vector2(carData.targetX, carData.targetY)));
        }

        else TileListWaypoints = GenerateWaypointsToParking(TileListWaypoints, endTile, nowDir); //ELSE: parking waypoints

        if (currentTile == errorTile) TileListWaypoints.Clear();

        return TileListWaypoints;


    }


    Vector2 GetNextTile(Vector2 currentPos)
    {


        if (nowDir != 3 && grid.FacingTop(currentPos))
        {
            nowDir = 2;
            return currentPos + new Vector2(0, 1);
        }

        else if (nowDir != 2 && grid.FacingDown(currentPos))
        {
            nowDir = 3;
            return currentPos + new Vector2(0, -1);
        }
        else if (nowDir != 1 && grid.FacingRight(currentPos))
        {
            nowDir = 0;
            return currentPos + new Vector2(1, 0);
        }
        else if (nowDir != 0 && grid.FacingLeft(currentPos))
        {
            nowDir = 1;
            return currentPos + new Vector2(-1, 0);
        }

        else return errorTile;


    }

    Vector2 GetNextTileInDirection(Vector2 currentPos, int direction)
    {

        if (direction == 2)
        {
            nowDir = 2;
            return currentPos + new Vector2(0, 1);
        }

        else if (direction == 3)
        {
            nowDir = 3;
            return currentPos + new Vector2(0, -1);
        }
        else if (direction == 0)
        {
            nowDir = 0;
            return currentPos + new Vector2(1, 0);
        }
        else if (direction == 1)
        {
            nowDir = 1;
            return currentPos + new Vector2(-1, 0);
        }

        else return errorTile;

    }




    int GetDirFromNextNode(LinkedList<Node> nodes)
    {

        return nodes.First.Value.GetConnection(nodes.First.Next.Value.GetPosition()).GetDirection();

    }




    List<Vector3> AddtoList(List<Vector3> newpath, Vector2 position, int lastDirection, int nowDirection)
    {

        Vector3 worldPos = grid.WorldPositionFromGridTile(position);

        foreach (Vector3 vec in GenerateWaypoints(lastDirection, nowDirection))
        {

            newpath.Add(worldPos + vec);
        }

        return newpath;
    }



    public List<Vector3> GenerateWaypoints(int fromDir, int toDir)
    {


        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width
        //float height = heightStreet / 4;
        //float widthSide = widthSideStreet / 2;

        List<Vector3> wayPoints = new List<Vector3>();




        if (fromDir == toDir)   //Straight street
        {


            if (fromDir == 3) wayPoints.Add(new Vector3(-width / 2, 0, -length));
            else if (fromDir == 2) wayPoints.Add(new Vector3(width / 2, 0, length));
            else if (fromDir == 1) wayPoints.Add(new Vector3(-length, 0, width / 2));
            else if (fromDir == 0) wayPoints.Add(new Vector3(length, 0, -width / 2));
        }
        else
        {




            bool rightTurn = ((fromDir == 1 && toDir == 2) || (fromDir == 3 && toDir == 1) || (fromDir == 0 && toDir == 3) || (fromDir == 2 && toDir == 0));


            if (rightTurn) //smallCurve
            {

                if (fromDir == 0) wayPoints.AddRange(changeDirection(smallCurve));
                else if (fromDir == 1) wayPoints.AddRange(changeDirection(convertCurveFlip(smallCurve)));
                else if (fromDir == 2) wayPoints.AddRange(convertCurveVertical(smallCurve));
                else if (fromDir == 3) wayPoints.AddRange(convertCurveHorizontal(smallCurve));


            }
            else if (!rightTurn)  //bigCurve
            {
                if (fromDir == 0) wayPoints.AddRange(changeDirection(convertCurveHorizontal(bigCurve)));
                else if (fromDir == 1) wayPoints.AddRange(changeDirection(convertCurveVertical(bigCurve)));
                else if (fromDir == 2) wayPoints.AddRange(bigCurve);
                else if (fromDir == 3) wayPoints.AddRange(convertCurveFlip(bigCurve));

            }



        }




        return wayPoints;

    }



    public List<Vector3> GenerateWaypointsToParking(List<Vector3> newpath, Vector2 endTile, int fromDir)
    {

        /*
         * PARKING SPACES: 
         * Saved as integer 0-32
         * -> in binary 6 bits (6 slots)
         *  0: free, 1; occupied
        */

        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width


        List<Vector3> wayPoints = new List<Vector3>();

        int parkInt = grid.tileDatas[(int)endTile.x, (int)endTile.y].ParkingSpace;  //Get Parking Spaces

        if (parkInt == 32) return wayPoints; //No Parking Space available

        

        string binaryParking = System.Convert.ToString(parkInt, 2).PadLeft(6, '0');   //To Binary with 6 digits

        

        int targetSpace = -1;

        if (fromDir == 0 || fromDir == 2) //From left/down
        {
            //Check first 3 slots in string

            //string first3 = binaryParking.Substring(0, 3);

            for (int i = 0; i <= 2; ++i)
            {

                if (binaryParking[i] == '0')
                {
                    targetSpace = i;
                    break;
                }

            }
        }
        else if (fromDir == 1 || fromDir == 3) //From right/up
        {
            //Check last 3 slots in string

            for (int i = 3; i <= 5; ++i)
            {

                if (binaryParking[i] == '0')
                {   
                    targetSpace = i;
                    break;
                }
            }
        }

        

        if (targetSpace == -1) return wayPoints; //No Parking Space available in your lane

        //Gen Waypoints

        if (targetSpace == 0)
        {
            wayPoints.Add(new Vector3(-parkingDistanceX, 0, -width/2));
            wayPoints.Add(new Vector3(-parkingDistanceX, 0, -parkingDistanceY));
        }
        else if (targetSpace == 1)
        {
            wayPoints.Add(new Vector3(0, 0, -width/2));
            wayPoints.Add(new Vector3(0, 0, -parkingDistanceY));
        }
        else if (targetSpace == 2)
        {
            wayPoints.Add(new Vector3(parkingDistanceX, 0, -width/2));
            wayPoints.Add(new Vector3(parkingDistanceX, 0, -parkingDistanceY));
        }
        else if (targetSpace == 3)
        {
            wayPoints.Add(new Vector3(-parkingDistanceX, 0, width/2));
            wayPoints.Add(new Vector3(-parkingDistanceX, 0, parkingDistanceY));
        }
        else if (targetSpace == 4)
        {
            wayPoints.Add(new Vector3(0, 0, width/2));
            wayPoints.Add(new Vector3(0, 0, parkingDistanceY));
        }
        else if (targetSpace == 5)
        {
            wayPoints.Add(new Vector3(parkingDistanceX, 0, width/2));
            wayPoints.Add(new Vector3(parkingDistanceX, 0, parkingDistanceY));
        }



        if (fromDir == 2 || fromDir == 3)  //Vertical -> Rotate WayPoints
        {

            wayPoints = Rotate90(wayPoints);
        }


        Vector3 worldPos = grid.WorldPositionFromGridTile(endTile);


        //To WorldPos
        foreach (Vector3 vec in wayPoints)
        {

            newpath.Add(worldPos + vec);
        }




       


        

        binaryParking = binaryParking.Remove(targetSpace, 1).Insert(targetSpace, "1");


        

        parkInt = System.Convert.ToInt32(binaryParking, 2); //Back To Int

        grid.tileDatas[(int)endTile.x, (int)endTile.y].ParkingSpace = parkInt; //Set tile parking spaces

        
        

        return newpath;
    }









    List<Vector3> changeDirection(List<Vector3> curve)
    {

        List<Vector3> newCurve = new List<Vector3>();

        foreach (Vector3 vec in curve)
        {

            newCurve.Insert(0, vec);
        }
        return newCurve;

    }



    List<Vector3> convertCurveHorizontal(List<Vector3> curve)
    {

        List<Vector3> newCurve = new List<Vector3>();

        foreach (Vector3 vec in curve)
        {

            newCurve.Add(flipHorizontal(vec));
        }
        return newCurve;
    }
    List<Vector3> convertCurveVertical(List<Vector3> curve)
    {

        List<Vector3> newCurve = new List<Vector3>();

        foreach (Vector3 vec in curve)
        {

            newCurve.Add(flipVertical(vec));
        }
        return newCurve;
    }
    List<Vector3> convertCurveFlip(List<Vector3> curve)
    {

        List<Vector3> newCurve = new List<Vector3>();

        foreach (Vector3 vec in curve)
        {

            newCurve.Add(flipVertical(flipHorizontal(vec)));
        }
        return newCurve;
    }



    Vector3 flipVertical(Vector3 original)
    {

        return new Vector3(-original.x, original.y, original.z);
    }
    Vector3 flipHorizontal(Vector3 original)
    {
        return new Vector3(original.x, original.y, -original.z);

    }




    List<Vector3> Rotate90(List<Vector3> points)
    {

        List<Vector3> newPoints = new List<Vector3>();

        foreach (Vector3 vec in points)
        {

            newPoints.Add(new Vector3(-vec.z, vec.y, vec.x));
        }
        return newPoints;

    }
}