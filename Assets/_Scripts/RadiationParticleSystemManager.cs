/// <summary>
/// This script adjust particle system emission and other properties based on the activity of a radiation source
/// </summary>

using UnityEngine;

public class RadiationParticleSystemManager : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSys;
    [SerializeField]
    private Source radiationSource;


    //[Header("Particle Emission Activity Throttling (shared)")]
    static public Vector2 radiationAmountRange = new Vector2(0.0f, 2000.0f);
    static public Vector2 emissionRateRange = new Vector2(0.0f, 2000);
    static public Vector2 ParticleScaleRange = new Vector2(0.2f, 15.0f);
    //private static float MaxParticleRate = 5000.0f;

    private float startSize = 0.05f;
    //private float count

    [SerializeField]
    float particleMappingFactor = 1.0f;
    
    [SerializeField]
    GeigerSound geigerSound;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (geigerSound == null)
        {
            geigerSound = GameObject.Find("GeigerTool").GetComponent<GeigerSound>();
        }

        if (radiationSource == null)
        {
            radiationSource = GetComponent<Source>();
        }

        if (radiationSource == null)
        {
            radiationSource = GetComponentInParent<Source>();
        }

        if (particleSys == null)
        {
            particleSys = GetComponent<ParticleSystem>();
        }
    }
#endif

    // Start is called before the first frame update
    void Awake()
    {
        startSize = particleSys.main.startSizeMultiplier;


        UpdateEmissionRate();
        InvokeRepeating("UpdateEmissionRate", 0.0f, 1.0f);
    }

    public float GetParticleMappingFactor()
    {
        return particleMappingFactor;
    }

    void UpdateEmissionRate()
    {
        float activity = radiationSource.totalActivityLevelBq;
        var emissionProps = particleSys.emission;
        var systemProps = particleSys.main;

        float normalizedRadiation = Mathf.InverseLerp(radiationAmountRange.x, radiationAmountRange.y, activity);

        float rate = Mathf.Lerp(emissionRateRange.x, emissionRateRange.y, normalizedRadiation);
        emissionProps.rateOverTime = Mathf.Clamp(rate, emissionRateRange.x, emissionRateRange.y);

        //float startSizeScale = Mathf.Lerp(ParticleScaleRange.x, ParticleScaleRange.y, normalizedRadiation);
        //systemProps.startSizeMultiplier = startSize * startSizeScale;
        
        particleMappingFactor = activity / rate;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == LayerMask.NameToLayer("Geiger"))
        {
            geigerSound.TryClick();
            //Debug.Log("Geiger click!");
        }
    }
}
