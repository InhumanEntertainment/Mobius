using UnityEngine;
using System.Collections;

[System.Serializable]
public class ResolutionOverride
{
    public string Name;
    public int ScreenHeight;
    public int TargetHeight;
}

[ExecuteInEditMode]
public class GameCamera : MonoBehaviour 
{
    public bool PixelPerfect = true;  

    /*// Pixel Perfect Resolution //
    public float TargetScale = 0;
    public int TargetHeight = 160;
    public int ScreenHeight = 720;
    public int ScreenWidth = 1280;
    public Vector2 Resolution;*/

    // Screen Resolution Overrides
    public float UnitScale = 10;
    public int DefaultTargetWidth = 240;
    public int DefaultTargetHeight = 360;
    public ResolutionOverride[] Resolutions;
	
    //============================================================================================================================================================================================//
    void Awake()
    {
        // Singleton: Destroy all others //
        Object[] cameras = FindObjectsOfType(typeof(GameCamera));
        if (cameras.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        float targetWidth = DefaultTargetWidth;
        float targetHeight = DefaultTargetHeight;

        /*foreach (ResolutionOverride resolution in Resolutions)
        {
            if (Screen.height == resolution.ScreenHeight)
            {
                //print("Found Override: " + resolution.ScreenHeight + " : " + resolution.TargetHeight);
                targetHeight = resolution.TargetHeight;
            }
        }*/

        //camera.orthographicSize = targetHeight / UnitScale / 2;
        //print("Resolution: " + Screen.height + " -> " + targetHeight + " - Scale: " + Screen.height / targetHeight);
    }

	//============================================================================================================================================================================================//
	public float CamWidthExact;
    public float CamHeightExact;
    public int CamWidthClosest;
    public int CamHeightClosest;
    public float CamAspect;
    public float CamOffset;
    public float CamSize;

    public float ScreenAspect;
    public float TargetAspect;

    //============================================================================================================================================================================================//
    void Update()
	{
        ScreenAspect = Screen.width / (float)Screen.height;
        TargetAspect = DefaultTargetWidth / (float)DefaultTargetHeight;

        float AspectRatio = ScreenAspect;

        if (ScreenAspect < TargetAspect)
        {
            AspectRatio = 1 / ScreenAspect;
            camera.rect = new Rect(0, 0, 1, 1);
        }
        else
        {
            AspectRatio = 1 / TargetAspect;

            // Center Viewport on PC //
            //if (Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64)
                camera.rect = new Rect((1 - (TargetAspect / ScreenAspect)) / 2, 0, TargetAspect / ScreenAspect, 1);
            //else
            //    camera.rect = new Rect(0, 0, TargetAspect / ScreenAspect, 1);
        }

        camera.orthographicSize = AspectRatio * (DefaultTargetWidth / UnitScale / 2);


        //CamAspect = Screen.width / (float)Screen.height;

        // Width //
        //CamWidthExact = Screen.width / (float)DefaultTargetWidth;
        //CamWidthClosest = Mathf.RoundToInt(CamWidthExact);

        // Height //
        //CamHeightExact = Screen.height / (float)DefaultTargetHeight;
        //CamHeightClosest = Mathf.RoundToInt(CamHeightExact);

        /*float CamClosest = CamWidthExact;
        if (Mathf.Abs(CamWidthExact) > Mathf.Abs(CamHeightExact))
            CamClosest = CamHeightExact;*/

        //CamSize = CamClosest;
        //camera.orthographicSize = CamClosest;
        //camera.orthographicSize = ((DefaultTargetHeight / UnitScale) * CamHeightExact / CamClosest / 2 * DefaultTargetHeight);
	}
	
	//============================================================================================================================================================================================//
    float pixify(float number)
    {
        float result = ((int)(number * 10)) / 10f;

        return result;
    }

    //============================================================================================================================================================================================//
    /*void FindPerfectResolution()
    {
        if (ScreenHeight > TargetHeight)
        {
            float remainder = 0;
            int testHeight = TargetHeight;

            do
            {
                remainder = ScreenHeight % testHeight;
                if (remainder == 0)
                {
                    TargetScale = ScreenHeight / testHeight;
                    Resolution = new Vector2(ScreenWidth / TargetScale, testHeight);
                }
                testHeight++;
            }
            while (remainder > 0);
        }
    }*/

    //============================================================================================================================================//
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(Camera.main.rect.width * Camera.main.orthographicSize * ScreenAspect * 2, Camera.main.rect.height * Camera.main.orthographicSize * 2, 0));
    }
}
