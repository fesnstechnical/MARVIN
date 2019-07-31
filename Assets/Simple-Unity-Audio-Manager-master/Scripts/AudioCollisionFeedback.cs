// Authored by: Jacky Yang
// Modified by: Joss Moo-Young

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCollisionFeedback : MonoBehaviour
{
    [SerializeField]
    AudioManager.Sound sound = AudioManager.Sound.GenericImpact;

    [SerializeField]
    AudioManager.Priority priority = AudioManager.Priority.Default;

    [SerializeField]
    AudioManager.Pitch pitch = AudioManager.Pitch.VeryLow;

    [SerializeField]
    float MIN_IMPACT_SPD = 0.7f;

    AudioManager am;



    // Start is called before the first frame update
    void Start()
    {
        // store impact speed in squared form
        MIN_IMPACT_SPD = MIN_IMPACT_SPD * MIN_IMPACT_SPD;
        am = AudioManager.GetInstance();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactSpdSq = collision.relativeVelocity.sqrMagnitude;
        if (impactSpdSq > MIN_IMPACT_SPD)
        {
            am.PlaySoundOnce(sound, transform, priority, AudioManager.UsePitch(pitch));
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        float impactSpdSq = collision.relativeVelocity.sqrMagnitude;
        if (impactSpdSq > MIN_IMPACT_SPD)
        {
            am.PlaySoundOnce(sound, transform, priority, AudioManager.UsePitch(pitch));
        }
    }
}
