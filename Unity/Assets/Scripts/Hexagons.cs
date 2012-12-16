using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hexagons : MonoBehaviour 
{
    public Texture HexagonTexture;
    public List<Hexagon>[] HexRows;
    public List<Hexagon>[] HexY;
    public List<Hexagon>[] HexZ;
    public List<Hexagon> All = new List<Hexagon>();

    int width = 12;
    public int height = 64;
    public float scale = 1f;
    public Vector3 offset = Vector3.zero;

    public int numrows = 9;
    public int numcols = 7;
    int numY;
    int numZ;

    //============================================================================================================================================//
    void Awake()
    {
        // Create Hexagons //
        float c = height / 2;
        float a = height / 4;
        float b = Mathf.Sqrt(c * c - a * a) * 2;
        width = (int)b;

        int extra;

        // Create Horizontal X Indices //
        HexRows = new List<Hexagon>[numrows];
        for (int y = 0; y < numrows; y++)
        {
            HexRows[y] = new List<Hexagon>();
            extra = y % 2;

            for (int x = 0; x < numcols + extra; x++)
            {
                Hexagon hex = new Hexagon(width);
                hex.Position = new Vector3(x * width - (extra * width / 2), y * (height * 0.75f), 0);

                All.Add(hex);
                HexRows[y].Add(hex);
            }
        }
        	    
        // Create Vertical Y and Z Indices //-----------------------------------------//
        // Get Side Rows //
        int numYSide = (numrows - 1) / 2;
        int numYTop = numcols;
        numY = numYTop + numYSide;
        HexY = new List<Hexagon>[numY];
        HexZ = new List<Hexagon>[numY];
        int currentY = 0;

        for (int i = numrows - 2; i > 0; i-=2)
        {
            HexY[currentY] = new List<Hexagon>();
            HexZ[currentY] = new List<Hexagon>();

            for (int y = 0, xx = 0; y < numrows; y++)
            {
                int step = 1 - (y % 2);
                xx = y / 2;
                if (y + i < numrows && xx < HexRows[y + i].Count)
                {
                    HexY[currentY].Add(HexRows[y + i][xx]);
                    HexZ[currentY].Add(HexRows[y + i][HexRows[y +i].Count - 1 - xx]);
                }
            }
            currentY++;
        }

        // Get Top rows //
        for (int x = 0; x < numYTop; x++)
        {
            HexY[currentY] = new List<Hexagon>();
            HexZ[currentY] = new List<Hexagon>();
            
            for (int y = 0, xx=0; y < numrows; y++)
            {
                int step = 1 - (y % 2);
                xx = 1 + y / 2 + x - step;
                if (xx < HexRows[y].Count)
                {
                    HexY[currentY].Add(HexRows[y][xx]);
                    HexZ[currentY].Add(HexRows[y][HexRows[y].Count - 1 - xx]);
                }
            }
            currentY++;
        }
	}

    //============================================================================================================================================//
    void Update()
    {
        
    }

    //============================================================================================================================================//
    void OnGUI()
    {

        // Clear Hexagon States //
        foreach (var hex in All)
        {
            hex.Active = 0;
        }

        SnakeController snake = GameObject.Find("Snake").GetComponent<SnakeController>();
        Camera cam = GameObject.Find("GameCamera").camera;
        
        // Set Hexagon States //
        for (int i = 0; i < snake.Tail.Count; i++)
        {
            Vector3 pos = cam.WorldToScreenPoint(snake.Tail[i]);

            for (int y = 0; y < numrows; y++)
            {
                for (int x = 0; x < HexRows[y].Count; x++)
                {

                    if (Vector3.Distance(new Vector3(pos.x, Screen.height - pos.y, 0), HexRows[y][x].Position + offset) < (height + width) / 4)
                    {
                        HexRows[y][x].Active = 1;
                        goto Skip;
                    }
                }
            }

        Skip:
            int a;
        }

        // Check X Lines //
        for (int y = 0; y < numrows; y++)
        {
            bool line = true;
            for (int x = 0; x < HexRows[y].Count; x++)
            {
                if (HexRows[y][x].Active == 0)
                {
                    line = false;
                    break;
                }
            }
            if (line)
            {
                for (int x = 0; x < HexRows[y].Count; x++)
                {
                    HexRows[y][x].Active = 2;
                }
            }
        }

        // Check Y Lines //
        for (int i = 0; i < numY; i++)
        {
            bool line = true;

            for (int x = 0; x < HexY[i].Count; x++)
            {
                if (HexY[i][x].Active == 0)
                {
                    line = false;
                    break;
                }
            }
            if (line)
            {
                for (int x = 0; x < HexY[i].Count; x++)
                {
                    HexY[i][x].Active = 2;
                }
            }
        }

        // Check Z Lines //
        for (int i = 0; i < numY; i++)
        {
            bool line = true;

            for (int x = 0; x < HexZ[i].Count; x++)
            {
                if (HexZ[i][x].Active == 0)
                {
                    line = false;
                    break;
                }
            }
            if (line)
            {
                for (int x = 0; x < HexZ[i].Count; x++)
                {
                    HexZ[i][x].Active = 2;
                }
            }
        }

        // Draw Hexagons // 
        if (HexRows != null)
        {           
            for (int y = 0; y < numrows; y++)
            {
                for (int x = 0; x < HexRows[y].Count; x++)
                {
                    if (HexRows[y][x].Active == 1)
                    {
                        GUI.color = new Color(1f, 0.1f, 0.5f, 0.2f);
                        GUI.DrawTexture(new Rect(HexRows[y][x].Position.x + offset.x - (height * scale / 2), HexRows[y][x].Position.y + offset.y - (height * scale / 2), height * scale, height * scale), HexagonTexture, ScaleMode.ScaleToFit, true);
                    }
                    else if (HexRows[y][x].Active == 2)
                    {
                        GUI.color = new Color(1f, 0.1f, 0.5f, 0.5f);
                        GUI.DrawTexture(new Rect(HexRows[y][x].Position.x + offset.x - (height * scale / 2), HexRows[y][x].Position.y + offset.y - (height * scale / 2), height * scale, height * scale), HexagonTexture, ScaleMode.ScaleToFit, true);
                    }
                    else
                    {
                        GUI.color = new Color(1f, 0.1f, 0.5f, 0.1f);
                        GUI.DrawTexture(new Rect(HexRows[y][x].Position.x + offset.x - (height * scale / 2), HexRows[y][x].Position.y + offset.y - (height * scale / 2), height * scale, height * scale), HexagonTexture, ScaleMode.ScaleToFit, true);                            
                    }
                }
            }
        }
	}
}




public class Hexagon
{
    public int Active = 0;
    public int Size = 0;
    public Vector3 Position = Vector3.zero;

    //============================================================================================================================================//
    public Hexagon(int size)
    {
        Size = size;
    }
}
