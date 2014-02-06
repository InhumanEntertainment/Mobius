using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour 
{
    public Transform TouchPoint;
    public Transform[] Dots;

    //======================================================================================================================================//
    void Update()
    {
	    if (Input.GetMouseButton(0) && !GameButton.InUse && Time.timeScale != 0)
	    {
            Vector3 start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            start.z = 0;
            TouchPoint.position = start;
            TouchPoint.gameObject.SetActive(true);

            if (Game.Instance != null && Game.Instance.Player != null)
            {
                Vector3 end = Game.Instance.Player.transform.position;
                for (int i = 0; i < Dots.Length; i++)
                {
                    float delta = ((float)i + 1) / (Dots.Length + 1);
                    Dots[i].position = Vector3.Lerp(end, start, delta);
                    Dots[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < Dots.Length; i++)
                    Dots[i].gameObject.SetActive(false);
            }
	    }
        else
        {
            TouchPoint.gameObject.SetActive(false);
            for (int i = 0; i < Dots.Length; i++)
                Dots[i].gameObject.SetActive(false);
        }
	}
}
