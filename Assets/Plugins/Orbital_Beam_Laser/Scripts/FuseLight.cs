using UnityEngine;

public class FuseLight : MonoBehaviour
{
    public Light fuseLight;
    private int fuseLightIntensity = 10;

    void Start()
    {
        
    }

    void Update()
    {
        fuseLightIntensity = Random.Range(5, 14);
        fuseLight.intensity = fuseLightIntensity;
    }
}