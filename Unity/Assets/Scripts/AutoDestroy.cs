using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour 
{
	//============================================================================================================================================================================================//
    void LateUpdate() 
    {
	    if (!particleSystem.IsAlive())
            Destroy(gameObject);
	}
}
