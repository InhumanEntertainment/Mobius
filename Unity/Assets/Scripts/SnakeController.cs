using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeController : MonoBehaviour 
{
    public List<Vector3> Tail = new List<Vector3>();
    Mesh mesh;
    public Vector3 Velocity = Vector3.zero;
    public Vector3 Target = Vector3.up;

    public float Speed = 10;
    public float Rotation = 10;
    public int TailMax = 50;
    public float TailWidthStart = 1f;
    public float TailWidthEnd = 1f;   
    public bool DrawWireframe = false;

    public bool DrawGuide;
    public Color GuideColor = new Color(0.7f, 0.8f, 1);
    public Texture TetureGuideDot;
    public Texture TetureGuidePosition;

    float DistanceTraveled = 0;
    public float StepSpacing = 0.05f;
            

    //============================================================================================================================================//
    void Awake()
    {
		Application.targetFrameRate = 60;
        mesh = new Mesh();
        Tail.Add(Vector3.zero);

        MeshFilter m = GetComponent<MeshFilter>();
        m.mesh = mesh;
    }

    //============================================================================================================================================//
    void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.collider.name);
    }

    //============================================================================================================================================//
    void Update() 
	{
        if (Time.timeScale > 0)
        {
            // Controls // 
            //#if UNITY_ANDROID1 || UNITY_IPHONE1
            if (Input.GetMouseButton(0))
            {
                Target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Target.z = 0;
            }
            //#else
            //        if (Input.touchCount > 0)
            //        {
            //            Target = new Vector3((Input.touches[0].position.x / w - 0.5f) * cam.orthographicSize * cam.aspect * 2, (Input.touches[0].position.y / h - 0.5f) * cam.orthographicSize * 2, 0);
            //        }
            //#endif

            Vector3 vec = Target - transform.position;
            vec.Normalize();

            float distance = Vector3.Distance(Target, transform.position);
            float force = 0;
            if (distance < 100)
            {
                force = (100f - distance) / 100f;
                force = (float)Mathf.Pow(force, 2);
                force *= Rotation;
            }
            Velocity += (vec * force * Time.deltaTime);

            if (!(Velocity.x == 0 && Velocity.y == 0))
            {
                Velocity.Normalize();
                Velocity = Velocity * Speed;
            }

            transform.position += (Velocity * Time.deltaTime);

            // Add Tail segment once you have travelled a certain distance //
            DistanceTraveled += (Velocity * Time.deltaTime).magnitude;
            Vector3 prevposition = Tail[Tail.Count - 1];

            if (DistanceTraveled > StepSpacing)
            {
                int steps = (int)(DistanceTraveled / StepSpacing);
                for (int i = 0; i < steps; i++)
                {
                    float delta = (float)(i + 1) / steps;
                    Vector3 pos = prevposition * (1 - delta) + transform.position * delta;

                    Tail.Add(pos);
                    if (Tail.Count > TailMax)
                        Tail.RemoveRange(0, Tail.Count - TailMax);

                    DistanceTraveled -= StepSpacing;
                }
            }

            //print(Velocity.magnitude);

            // Rotate Head Cube //
            //GameObject cube = GameObject.Find("Cube");
            //cube.transform.LookAt(transform.position);
            //cube.transform.position = transform.position;

            // Death Tail //
            
            /*for (int i = 0; i < Tail.Count - 1; i++)
            {
                if ((Tail[i] - Tail[Tail.Count - 1]).magnitude < 0.6f)
                {
                    Tail.Clear();
                    print("Tail");
                    GameOver over = GameObject.Find("GameOver").GetComponent<GameOver>();
                    over.Over();
                    goto Draw;
                }
            }*/

            // Death Walls //
            if (Camera.main.WorldToScreenPoint(Tail[Tail.Count - 1]).x < 0 || Camera.main.WorldToScreenPoint(Tail[Tail.Count - 1]).x > Screen.width)
            {
                //App.Instance.SetScreen("Game Over");
            }
            else if (Camera.main.WorldToScreenPoint(Tail[Tail.Count - 1]).y < 0 || Camera.main.WorldToScreenPoint(Tail[Tail.Count - 1]).y > Screen.height)
            {
                //App.Instance.SetScreen("Game Over");
            }
        }

        BuildMesh();
    }

    //============================================================================================================================================//
    void OnDrawGizmos()
    {
        // Draw Collision Spheres //
        for (int i = 0; i < Tail.Count - 1; i++)
        {
            Gizmos.color = Color.grey;
            //Gizmos.DrawWireSphere(Tail[i], 0.5f);
        }
    }

    //============================================================================================================================================//
    void OnGUI()
    {
        if (DrawGuide)
        {
            // Get Game Camera //
            Vector3 target = Camera.main.WorldToScreenPoint(Target);
            Vector3 pos1 = Camera.main.WorldToScreenPoint(transform.position);

            // Draw Guides //
            GUI.color = new Color(GuideColor.r, GuideColor.g, GuideColor.b, 0.1f);
            GUI.DrawTexture(new Rect(target.x - 32, Screen.height - target.y - 32, 64, 64), TetureGuidePosition);        
            
            // Draw Markers //
            GUI.color = new Color(GuideColor.r, GuideColor.g, GuideColor.b, 1f);
            for (float i = 0; i < 1; i += 0.05f)
            {
                Vector3 pos = pos1 * i + target * (1.0f - i);
                pos.y = Screen.height - pos.y;
                GUI.DrawTexture(new Rect(pos.x - 2, pos.y - 2, 4, 4), TetureGuideDot);
            }
        }
    }

    //============================================================================================================================================//
    void BuildMesh()
    {
        if (Tail.Count > 1)
        {
            // Then create triangles //
            Vector3[] vertices = new Vector3[Tail.Count * 2];
            Color[] colors = new Color[Tail.Count * 2];
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

                Vector3 left = new Vector3(vector.y * -1, vector.x, 0);
                Vector3 right = new Vector3(vector.y, vector.x * -1, 0);

                // from 0 to 1 along the length of the tail //
                float v = 1 - ((float)i / (Tail.Count - 1));
                float tailwidth = Mathf.Lerp(TailWidthStart, TailWidthEnd, v);

                vertices[i * 2] = Tail[i] + left * tailwidth - transform.position;
                vertices[i * 2 + 1] = Tail[i] + right * tailwidth - transform.position;

                colors[i * 2] = Color.white;
                colors[i * 2 + 1] = Color.white;


                float uvmax = Tail.Count / 20f;
                uv[i * 2] = new Vector2(0, v * uvmax);
                uv[i * 2 + 1] = new Vector2(1, v * uvmax);

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
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.triangles = triangles;  
          

            //Graphics.DrawMesh(mesh, Matrix4x4.identity, renderer.material, 0);
        }       
    }
}
