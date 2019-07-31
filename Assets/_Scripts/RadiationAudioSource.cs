/// <summary>
/// This script adjust sound source properties based on the activity of a radiation source
/// </summary>

using UnityEngine;

[RequireComponent(typeof(Source))]
[RequireComponent(typeof(AudioSource))]
public class RadiationAudioSource : MonoBehaviour
{

    [SerializeField]
    Source radiationSource;

    AudioSource audioSource;

    [SerializeField]
    [Tooltip("How much radiation is the minimum and how much is the maximum for mapping of radiation level to sound properties")]
    Vector2 radiationAmountRange = new Vector2(1, 10000); // Bq
    [SerializeField]
    [Tooltip("Pitch range based on radiation level")]
    Vector2 pitchRange = new Vector2(0.1f, 3.0f);
    [SerializeField]
    [Tooltip("Pitch range based on radiation level")]
    Vector2 volumeRange = new Vector2(0.25f, 0.2f);

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (radiationSource == null)
        {
            radiationSource = GetComponent<Source>();
        }

        if (radiationSource == null)
        {
            radiationSource = GetComponentInParent<Source>();
        }

        UpdateAudio();
        InvokeRepeating("UpdateAudio", 0.0f, 1.0f);
    }

    void UpdateAudio()
    {
        float activity = radiationSource.totalActivityLevelBq;

        float normalizedRadiation = Mathf.InverseLerp(radiationAmountRange.x, radiationAmountRange.y, activity);

        float pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, normalizedRadiation);
        audioSource.pitch = pitch;

        float vol = Mathf.Lerp(volumeRange.x, volumeRange.y, normalizedRadiation);
        audioSource.volume = vol;

        float derp = UtilMath.Lmap(0.5f, 0.0f, 1.0f, 0.0f, 100.0f);
    }
}