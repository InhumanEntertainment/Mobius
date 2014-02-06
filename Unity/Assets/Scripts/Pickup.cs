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
        //transform.Rotate(new Vector3(0, 0, Random.value * 10));
	}

    //============================================================================================================================================//
    void Update() 
    {
        //transform.Rotate(new Vector3(0, 0, 8f));
        //transform.RotateAroundLocal(Vector3.forward, 5f * Time.deltaTime);                                             
	}

    //============================================================================================================================================//
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            Destroy(gameObject);
            Camera cam = Camera.main;
            Vector3 pos = new Vector3((Random.value * 0.8f - 0.4f) * cam.orthographicSize * cam.aspect * 2, (Random.value * 0.8f - 0.4f) * cam.orthographicSize * 2, 0);

            Spawn();

            Game.Instance.AddScore(1);
            

            SnakeController snake = Game.Instance.Player;

            // Lengthen Tail //
            snake.TailMax += TailSegments;

            // Create Effect //
            Instantiate(EffectPickup, transform.position, new Quaternion());
        }
    }

    //============================================================================================================================================//
    void Spawn()
    {
        Vector3 pos = new Vector3(Screen.width * Random.value, Screen.height * Random.value, 0);
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = 0;
        
        GameObject pickup = (GameObject)Resources.Load("Pickup");
        GameObject.Instantiate(pickup, pos, Quaternion.identity);
    }
}
