using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Obligatory components for the mesh
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour
{
    public int xSize, ySize, zSize;
    public int roundness;

    // Store the vetex's coordinates
    private Vector3[] vertices;
    // Store normal vectors
    private Vector3[] normals;
    private Mesh mesh;

    // We are stealing the Color Chanel (4D) to store our primaly cube coordinates in it
    private Color32[] cubeUV; 

    private void Awake()
    {
        Generate();

    }

    private void Generate()
    {

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Cube";
        CreateVertices();
        CreateTriangles();
        CreateColliders();

    }

    private void CreateColliders()
    {   
        // Rectangular collider (Box)
        AddBoxCollider(xSize, ySize - roundness * 2, zSize - roundness * 2);
		AddBoxCollider(xSize - roundness * 2, ySize, zSize - roundness * 2);
        AddBoxCollider(xSize - roundness * 2, ySize - roundness * 2, zSize);

        // Capsule colliders
        Vector3 min = Vector3.one * roundness;
		Vector3 half = new Vector3(xSize, ySize, zSize) * 0.5f; 
		Vector3 max = new Vector3(xSize, ySize, zSize) - min;

		AddCapsuleCollider(0, half.x, min.y, min.z);
		AddCapsuleCollider(0, half.x, min.y, max.z);
		AddCapsuleCollider(0, half.x, max.y, min.z);
		AddCapsuleCollider(0, half.x, max.y, max.z);
		
		AddCapsuleCollider(1, min.x, half.y, min.z);
		AddCapsuleCollider(1, min.x, half.y, max.z);
		AddCapsuleCollider(1, max.x, half.y, min.z);
		AddCapsuleCollider(1, max.x, half.y, max.z);
		
		AddCapsuleCollider(2, min.x, min.y, half.z);
		AddCapsuleCollider(2, min.x, max.y, half.z);
		AddCapsuleCollider(2, max.x, min.y, half.z);
		AddCapsuleCollider(2, max.x, max.y, half.z);
        

    }

    // Create an instance of a boxCollider
    private void AddBoxCollider(float x, float y, float z)
    {
        BoxCollider c = gameObject.AddComponent<BoxCollider>();
        // Scale it 
        c.size = new Vector3(x, y, z);
    }

    private void AddCapsuleCollider(int direction, float x, float y, float z)
    {
        CapsuleCollider c = gameObject.AddComponent<CapsuleCollider>();
        // Changes the orientation center
        c.center = new Vector3(x, y, z);
        c.direction = direction;
        c.radius = roundness;
        c.height = c.center[direction] * 2f;
    }


    // FILL THE VERTICES ARRAY
    private void CreateVertices()
    {
        // Defining the num of vertices
        int cornerVertices = 8;
        // Each edge has an amount of vertices equal to the corresponding size minus one
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];
        cubeUV = new Color32[vertices.Length];

        // Creates the vertices in a circular layer order

        int v = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {

                SetVertex(v++, x, y, 0);

            }
            for (int z = 1; z <= zSize; z++)
            {
                SetVertex(v++, xSize, y, z);

            }
            for (int x = xSize - 1; x >= 0; x--)
            {
                SetVertex(v++, x, y, zSize);

            }
            for (int z = zSize - 1; z > 0; z--)
            {
                SetVertex(v++, 0, y, z);

            }
        }

        // Fill the top and bottom cap
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                SetVertex(v++, x, ySize, z);

            }
        }
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                SetVertex(v++, x, 0, z);

            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
    }

    // Create the vertices and set the normals for the rounded cube
    private void SetVertex(int i, int x, int y, int z)
    {
        Vector3 inner = vertices[i] = new Vector3(x, y, z);

        //------ X direction --------
        // If we are on the left side
        if (x < roundness)
        {
            inner.x = roundness;
        }
        // If we are on the right side
        else if (x > xSize - roundness)
        {
            inner.x = xSize - roundness;
        }
        //------------------------------

        //-------- Y direction ---------
        // If we are on the left side
        if (y < roundness)
        {
            inner.y = roundness;
        }
        // If we are on the right side
        else if (y > ySize - roundness)
        {
            inner.y = ySize - roundness;
        }
        //------------------------------

        //-------- Z direction ---------
        // If we are on the left side
        if (z < roundness)
        {
            inner.z = roundness;
        }
        // If we are on the right side
        else if (z > zSize - roundness)
        {
            inner.z = zSize - roundness;
        }
        //------------------------------

        normals[i] = (vertices[i] - inner).normalized;
        vertices[i] = inner + normals[i] * roundness;
        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }

    // FILL THE TRIANGLES ARRAY
    private void CreateTriangles()
    {
        // Slipt the mesh into three pairs of opposite face
        int[] trianglesZ = new int[(xSize * ySize) * 12];
		int[] trianglesX = new int[(ySize * zSize) * 12];
		int[] trianglesY = new int[(xSize * zSize) * 12];
        int ring = (xSize + zSize) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;


        // Loops throught the rings and fills them with quads  
        for (int y = 0; y < ySize; y++, v++)
        {

            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zSize; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < xSize; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < zSize - 1; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            // Its second and fourth vertex need to rewind to the start of the ring
            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);

        }

        // top and bottom faces simply use the Y array
        // Fills the top cap
        tY = CreateTopFace(trianglesY, tY, ring);
        // Fills the bottom cap
        tY = CreateBottomFace(trianglesY, tY, ring);
        
        // Assign 3 different submeshes with 3 different materials
        mesh.subMeshCount = 3;
		mesh.SetTriangles(trianglesZ, 0);
		mesh.SetTriangles(trianglesX, 1);
		mesh.SetTriangles(trianglesY, 2);
    }

    private int CreateTopFace(int[] triangles, int t, int ring)
    {
        // ---- Draws the bottom border -------
        int v = ring * ySize;
        for (int x = 0; x < xSize - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        // The final quad's fourth vertex is different though, as that's where the ring bends upwards
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);
        // -------------------------------------

        // Keeps track if the quad is being drawn at the border or in the middle
        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        //------ Draws the middle part ---------
        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
            }
            // The last quad of the row once again has to deal with the outer ring
            t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
        }
        //----------------------------------------

        // Sets the first quad of the last row
        int vTop = vMin - 2;

        //------ Draws the top border ---------
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);
        // -------------------------------------

        return t;
    }

    private int CreateBottomFace(int[] triangles, int t, int ring)
    {

        int v = 1;

        // ---- Draws the bottom border -------
        int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xSize - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);
        //----------------------------------------

        int vMin = ring - 2;
        vMid -= xSize - 2;
        int vMax = v + 2;

        //------ Draws the middle part ---------
        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
        }
        //----------------------------------------

        int vTop = vMin - 1;

        //------ Draws the top border ---------
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
        //----------------------------------------

        return t;
    }

    // Method to draw quads
    private static int
    SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
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
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            Vector3 worldNormal = transform.TransformDirection(normals[i]);

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(worldPos, 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(worldPos, worldNormal);
        }
    }

}
