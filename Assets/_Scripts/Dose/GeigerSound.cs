using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeigerSound : MonoBehaviour
{

    private AudioSource clickSource;

    private GeigerController geigerController;

    //[SerializeField]
    //[Tooltip("Adds some randomness to clicking. 1.0 means no clicks are lost. 0.5 means half are lost")]
    //float clickProbability = 0.5f;
    //
    //Vector2 clickDelayRange = new Vector2(1.0f, 0.01f);
    [SerializeField]
    [Tooltip("Minimum time between clicks")]
    float clickDelayMinimum = 0.001f;

    [Tooltip("Energy accumulated from particle collisions--causes clicks")]
    [SerializeField]
    float particleEnergyLevel = 0.0f;

    static float MAX_PARTICLE_ENERGY = 10.0f; 
    
    // Start is called before the first frame update
    void Start()
    {
        clickSource = GetComponents<AudioSource>()[1];

        geigerController = GetComponent<GeigerController>();
        //StartCoroutine(SoundCoroutineV3() );
    }

    public void TryClick()
    {
        //if(geigerController.getActive() && !clickSource.isPlaying)
        //{
        //    clickSource.Play();
        //}
    }

    public void AddDose(float particleEnergy)
    {
        particleEnergyLevel = Mathf.Min(particleEnergyLevel + particleEnergy, MAX_PARTICLE_ENERGY);
    }

    private void Update()
    {
        if (geigerController.getActive() && !clickSource.isPlaying)
        {
            if(particleEnergyLevel > 1.0f)
            {
                particleEnergyLevel -= 1.0f;

                clickSource.Stop();
                clickSource.Play();
            }
        }
    }

    IEnumerator SoundCoroutineV2()
    {
        float timer = 0;
    
        while (geigerController.getActive())
        {
            //Debug.Log("lol sound update");
            float intensityOfHundred = geigerController.getIntensity();

            if (intensityOfHundred > 0)
            {
                float waitTime = clickDelayMinimum / (intensityOfHundred * 0.01f); //Mathf.InverseLerp(clickDelayRange.x, clickDelayRange.y, intensityOfHundred * 0.01f);

                // if a new update should cause the timer to change, set the new timer
                if(waitTime < timer)
                {
                    timer = waitTime;
                }

                // if timer is done then click
                if (timer <= 0.0f)
                {
                    clickSource.Play();
                    //Debug.Log("geiger: click!");
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    
    }


    IEnumerator SoundCoroutine()
    {

        while (true)
        {

            if (geigerController != null)
            {

                if (geigerController.getIntensity() > 0 && geigerController.getActive())
                {


                    clickSource.Play();

                    //Want it so at 100% tick is every 0.5seconds
                    float minTime = 0.1f;
                    float maxTime = 1f;

                    //y=mx+b
                    float m = -(maxTime - minTime) / (100);
                    float b = maxTime;

                    yield return new WaitForSeconds((m * geigerController.getIntensity()) + b);


                }
                else
                {

                    yield return new WaitForSeconds(1f);

                }

            }

        }

    }


}
