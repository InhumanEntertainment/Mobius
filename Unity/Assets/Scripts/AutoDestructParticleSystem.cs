using UnityEngine;

public class AutoDestructParticleSystem : MonoBehaviour
{
    void LateUpdate() 
    {
        if (!particleSystem.IsAlive())
            Object.Destroy(this.gameObject); 
    }
}