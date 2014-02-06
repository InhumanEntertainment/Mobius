using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobiusStrip : MonoBehaviour
{
    public List<Vector3> Tail = new List<Vector3>();
    public Vector3 Velocity = Vector3.zero;
    public Vector3 Target = Vector3.up;

    public float TailWidthStart = 1f;
    public float TailWidthEnd = 1f;
    public bool DrawWireframe = false;

    //============================================================================================================================================//
    List<Vector3> loop = new List<Vector3>();
    Vector3 looptarget;
    int pos = 0;
    int looplength = 50;

    //============================================================================================================================================//
    void Start()
    {
        for (int i = 0; i < looplength; i++)
        {
            float v = (float)i / looplength * Mathf.Deg2Rad * 360;
            float x = Mathf.Sin(v) * 0.4f;
            float y = Mathf.Cos(v) * 0.4f;

            loop.Add(new Vector3(x, y, 0));
        }
    }

    //============================================================================================================================================//
    int GetDistance(int index)
    {
        int a = Mathf.Abs(pos - index);
        int b = looplength - pos + index;
        //print(a + " : " + b + " = " + Mathf.Min(a, b));

        return Mathf.Min(a, b);
    }

    //============================================================================================================================================//
    int index(int i)
    {
        if (i < 0)
        {
            return looplength + i;
        }
        else if (i >= looplength)
        {
            return i - looplength;
        }
        else
        {
            return i;
        }
    }

    //============================================================================================================================================//
    int GetClosestPoint(Vector3 position)
    {
        int closest = 0;
        float closestdistance = Vector3.Distance(position, loop[0]);

        for (int i = 1; i < looplength; i++)
        {
            float distance = Vector3.Distance(position, loop[i]);
            if (distance < closestdistance)
            {
                closestdistance = distance;
                closest = i;
            }
        }

        return closest;
    }

    //============================================================================================================================================//
    void Update()
    {
        // Get Game Camera //
        Camera cam = Camera.main;
        var w = cam.pixelWidth;
        var h = cam.pixelHeight;


        // Logic //
        if (Input.GetMouseButtonDown(0) && !GameButton.InUse)
        {
            looptarget = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            looptarget.z = 0;
            pos = GetClosestPoint(looptarget);
        }
        if (Input.GetMouseButton(0) && !GameButton.InUse)
        {
            looptarget = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            looptarget.z = 0;

            // For each point on loop //
            for (int i = 0; i < looplength; i++)
            {
                float pointamount = 1 - Mathf.Min((float)GetDistance(i) / looplength * 30, 1);
                pointamount = Mathf.Pow(pointamount, 2);

                Vector3 vec = looptarget - loop[i];
                vec.Normalize();

                float distance = Vector3.Distance(looptarget, loop[i]);
                float force = 0;
                if (distance < 2)
                {
                    force = (2f - distance) / 2f;
                    force = (float)Mathf.Pow(force, 1);
                    //force *= (1f * pointamount);

                    /*if (force > 0.7f)
                    {
                        force = 0.7f;
                    }*/

                    if (pointamount > 0.05f)
                    {
                        loop[i] = looptarget;
                    }
                    else
                    {
                        loop[i] += pointamount * vec;
                    
                    }
                }
            }
        }

        // Smooth Curve //
        List<Vector3> buffer = new List<Vector3>(loop);

        for (int i = 0; i < looplength; i++)
        {
            Vector3 vec1 = loop[i] - loop[index(i - 3)];
            vec1.Normalize();
            float distance1 = (float)0.04f - Vector3.Distance(loop[index(i - 3)], loop[i]) / 4;
            distance1 = Mathf.Max(0, distance1);

            Vector3 vec2 = loop[i] - loop[index(i + 3)];
            vec2.Normalize();
            float distance2 = (float)0.04f - Vector3.Distance(loop[index(i + 3)], loop[i]) / 4;
            distance2 = Mathf.Max(0, distance2);

            Vector3 force = (vec1 * distance1 + vec2 * distance2) * 1f;

            buffer[i] = loop[i] * 0.6f + loop[index(i - 1)] * 0.2f + loop[index(i + 1)] * 0.2f;
            buffer[i] += force;
        }
        loop = buffer;

        // Draw //
        for (int i = 0; i < loop.Count; i++)
        {
            float pointamount = 1 - Mathf.Min((float)GetDistance(i) / looplength * 8, 1);
            pointamount = Mathf.Pow(pointamount, 2);

            if (i == loop.Count - 1)
            {
                Debug.DrawLine(loop[i], loop[0], new Color(pointamount, 1, 1));
            }
            else
                Debug.DrawLine(loop[i], loop[i + 1], new Color(pointamount, 1, 1));

        }

        /*pos++;
        if (pos == looplength)
        {
            pos = 0;
        }*/
        Tail = new List<Vector3>(loop);
        Tail.Add(Tail[0]);
        BuildMesh();
    }

    //============================================================================================================================================//
    void OnDrawGizmos()
    {

    }

    //============================================================================================================================================//
    float UVScroll = 0;
    
    void BuildMesh()
    {
        if (Tail.Count > 1)
        {
            // Then create triangles //
            Vector3[] vertices = new Vector3[Tail.Count * 2];
            Vector2[] uv = new Vector2[Tail.Count * 2];
            int[] triangles = new int[(Tail.Count - 1) * 6];

            // Generate Vertices //
            for (int i = 0; i < Tail.Count; i++)
            {
                // Generate the vertex positions //
                Vector3 vector;
                if (i == 0)
                {
                    vector = Tail[i] - Tail[i + 1];
                }
                else if (i == Tail.Count - 1)
                {
                    vector = Tail[i - 1] - Tail[i];
                }
                else
                {
                    vector = Tail[i - 1] - Tail[i + 1];
                }

                vector.Normalize();

                Vector3 left = new Vector3(vector.y * -1, vector.x, -1);
                Vector3 right = new Vector3(vector.y, vector.x * -1, -1);

                // from 0 to 1 along the length of the tail //
                float v = 1 - ((float)i / (Tail.Count - 1));
                float tailwidth = Mathf.Lerp(TailWidthStart, TailWidthEnd, v);

                vertices[i * 2] = Tail[i] + left * tailwidth + transform.position;
                vertices[i * 2 + 1] = Tail[i] + right * tailwidth + transform.position;

                UVScroll += Time.deltaTime * 0.02f;
                uv[i * 2] = new Vector2(0, v * 20 + UVScroll);
                uv[i * 2 + 1] = new Vector2(1, v * 20 + UVScroll);

                //Debug.DrawLine(Tail[i] + left, Tail[i] + right, Color.blue);
            }

            // Generate Triangles //
            for (int i = 0; i < Tail.Count - 1; i++)
            {
                int t1 = i * 2;
                int t2 = i * 2 + 1;
                int t3 = i * 2 + 2;
                int t4 = i * 2 + 3;

                triangles[i * 6] = t1;
                triangles[i * 6 + 1] = t2;
                triangles[i * 6 + 2] = t3;

                triangles[i * 6 + 3] = t3;
                triangles[i * 6 + 4] = t2;
                triangles[i * 6 + 5] = t4;

                // Draw Wireframe //
                if (DrawWireframe)
                {
                    Debug.DrawLine(vertices[t1], vertices[t2], Color.black);
                    Debug.DrawLine(vertices[t3], vertices[t4], Color.black);
                    Debug.DrawLine(vertices[t1], vertices[t3], Color.black);
                    Debug.DrawLine(vertices[t2], vertices[t4], Color.black);
                }
            }

            // Draw Tail Mesh //
            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            MeshFilter m = GetComponent<MeshFilter>();
            Graphics.DrawMesh(mesh, Matrix4x4.identity, renderer.material, 0);
        }
    }
}
