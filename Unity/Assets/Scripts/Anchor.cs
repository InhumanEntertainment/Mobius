using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Anchor : MonoBehaviour 
{
	public enum AnchorPoint
	{
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
        Custom
	}

	public AnchorPoint Location = AnchorPoint.MiddleCenter;
    public Vector2 CustomLocation = new Vector2(0.5f, 0.5f);
	Vector3[] Vectors = 
	{
		new Vector3(0, 1, 0),
		new Vector3(0.5f, 1, 0),
		new Vector3(1, 1, 0),
		new Vector3(0, 0.5f, 0),
		new Vector3(0.5f, 0.5f, 0),
		new Vector3(1, 0.5f, 0),
		new Vector3(0, 0, 0),
		new Vector3(0.5f, 0, 0),
		new Vector3(1, 0, 0),
	};
	
	//=======================================================================================================================================================/
	void Update () 
	{
        Vector3 offset;
        if (Location == AnchorPoint.Custom)
            offset = transform.parent.position + Vector3.Scale(CustomLocation, new Vector3(Camera.main.rect.width * Screen.width, Camera.main.rect.height * Screen.height, 0)) + new Vector3(Camera.main.rect.xMin * Screen.width, Camera.main.rect.yMin, 0);
        else
        {
            Vector3 cam = Vectors[(int)Location];
            if (Location == AnchorPoint.TopLeft || Location == AnchorPoint.MiddleLeft || Location == AnchorPoint.BottomLeft)
                cam += new Vector3(Camera.main.rect.xMin, 0, 0);
            else if (Location == AnchorPoint.TopRight || Location == AnchorPoint.MiddleRight || Location == AnchorPoint.BottomRight)
                cam -= new Vector3(Camera.main.rect.xMin, 0, 0);

            // Positioning //
            offset = transform.parent.position + Vector3.Scale(cam, new Vector3(Screen.width, Screen.height, 0));         
        }

        Vector3 wh = Camera.main.ScreenToWorldPoint(offset);
        wh.z = 0;
        transform.localPosition = wh;
	}
}
