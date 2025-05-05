using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Foundation", order = 1)]

public class MeshFoundation : ScriptableObject
{

    //public float widthStreet = 2.5f;
    public float lengthStreet = 6;

    public float heightStreet = 0.1f;
    //public float widthSideStreet = 1.5f;

    public Color baseColor = Color.white;



    public Mesh GenerateMesh(GameObject go)
    {
       
        Mesh mesh = new Mesh();

        
        float length = lengthStreet / 4; //length = half of actual width
        float height = heightStreet / 4;
        

        Vector3[] vertices = new Vector3[20]
        {
            //Base

            new Vector3(-length, height, -length), new Vector3(-length, height, -length), new Vector3(-length, height, -length),
            new Vector3(length, height, -length), new Vector3(length, height, -length), new Vector3(length, height, -length),
            new Vector3(-length, height, length), new Vector3(-length, height, length), new Vector3(-length, height, length),
            new Vector3(length, height, length), new Vector3(length, height, length), new Vector3(length, height, length),




            new Vector3(-length, 0, -length), new Vector3(-length, 0, -length),
            new Vector3(length, 0, -length), new Vector3(length, 0, -length),
            new Vector3(-length, 0, length), new Vector3(-length, 0, length),
            new Vector3(length, 0, length), new Vector3(length, 0, length)


        };
        mesh.vertices = vertices;

        int[] tris = new int[30]
        {
            // lower base left triangle & upper base right triangle
            0, 6, 3,
            6, 9, 3,
            // Front
            12, 1, 14,
            1, 4, 14, 
            // Side
            13, 17, 2,
            17, 8, 2, 
            // Side
            5, 11, 15,
            11, 19, 15,
            // Back
            7, 16, 10,
            16, 18, 10,



        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[20]
        {
           
            Vector3.up,
            -Vector3.forward,
            -Vector3.right,
            Vector3.up,
            -Vector3.forward,
            Vector3.right,

           
            Vector3.up,
            Vector3.forward,     
            -Vector3.right,
            Vector3.up,
            Vector3.forward,
            Vector3.right,
            

            -Vector3.forward,
            -Vector3.right,
            -Vector3.forward,
            Vector3.right,
            Vector3.forward,
            -Vector3.right,
            Vector3.forward,
            Vector3.right


        };
        mesh.normals = normals;


        List<Color> colors = new List<Color>();

        for (int i = 0; i < vertices.Length; ++i) {

            colors.Add(baseColor);
        }
        mesh.colors = colors.ToArray();

        //meshFilter.mesh = mesh;

        return mesh;
    }
}
