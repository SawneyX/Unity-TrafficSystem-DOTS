using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Cross3Street", order = 1)]

public class MeshStreet3Cross : ScriptableObject
{

    public float widthStreet = 2.5f;
    public float lengthStreet = 6;

    public float heightStreet = 0.23f;
    public float widthSideStreet = 1.5f;

    public Color baseColor = new Color(0.6f, 0.6f, 0.6f); // Color.grey;
    public Color sideColor = Color.white;


    public float segmentRes = 0.1f; //0 - 0.3 | Smaller = Better Resolution  (If weird mesh -> lower minSegmentCount)
    public int minSegmentCount = 5;

    public List<Vector3> points;

    static float rightAngle = 0.5f * Mathf.PI;

    List<int> trisList = new List<int>();
    List<Color> colorList = new List<Color>();



    public Mesh GenerateMesh(GameObject go)
    {

        Mesh mesh = new Mesh();
        points.Clear();

        trisList.Clear();
        colorList.Clear();

        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width
        float height = heightStreet / 4;
        float widthSide = widthSideStreet / 2;

        segmentRes = Mathf.Min(segmentRes, 0.3f);

        float arcLength = rightAngle * (length - width - widthSide);
        int segmentCount1 = (int)(arcLength / segmentRes);
        if (segmentCount1 < minSegmentCount) segmentCount1 = minSegmentCount;
        int curvelength1 = ++segmentCount1;

        arcLength = rightAngle * (length - width);
        segmentCount1 = (int)(arcLength / segmentRes);
        int curvelength2 = ++segmentCount1;



        //Base  (double for sharp edge)
        points.Add(new Vector3(-width, 0, -length)); points.Add(new Vector3(-width, 0, -length));
        points.Add(new Vector3(width, 0, -length)); points.Add(new Vector3(width, 0, -length));
        points.Add(new Vector3(-width, 0, length)); points.Add(new Vector3(-width, 0, length));
        points.Add(new Vector3(width, 0, length)); points.Add(new Vector3(width, 0, length));

        //Right (double for sharp edge)
        points.Add(new Vector3(width, height, -length)); points.Add(new Vector3(width, height, -length));
        points.Add(new Vector3(width + widthSide, height, -length)); points.Add(new Vector3(width + widthSide, height, -length));
        points.Add(new Vector3(width, height, length)); points.Add(new Vector3(width, height, length));
        points.Add(new Vector3(width + widthSide, height, length)); points.Add(new Vector3(width + widthSide, height, length));

        //Border Right
        points.Add(new Vector3(width + widthSide, 0, -length));
        points.Add(new Vector3(width + widthSide, 0, length));

           
        

        //Inner 2 Curves (height = 0)

        points.AddRange(calculateCurveVertices(length - width, 0, segmentRes, 1));
        points.AddRange(calculateCurveVertices(length - width, 0, segmentRes, -1));

        //Top 4 Curves (height != 0)

        points.AddRange(calculateCurveVertices(length - width - widthSide, height, segmentRes, 1));
        points.AddRange(calculateCurveVertices(length - width, height, segmentRes, 1));

        points.AddRange(calculateCurveVertices(length - width - widthSide, height, segmentRes, -1));
        points.AddRange(calculateCurveVertices(length - width, height, segmentRes, -1));


        //Borders Lower Part
        points.AddRange(calculateCurveVertices(length - width - widthSide, 0, segmentRes, 1));
        points.AddRange(calculateCurveVertices(length - width - widthSide, height, segmentRes, 1));

        points.AddRange(calculateCurveVertices(length - width, height, segmentRes, 1));
        points.AddRange(calculateCurveVertices(length - width, 0, segmentRes, 1));

        


        //Borders Upper Part
        
        points.AddRange(calculateCurveVertices(length - width - widthSide, 0, segmentRes, -1));
        points.AddRange(calculateCurveVertices(length - width - widthSide, height, segmentRes, -1));

        points.AddRange(calculateCurveVertices(length - width, height, segmentRes, -1));
        points.AddRange(calculateCurveVertices(length - width, 0, segmentRes, -1));
        

        Vector3[] verts = points.ToArray();
        mesh.vertices = verts;

        int[] tris = new int[24]
        {
            // lower base left triangle & upper base right triangle
            0, 4, 2,
            4, 6, 2,
            // Right Side
            8, 12, 10,
            12, 14, 10, 
            
            // Right inner border
            3, 7, 9,
            7, 13, 9,
            //Right outer border
            11, 15, 16,
            15, 17, 16,
            
        };

        trisList.AddRange(tris);

        //Curve Part
        int startVects = 18;
        int originIndex = startVects;

        for (int i = startVects; i < (verts.Length - 2) * 3; i++)
        {
            originIndex = startVects;

            //Base Curve
            if (i > originIndex - 1 && i < originIndex + curvelength2 - 1)
            {

                AddTris(i, curvelength2, curvelength2, originIndex, 1);

                continue;
            }

            originIndex = startVects + 2 * curvelength2;


            //Top Curves
            if (i > originIndex - 1 && i < originIndex + curvelength1 + curvelength2 - 1)
            {

                AddTris(i, curvelength1, curvelength2, originIndex, 1);

                continue;
            }
            originIndex = startVects + 2 * curvelength2 + curvelength1 + curvelength2;
            
            if (i > originIndex - 1 && i < originIndex + curvelength1 + curvelength2 - 1)
            {

                AddTris(i, curvelength1, curvelength2, originIndex, -1);

                continue;
            }

            originIndex = startVects + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2;


            //Borders Lower Part
            if (i > originIndex - 1 && i < originIndex + 2 * curvelength1 - 1)
            {

                AddTris(i, curvelength1, curvelength1, originIndex, 1);

                continue;
            }
            originIndex = startVects + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1;

            if (i > originIndex - 1 && i < originIndex + 2 * curvelength2 - 1)
            {

                AddTris(i, curvelength2, curvelength2, originIndex, 1);

                continue;
            }
            originIndex = startVects + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2;


            //Borders Upper Part
            if (i > originIndex - 1 && i < originIndex + 2 * curvelength1 - 1)
            {

                AddTris(i, curvelength1, curvelength1, originIndex, -1);

                continue;
            }
            originIndex = startVects + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1; 

            if (i > originIndex - 1 && i < originIndex + 2 * curvelength2 - 1)
            {

                AddTris(i, curvelength2, curvelength2, originIndex, -1);

                continue;
            }
            originIndex = startVects + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2;




        }

        mesh.triangles = trisList.ToArray();

        Vector3[] normals1 = new Vector3[18]
        {
            //Flat
            Vector3.up,
            Vector3.right,
            Vector3.up,
            -Vector3.right,
            Vector3.up,
            Vector3.right,
            Vector3.up,
            -Vector3.right,

            //Right Side

            Vector3.up,
            -Vector3.right,
            Vector3.up,
            Vector3.right,
            Vector3.up,
            -Vector3.right,
            Vector3.up,
            Vector3.right,

            
            //2 Border Verts

            Vector3.right,
            Vector3.right,
            
        };

        Vector3[] normals = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            if (i < normals1.Length) normals[i] = normals1[i];


            else if (i < normals1.Length + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2) normals[i] = Vector3.up;

            else if (i < normals1.Length + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1) normals[i] = -(verts[i] - new Vector3(-length, verts[i].y, -length)).normalized;
            else if (i < normals1.Length + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2) normals[i] = (verts[i] - new Vector3(-length, verts[i].y, -length)).normalized;
            else if (i < normals1.Length + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1) normals[i] = -(verts[i] - new Vector3(-length, verts[i].y, length)).normalized;
            else if (i < normals1.Length + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2 + 2 * curvelength1 + 2 * curvelength2) normals[i] = (verts[i] - new Vector3(-length, verts[i].y, length)).normalized;
            
        }



        mesh.normals = normals;


        Color[] colors1 = new Color[18] {

            //Base
            baseColor,
            sideColor,
            baseColor,
            sideColor,
            baseColor,
            sideColor,
            baseColor,
            sideColor,

            //Right
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,

          
            //2 Border Verts
            sideColor,
            sideColor
            

        };

        Color[] colors = new Color[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            if (i < colors1.Length) colors[i] = colors1[i];
            else if (i < colors1.Length + 2 * curvelength2) colors[i] = baseColor;
            else colors[i] = sideColor;
        }


        mesh.colors = colors;



        //meshFilter.mesh = mesh;

        return mesh;
    }


    void AddTris(int i, int curve1, int curve2, int startIndex, int factor)
    {


        int t = Mathf.Min(startIndex + curve1 - 1, i);

        if (factor == 1)
        {

            if (t < startIndex + curve1 - 1)
            {

                trisList.Add(t);
                trisList.Add(t + 1);
                trisList.Add(t + curve1);

                trisList.Add(t + 1);
                trisList.Add(t + curve1 + 1);
                trisList.Add(t + curve1);

            }

            else if (i > t + curve1 - 1 && i < startIndex + curve1 + curve2)
            {
                trisList.Add(t);
                trisList.Add(i + 1);
                trisList.Add(i);

            }
        }

        else {

            if (t < startIndex + curve1 - 1)
            {
                trisList.Add(t + curve1);
                trisList.Add(t + 1);    
                trisList.Add(t);


                trisList.Add(t + curve1);    
                trisList.Add(t + curve1 + 1);
                trisList.Add(t + 1);

            }

            else if (i > t + curve1 - 1 && i < startIndex + curve1 + curve2)
            {
                trisList.Add(i);              
                trisList.Add(i + 1);
                trisList.Add(t);

            }

        }


    }





    public Vector3[] calculateCurveVertices(float radius, float height, float segmentLength, int factor)
    {


        float arcLength = rightAngle * radius;

        int segmentCount = (int)(arcLength / segmentLength);

        if (segmentCount < minSegmentCount) segmentCount = minSegmentCount; //Override min segment count


        float segmentAngle = rightAngle / segmentCount;

        Vector3[] curvepoints = new Vector3[segmentCount + 1];
        curvepoints[0] = new Vector3(-lengthStreet / 4 + radius, height, factor * -lengthStreet / 4); //First Point

        for (int i = 1; i <= segmentCount; i++)
        {
            float angle = i * segmentAngle;
            float x = radius * Mathf.Cos(angle);
            float z = factor * radius * Mathf.Sin(angle);

            x = -((lengthStreet / 4)) + x;   //Koordinatenwechsel (Origin in Mitte)
            z = factor * -((lengthStreet / 4)) + z;

            curvepoints[i] = new Vector3(x, height, z);

        }

        curvepoints[curvepoints.Length - 1] = new Vector3(-lengthStreet / 4, height, factor * -lengthStreet / 4 + factor * radius); //Last Point

        return curvepoints;
    }








    /*
    public List<Vector3> GenerateWaypoints(bool right, int direction)
    {


        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width
        float height = heightStreet / 4;
        float widthSide = widthSideStreet / 2;

        List<Vector3> wayPoints = new List<Vector3>();

        if (right)
        {


            wayPoints.Add(new Vector3(width / 2, 0, length));
            //wayPoints.AddRange(calculateCurveVertices(length + width / 2, 0, segmentRes));
        }

        else if (right)
        {

            wayPoints.Add(new Vector3(-width / 2, 0, -length));

        }
        else if (right) {

            wayPoints.AddRange(calculateCurveVertices(length + width / 2, 0, segmentRes, 0));
        }

        else


        return wayPoints;

    }

    */




}
