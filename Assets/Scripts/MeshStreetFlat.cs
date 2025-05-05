using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FlatStreet", order = 1)]

public class MeshStreetFlat : ScriptableObject
{
    public float widthStreet = 2.5f;
    public float lengthStreet = 6;

    public float heightStreet = 0.23f;
    public float widthSideStreet = 1.5f;

    public Color baseColor = new Color(0.6f, 0.6f, 0.6f); // Color.grey;
    public Color sideColor = Color.white;






    public Mesh GenerateMesh(GameObject go)
    {
        //GameObject go = new GameObject();
        //MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Shader Graphs/RoadShader")); //"Universal Render Pipeline/Lit"
        //MeshFilter meshFilter = go.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width
        float height = heightStreet / 4;
        float widthSide = widthSideStreet / 2;

        Vector3[] vertices = new Vector3[28]
        {
            //Base  (double for sharp edge)
            new Vector3(-width, 0, -length),  new Vector3(-width, 0, -length),
            new Vector3(width, 0, -length), new Vector3(width, 0, -length),
            new Vector3(-width, 0, length), new Vector3(-width, 0, length),
            new Vector3(width, 0, length), new Vector3(width, 0, length),

            //Left (double for sharp edge)
            new Vector3(-(width+widthSide), height, -length), new Vector3(-(width+widthSide), height, -length),
            new Vector3(-width, height, -length),  new Vector3(-width, height, -length),
            new Vector3(-(width+widthSide), height, length), new Vector3(-(width+widthSide), height, length),
            new Vector3(-width, height, length), new Vector3(-width, height, length),
            
            //Right (double for sharp edge)
            new Vector3(width, height, -length), new Vector3(width, height, -length),
            new Vector3(width + widthSide, height, -length), new Vector3(width + widthSide, height, -length),
            new Vector3(width, height, length), new Vector3(width, height, length),
            new Vector3(width + widthSide, height, length), new Vector3(width + widthSide, height, length),

            //Border Left
            new Vector3(-(width+widthSide), 0, -length),
            new Vector3(-(width+widthSide), 0, length),

            //Border Right
            new Vector3(width + widthSide, 0, -length),
            new Vector3(width + widthSide, 0, length)


        };
        mesh.vertices = vertices;

        int[] tris = new int[42]
        {
            // lower base left triangle & upper base right triangle
            0, 4, 2,
            4, 6, 2,
            // left Side
            8, 12, 10,
            12, 14, 10, 
            // right Side
            16, 20, 18,
            20, 22, 18,
            // left inner border
            11, 15, 1,
            15, 5, 1,
            //left outer border
            24, 25, 9,
            25, 13, 9,
            //right inner border
            3, 7, 17,
            7, 21, 17,
            //right outer border
            19, 23, 26,
            23, 27, 26



        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[28]
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

            //Left Side
            Vector3.up,
            -Vector3.right,
            Vector3.up,
            Vector3.right,
            Vector3.up,
            -Vector3.right,
            Vector3.up,
            Vector3.right,

            //Right Side

            Vector3.up,
            -Vector3.right,
            Vector3.up,
            Vector3.right,
            Vector3.up,
            -Vector3.right,
            Vector3.up,
            Vector3.right,

            //4 Border Verts

            -Vector3.right,
            -Vector3.right,
            Vector3.right,
            Vector3.right


        };
        mesh.normals = normals;


        Color[] colors = new Color[28] {

            //Base
            baseColor,
            sideColor,
            baseColor,
            sideColor,
            baseColor,
            sideColor,
            baseColor,
            sideColor,

            //Left
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
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

            //4 Border Verts
            sideColor,
            sideColor,
            sideColor,
            sideColor

        };
        mesh.colors = colors;


        Vector2[] uv = new Vector2[28]
        {
            //Base
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),

            //Left Side
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            //Right Side
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),

            //Borders
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0)


        };
        mesh.uv = uv;

        //meshFilter.mesh = mesh;

        return mesh;
    }






    public List<Vector3> GenerateWaypoints(bool right) {


        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width
        float height = heightStreet / 4;
        float widthSide = widthSideStreet / 2;

        List<Vector3> wayPoints = new List<Vector3>();

        if (right)
        {
            
            wayPoints.Add(new Vector3(width / 2, 0, length));
        }

        else {

            wayPoints.Add(new Vector3(-width / 2, 0, -length));
           
        }

        return wayPoints;

    }






}
