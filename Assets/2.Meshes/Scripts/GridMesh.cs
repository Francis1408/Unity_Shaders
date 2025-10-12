using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Obligatory components for the mesh
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridMesh : MonoBehaviour
{

    public int xSize, ySize;

    // Store the vetex's coordinates
    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
        Generate();

    }

    private void Generate()
    {

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        // Num vertices :  (#x+1)(#y+1)
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];


        // Vector to map de texture
        Vector2[] uv = new Vector2[vertices.Length];
        // Adds tangents to orient the normal vector
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }

        // Write the info in the mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;
//
//      The triangles are defined in a clockwise or counter-clockwise order:
//
//      First triangle:  (0, 2, 1) // Clockwise
//      Second triangle: (1, 3, 2) // Counter-Clockwise


        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                // First Triangle
                triangles[ti] = vi;
                triangles[ti + 2] = vi + 1;
                triangles[ti + 1] = vi + xSize + 1;

                // Second Triangle
                triangles[ti + 3] = vi + 1;
                triangles[ti + 5] = vi + xSize + 2;
                triangles[ti + 4] = vi + xSize + 1;


            }

        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }

    private void OnDrawGizmos()
    {
        // Avoid to draw null on edit mode
        if (vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
        }
    }

}
