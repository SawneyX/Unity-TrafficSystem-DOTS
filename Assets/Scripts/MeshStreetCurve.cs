using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CurveStreet", order = 1)]

public class MeshStreetCurve : ScriptableObject
{

    public float widthStreet = 2;
    public float lengthStreet = 4;

    public float heightStreet = 0.2f;
    public float widthSideStreet = 1f;

    public Color baseColor = new Color(0.6f, 0.6f, 0.6f); // Color.grey;
    public Color sideColor = Color.white;

    public float segmentRes = 0.1f; //0 - 0.3 | Smaller = Better Resolution  (If weird mesh -> lower minSegmentCount)
    public int minSegmentCount = 5;

    static float rightAngle = 0.5f * Mathf.PI;

    public List<Vector3> points;
   
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

        arcLength = rightAngle * (length + width);
        segmentCount1 = (int)(arcLength / segmentRes);
        int curvelength3 = ++segmentCount1;

        arcLength = rightAngle * (length + width + widthSide);
        segmentCount1 = (int)(arcLength / segmentRes);
        int curvelength4 = ++segmentCount1;



        //2 Inner base Curves
        points.AddRange(calculateCurveVertices(length - width, 0, segmentRes));
        points.AddRange(calculateCurveVertices(length + width, 0, segmentRes));

        //4 Top Curves
        points.AddRange(calculateCurveVertices(length - width - widthSide, height, segmentRes));
        points.AddRange(calculateCurveVertices(length - width, height, segmentRes));
        points.AddRange(calculateCurveVertices(length + width, height, segmentRes));
        points.AddRange(calculateCurveVertices(length + width + widthSide, height, segmentRes));

        //Inner Borders
        points.AddRange(calculateCurveVertices(length - width, height, segmentRes));
        points.AddRange(calculateCurveVertices(length - width, 0, segmentRes));

        points.AddRange(calculateCurveVertices(length + width, 0, segmentRes));
        points.AddRange(calculateCurveVertices(length + width, height, segmentRes));
        

        //Outer Border Curves
        points.AddRange(calculateCurveVertices(length - width - widthSide, 0, segmentRes));
        points.AddRange(calculateCurveVertices(length - width - widthSide, height, segmentRes));

        points.AddRange(calculateCurveVertices(length + width + widthSide, height, segmentRes));
        points.AddRange(calculateCurveVertices(length + width + widthSide, 0, segmentRes));



        Vector3[] verts = points.ToArray();
        mesh.vertices = verts;
        
        /*
        foreach (Vector3 v3 in verts)
        {
            GameObject test1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(test1);
            test1.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

            Instantiate(test1, v3, Quaternion.identity);
        }
        */



        int originIndex = 0;

        for (int i = 0; i < (verts.Length - 2) * 3; i++)
        {
            originIndex = 0;

            //Base Curve
            if (i < originIndex + curvelength2 + curvelength3 - 1)
            {

                AddTris(i, curvelength2, curvelength3, originIndex);

                continue;
            }

            originIndex = curvelength2 + curvelength3;

            
            //Inner SideWalkCurve
            if (i > originIndex - 1 && i < originIndex + curvelength1 + curvelength2 - 1)
            {

                AddTris(i, curvelength1, curvelength2, originIndex);

                continue;   

            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2;
           

            //Outer SideWalkCurve   
            if (i > originIndex - 1 && i < originIndex + curvelength3 + curvelength4 - 1) {
                
                AddTris(i, curvelength3, curvelength4, originIndex);
                
                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4;


            //Inner Borders
            if (i > originIndex - 1 && i < originIndex + 2 * curvelength2 - 1)
            {

                AddTris(i, curvelength2, curvelength2, originIndex);

                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2;

            if (i > originIndex - 1 && i < originIndex + 2 * curvelength3 - 1)
            {

                AddTris(i, curvelength3, curvelength3, originIndex);

                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3;



            //Outer Borders
            if (i > originIndex - 1 && i < originIndex + 2 * curvelength1 - 1)
            {

                AddTris(i, curvelength1, curvelength1, originIndex);

                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3 + 2 * curvelength1;

            if (i > originIndex - 1 && i < originIndex + 2 * curvelength4 - 1)
            {

                AddTris(i, curvelength4, curvelength4, originIndex);

                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3 + 2 * curvelength1 + 2 * curvelength4;

        }



        mesh.triangles = trisList.ToArray();
        /*
        int[] tris = new int[(verts.Length - 2) * 3];
        
        for (int i = 0; i < tris.Length / 6; i++) {

            if (i < curvelength1 - 1)
            {
                tris[i * 6 + 0] = i;
                tris[i * 6 + 1] = i + 1;
                tris[i * 6 + 2] = curvelength1 + i;

                tris[i * 6 + 3] = i + 1;
                tris[i * 6 + 4] = curvelength1 + i + 1;
                tris[i * 6 + 5] = curvelength1 + i;
            }
            

            //tris[i * 6 + 0] = i;
            //tris[i * 6 + 1] = i + 1;
            //tris[i * 6 + 2] = curvelength2 + i;

            //tris[i * 6 + 3] = i + 1;
            //tris[i * 6 + 4] = curvelength2 + i + 1;
            //tris[i * 6 + 5] = curvelength2 + i;

        }

        mesh.triangles = tris;
        */

        

        Vector3[] normals = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4) normals[i] = Vector3.up;

            else if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2) normals[i] = (verts[i] - new Vector3(-length, verts[i].y, -length)).normalized;
            else if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3) normals[i] = -(verts[i] - new Vector3(-length, verts[i].y, -length)).normalized;
            else if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3 + 2 * curvelength1) normals[i] = -(verts[i] - new Vector3(-length, verts[i].y, -length)).normalized;
            else if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3 + 2 * curvelength1 + 2 * curvelength4) normals[i] = (verts[i] - new Vector3(-length, verts[i].y, -length)).normalized; //new Vector3(-length, verts[i].y, -length)
        }
        mesh.normals = normals;


        for (int i = 0; i < verts.Length; i++) {

            if (i < curvelength2 + curvelength3) colorList.Add(baseColor);
            else colorList.Add(sideColor);
        }

      
        mesh.colors = colorList.ToArray();



        return mesh;
    }


    public Mesh GenerateMeshWithFoundation(GameObject go)
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

        arcLength = rightAngle * (length + width);
        segmentCount1 = (int)(arcLength / segmentRes);
        int curvelength3 = ++segmentCount1;

        arcLength = rightAngle * (length + width + widthSide);
        segmentCount1 = (int)(arcLength / segmentRes);
        int curvelength4 = ++segmentCount1;



        //2 Inner base Curves
        points.AddRange(calculateCurveVertices(length - width, 0, segmentRes));
        points.AddRange(calculateCurveVertices(length + width, 0, segmentRes));

        //4 Top Curves
        points.AddRange(calculateCurveVertices(length - width - widthSide, height, segmentRes));
        points.AddRange(calculateCurveVertices(length - width, height, segmentRes));
        points.AddRange(calculateCurveVertices(length + width, height, segmentRes));
        points.AddRange(calculateCurveVertices(length + width + widthSide, height, segmentRes));

        //Inner Borders
        points.AddRange(calculateCurveVertices(length - width, height, segmentRes));
        points.AddRange(calculateCurveVertices(length - width, 0, segmentRes));

        points.AddRange(calculateCurveVertices(length + width, 0, segmentRes));
        points.AddRange(calculateCurveVertices(length + width, height, segmentRes));


        //Outer Border Curves
        points.AddRange(calculateCurveVertices(length - width - widthSide, 0, segmentRes));
        points.AddRange(calculateCurveVertices(length - width - widthSide, height, segmentRes));

        //points.AddRange(calculateCurveVertices(length + width + widthSide, height, segmentRes));
        //points.AddRange(calculateCurveVertices(length + width + widthSide, 0, segmentRes));


        

        //FOUNDATION
        points.Add(new Vector3(width + widthSide, height, -length)); points.Add(new Vector3(width + widthSide, height, -length));
        points.Add(new Vector3(-length, height, width + widthSide)); points.Add(new Vector3(-length, height, width + widthSide));
        points.Add(new Vector3(width + widthSide, height, width + widthSide)); points.Add(new Vector3(width + widthSide, height, width + widthSide)); points.Add(new Vector3(width + widthSide, height, width + widthSide));  //height corner

        points.Add(new Vector3(width + widthSide, 0, -length));
        points.Add(new Vector3(-length, 0, width + widthSide));
        points.Add(new Vector3(width + widthSide, 0, width + widthSide)); points.Add(new Vector3(width + widthSide, 0, width + widthSide)); //ground corner

        points.Add(new Vector3(widthSide / 2, height, widthSide / 2));

        Vector3[] verts = points.ToArray();
        
        mesh.vertices = verts;

        



        int originIndex = 0;

        for (int i = 0; i < (verts.Length - 2 - 11) * 3; i++)
        {
            originIndex = 0;

            //Base Curve
            if (i < originIndex + curvelength2 + curvelength3 - 1)
            {

                AddTris(i, curvelength2, curvelength3, originIndex);

                continue;
            }

            originIndex = curvelength2 + curvelength3;


            //Inner SideWalkCurve
            if (i > originIndex - 1 && i < originIndex + curvelength1 + curvelength2 - 1)
            {

                AddTris(i, curvelength1, curvelength2, originIndex);

                continue;

            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2;


            //Outer SideWalkCurve   
            if (i > originIndex - 1 && i < originIndex + curvelength3 + curvelength4 - 1)
            {

                AddTris(i, curvelength3, curvelength4, originIndex);

                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4;


            //Inner Borders
            if (i > originIndex - 1 && i < originIndex + 2 * curvelength2 - 1)
            {

                AddTris(i, curvelength2, curvelength2, originIndex);

                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2;

            if (i > originIndex - 1 && i < originIndex + 2 * curvelength3 - 1)
            {

                AddTris(i, curvelength3, curvelength3, originIndex);

                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3;



            //Outer Borders
            if (i > originIndex - 1 && i < originIndex + 2 * curvelength1 - 1)
            {

                AddTris(i, curvelength1, curvelength1, originIndex);

                continue;
            }
            originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3 + 2 * curvelength1;

            /*
            if (i > originIndex - 1 && i < originIndex + 1 * curvelength4 - 1)
            {

                AddTris(i, curvelength4, curvelength4, originIndex);

                continue;
            }
            */
            //originIndex = curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3 + 2 * curvelength1 + 1 * curvelength4;
            
        }

        //FOUNDATION

        List<int> foundationTris = new List<int> { originIndex + 0, originIndex + 11, originIndex + 4, originIndex + 2, originIndex + 4, originIndex + 11, originIndex + 1, originIndex + 6, originIndex + 7, originIndex + 6, originIndex + 10, originIndex + 7, originIndex + 3, originIndex + 8, originIndex + 5, originIndex + 8, originIndex + 9, originIndex + 5 };

        trisList.AddRange(foundationTris);


        mesh.triangles = trisList.ToArray();
       

        Vector3[] normals = new Vector3[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4) normals[i] = Vector3.up;

            else if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2) normals[i] = (verts[i] - new Vector3(-length, verts[i].y, -length)).normalized;
            else if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3) normals[i] = -(verts[i] - new Vector3(-length, verts[i].y, -length)).normalized;
            else if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3 + 2 * curvelength1) normals[i] = -(verts[i] - new Vector3(-length, verts[i].y, -length)).normalized;
            //else if (i < curvelength2 + curvelength3 + curvelength1 + curvelength2 + curvelength3 + curvelength4 + 2 * curvelength2 + 2 * curvelength3 + 2 * curvelength1 + 2 * curvelength4) normals[i] = (verts[i] - new Vector3(-length, verts[i].y, -length)).normalized; //new Vector3(-length, verts[i].y, -length)

            else
            {
                normals[i] = Vector3.up; ++i;
                normals[i] = Vector3.right; ++i;
                normals[i] = Vector3.up; ++i;
                normals[i] = Vector3.forward; ++i;
                normals[i] = Vector3.up; ++i;
                normals[i] = Vector3.forward; ++i;
                normals[i] = Vector3.right; ++i;

                normals[i] = Vector3.right; ++i;
                normals[i] = Vector3.forward; ++i;
                normals[i] = Vector3.forward; ++i;
                normals[i] = Vector3.right; ++i;

                normals[i] = Vector3.up; ++i;

                break;
            }
            

        }

         



        mesh.normals = normals;


        for (int i = 0; i < verts.Length; i++)
        {

            if (i < curvelength2 + curvelength3) colorList.Add(baseColor);
            else colorList.Add(sideColor);
        }


        mesh.colors = colorList.ToArray();



        return mesh;
    }
















    void AddTris(int i, int curve1, int curve2, int startIndex) {


        int t = Mathf.Min(startIndex + curve1 - 1, i);

        
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








    public Vector3[] calculateCurveVertices(float radius, float height, float segmentLength) {


        float arcLength = rightAngle * radius;

        int segmentCount = (int) (arcLength / segmentLength);

        if (segmentCount < minSegmentCount) segmentCount = minSegmentCount; //Override min segment count
           

        float segmentAngle = rightAngle / segmentCount;

        Vector3[] curvepoints = new Vector3[segmentCount + 1];
        curvepoints[0] = new Vector3(-lengthStreet / 4 + radius, height, -lengthStreet/4); //First Point

        for (int i = 1; i <= segmentCount; i++)
        {
            float angle = i * segmentAngle;
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);

            x = -((lengthStreet / 4)) + x;   //Koordinatenwechsel (Origin in Mitte)
            z = -((lengthStreet / 4)) + z;

            curvepoints[i] = new Vector3(x, height, z);
            
        }

        curvepoints[curvepoints.Length - 1] = new Vector3(-lengthStreet/4, height, -lengthStreet/4 + radius); //Last Point
        
        return curvepoints;
    }





    /*
    public List<Vector3> GenerateWaypoints(bool right)
    {


        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width
        float height = heightStreet / 4;
        float widthSide = widthSideStreet / 2;

        List<Vector3> wayPoints = new List<Vector3>();

        if (right)
        {
            wayPoints.AddRange(calculateCurveVertices(length + width / 2, 0, segmentRes));
        }

        else
        {

            wayPoints.AddRange(calculateCurveVertices(length - width / 2, 0, segmentRes));

        }

        return wayPoints;

    }


    */



}
