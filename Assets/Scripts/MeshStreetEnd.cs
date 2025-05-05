using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EndStreet", order = 1)]

public class MeshStreetEnd : ScriptableObject
{

    public float widthStreet = 2;
    public float lengthStreet = 4;

    public float heightStreet = 0.2f;
    public float widthSideStreet = 1f;

    public Color baseColor = new Color(0.6f, 0.6f, 0.6f); // Color.grey;
    public Color sideColor = Color.white;



    public Mesh GenerateMesh(GameObject go) {

        Mesh mesh = new Mesh();

        float width = widthStreet / 4;  //width = half of actual width
        float length = lengthStreet / 4; //length = half of actual width
        float height = heightStreet / 4;
        float widthSide = widthSideStreet / 2;


        Vector3[] vertices = new Vector3[44]
        {
            //Base  (double for sharp edge)
            new Vector3(-width, 0, -(length - widthSide)),  new Vector3(-width, 0, -(length - widthSide)),  new Vector3(-width, 0, -(length - widthSide)),
            new Vector3(width, 0, -(length - widthSide)), new Vector3(width, 0, -(length - widthSide)), new Vector3(width, 0, -(length - widthSide)),
            new Vector3(-width, 0, length), new Vector3(-width, 0, length),
            new Vector3(width, 0, length), new Vector3(width, 0, length),

            //Under (double for sharp edge)
            new Vector3(-(width+widthSide), height, -length), new Vector3(-(width+widthSide), height, -length), new Vector3(-(width+widthSide), height, -length),
            new Vector3(-width, height, -length),  new Vector3(-width, height, -length),
            new Vector3(width, height, -length), new Vector3(width, height, -length),
            new Vector3(width + widthSide, height, -length),  new Vector3(width + widthSide, height, -length), new Vector3(width + widthSide, height, -length),

            //Middle
            new Vector3(-(width+widthSide), height, -(length-widthSide)), new Vector3(-(width+widthSide), height, -(length-widthSide)),
            new Vector3(-width, height, -(length-widthSide)),  new Vector3(-width, height, -(length-widthSide)), new Vector3(-width, height, -(length-widthSide)),
            new Vector3(width, height, -(length-widthSide)), new Vector3(width, height, -(length-widthSide)), new Vector3(width, height, -(length-widthSide)),
            new Vector3(width + widthSide, height, -(length-widthSide)),  new Vector3(width + widthSide, height, -(length-widthSide)),

            //Top
            new Vector3(-(width+widthSide), height, length), new Vector3(-(width+widthSide), height, length),
            new Vector3(-width, height, length),  new Vector3(-width, height, length),
            new Vector3(width, height, length), new Vector3(width, height, length),
            new Vector3(width + widthSide, height, length),  new Vector3(width + widthSide, height, length),


            //Borders
            new Vector3(-(width+widthSide), 0, -length), new Vector3(-(width+widthSide), 0, -length),
            new Vector3(width + widthSide, 0, -length), new Vector3(width + widthSide, 0, -length),
            new Vector3(-(width+widthSide), 0, length),
            new Vector3(width + widthSide, 0, length)


        };
        mesh.vertices = vertices;

        int[] tris = new int[72]  //24*3
        {
            // lower base left triangle & upper base right triangle
            0, 6, 3,
            6, 8, 3,
            // Under
            10, 20, 13,
            20, 22, 13, 
            13, 22, 15,
            22, 25, 15,
            15, 25, 17, 
            25, 28, 17,

            //left
            20, 30, 22,
            30, 32, 22,

            //right
            25, 34, 28,
            34, 36, 28,

            // middle inner border
            24, 2, 27, 
            2, 5, 27, 

            //middle outer border
            39, 12, 41, 
            12, 19, 41,

            // left inner border
            23, 33, 1,
            33, 7, 1,
            //left outer border
            38, 42, 11,
            42, 31, 11,
            //right inner border
            4, 9, 26,
            9, 35, 26,
            //right outer border
            18, 37, 41,
            37, 43, 41


        };
        mesh.triangles = tris;


        Vector3[] normals = new Vector3[44]
        {
            //Flat
            Vector3.up,
            Vector3.right,
            Vector3.forward,
            Vector3.up,
            -Vector3.right,
            Vector3.forward,
            Vector3.up,
            Vector3.right,
            Vector3.up,
            -Vector3.right,

            //Under
            Vector3.up,
            -Vector3.right,
            -Vector3.forward,
            Vector3.up,
            -Vector3.forward,
            Vector3.up,
            -Vector3.forward,
            Vector3.up,
            Vector3.right,
            -Vector3.forward,

            //Middle

            Vector3.up,
            -Vector3.right,
            Vector3.up,
            Vector3.right,
            Vector3.forward,
            Vector3.up,
            -Vector3.right,
            Vector3.forward,
            Vector3.up,
            Vector3.right,

            //Top
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
            -Vector3.forward,
            Vector3.right,
            -Vector3.forward,
            -Vector3.right,
            Vector3.right


        };
        mesh.normals = normals;

        Color[] colors = new Color[44] {

            //Base
            baseColor,
            sideColor,
            sideColor,
            baseColor,
            sideColor,
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
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
            sideColor,
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



        return mesh;
    }







}
