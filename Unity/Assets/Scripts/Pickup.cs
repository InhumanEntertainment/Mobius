using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pickup : MonoBehaviour 
{
    public ParticleSystem EffectSpawn;
    public ParticleSystem EffectPickup;
    public int TailSegments = 0;
    
    //============================================================================================================================================//
    void Start() 
    {
        transform.Rotate(new Vector3(0, 0, Random.value * 10));
	}

    //============================================================================================================================================//
    void Update() 
    {
        //transform.Rotate(new Vector3(0, 0, 8f));
        transform.RotateAroundLocal(Vector3.forward, 5f * Time.deltaTime);                                             
	}

    //============================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == GameObject.Find("Snake"))
        {
            Destroy(gameObject);
            Camera cam = GameObject.Find("GameCamera").camera;
            Vector3 pos = new Vector3((Random.value * 0.8f - 0.4f) * cam.orthographicSize * cam.aspect * 2, (Random.value * 0.8f - 0.4f) * cam.orthographicSize * 2, 0);

            Spawn();

            

            GameObject mobius = GameObject.Find("Snake");
            SnakeController snake = mobius.GetComponent<SnakeController>();

            // Lengthen Tail //
            snake.TailMax += TailSegments;

            // Create Effect //
            Instantiate(EffectPickup, transform.position, new Quaternion());
        }
    }

    //============================================================================================================================================//
    void Spawn()
    {
        //Hexagons hex = GameObject.Find("Hexagons").GetComponent<Hexagons>();
        Camera cam = GameObject.Find("GameCamera").camera;
        
        // Create Inactive Hexagon List //
        //List<int> Inactive = new List<int>();

        /*for (int i = 0; i < hex.All.Count; i++)
		{
            if (hex.All[i].Active == 0)
	        {
		        Inactive.Add(i);
	        }			 
		}

        // Get Random Position //
        int r = (int)(Random.value * (Inactive.Count - 1));

        Vector3 pos = hex.All[Inactive[r]].Position;
        pos += hex.offset;
        pos = new Vector3(pos.x, Screen.height - pos.y, 0);
        
        pos = cam.ScreenToWorldPoint(pos);

        pos.z = 0;
        print(pos);
         */
        Vector3 pos = new Vector3(Screen.width * Random.value, Screen.height * Random.value, 0);
        pos = cam.ScreenToWorldPoint(pos);
        pos.z = 0;
        
        GameObject pickup = (GameObject)Resources.Load("Pickup");
        Quaternion q = Quaternion.Euler(new Vector3(-90, 0, 0));
        GameObject.Instantiate(pickup, pos, q);
    }
}
