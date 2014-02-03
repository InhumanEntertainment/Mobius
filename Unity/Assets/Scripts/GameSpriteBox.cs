using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[ExecuteInEditMode]
public class GameSpriteBox: MonoBehaviour 
{
    public Sprite Sprite;
    public Vector2 Size = new Vector2(128, 32);
    public Vector2 SizeTopLeft = new Vector2(2, 2);
    public Vector2 SizeBottomRight = new Vector2(2, 2);

    Mesh Mesh;

    public string SortingLayer;
    public int SortingOrder;

    public float PixelUnits = 100;
    public bool Reload = false;

    //============================================================================================================================================//
    void Awake()
    {
        Mesh = new Mesh();
	}

    //============================================================================================================================================//
    void Update()
    {
        if (SortingOrder != renderer.sortingOrder)
            renderer.sortingOrder = SortingOrder;
        if (SortingLayer != renderer.sortingLayerName)
            renderer.sortingLayerName = SortingLayer;

        if (Sprite != null && Reload == true)
        {
            CreateMesh();
            Reload = false;
        }
	}

    //============================================================================================================================================//
    Vector2 PixelToUV(Vector2 pos)
    {
        Vector2 uv_scale = new Vector2(1f / (float)renderer.sharedMaterial.mainTexture.width, 1f / (float)renderer.sharedMaterial.mainTexture.height);
        return Vector2.Scale(pos, uv_scale);
    }

    //============================================================================================================================================//
    void CreateMesh()
    {
        List<Vector3> Vertices = new List<Vector3>();
        List<Vector2> Uvs = new List<Vector2>();
        List<int> Triangles = new List<int>();
        List<Color> Colors = new List<Color>();


        Vector3[] vertex_offsets = { Vector3.zero, new Vector3(SizeTopLeft.x, SizeTopLeft.y, 0), new Vector3(Size.x - SizeBottomRight.x, Size.y - SizeBottomRight.y, 0), new Vector3(Size.x, Size.y, 0) };
        Vector2 uv_offset = new Vector2(Sprite.rect.x, Sprite.rect.y);
        Vector2 sprite_size = new Vector2(Sprite.rect.width, Sprite.rect.height);
        Vector2[] uv_offsets = {
                                   PixelToUV(uv_offset), 
                                   PixelToUV(uv_offset + SizeTopLeft), 
                                   PixelToUV(uv_offset + sprite_size - SizeBottomRight), 
                                   PixelToUV(uv_offset + sprite_size)
                               };

        // Vertices + UVs + Colors //
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Vertices.Add(new Vector3(vertex_offsets[x].x, vertex_offsets[y].y, 0) / PixelUnits);
                Uvs.Add(new Vector2(uv_offsets[x].x, uv_offsets[y].y));
                Colors.Add(Color.white);
            }
        }

        // Triangles //
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                int v1 = y * 4 + x;
                int v2 = y * 4 + x + 1;
                int v3 = (y + 1) * 4 + x;
                int v4 = (y + 1) * 4 + x + 1;

                Triangles.Add(v3);
                Triangles.Add(v2);
                Triangles.Add(v1);

                Triangles.Add(v3);
                Triangles.Add(v4);
                Triangles.Add(v2);
            }
        }  
        
        // Draw Mesh //           
        Mesh.Clear();
        Mesh.vertices = Vertices.ToArray();
        Mesh.uv = Uvs.ToArray();
        Mesh.triangles = Triangles.ToArray();
        Mesh.colors = Colors.ToArray();

        MeshFilter m = GetComponent<MeshFilter>();
        if (m == null)
            m = gameObject.AddComponent<MeshFilter>();

        MeshRenderer r = GetComponent<MeshRenderer>();
        if (r == null)
            r = gameObject.AddComponent<MeshRenderer>();

        m.mesh = Mesh;     
    }
}
