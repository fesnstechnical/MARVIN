using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class ParticleCollision : MonoBehaviour
    {
        public ParticleSystem part;
        public List<ParticleCollisionEvent> collisionEvents;
        public TactSender TactSenderForCollision;
        public GameObject rayShootEffectPrefab;
        public GeigerSound geiger;

        //public int TotalCollisions;
        //public float TritiumIntake;
        //public float TritiumTotal;
        //float totalparticles;
        //public LayerMask hapticEnabledLayers;

        RadiationParticleSystemManager particleManager;
        [Tooltip("Number of radiation particles each Unity particle represents")]
        private float particleEnergyLevel = 1.0f;

        private static AudioManager am;

        [Tooltip("Do no more than this many collisions each frame")]
        //[SerializeField]
        uint maxCollisionsPerFrame = 3;


        void Awake()
        {
            part = GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
            am = AudioManager.GetInstance();
            particleManager = GetComponent<RadiationParticleSystemManager>();

            //TotalCollisions = 0;
            //totalparticles = part.startLifetime * part.emissionRate;
        }

        void OnParticleCollision(GameObject other)
        {
            if (other.layer == LayerMask.NameToLayer("Body"))
            {

                int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

                //Ray ray = new Ray(collisionEvents[0].intersection, collisionEvents[0].velocity);
                //RaycastHit raycastHit;
                //if (Physics.Raycast(ray, out raycastHit, 0.5f, hapticEnabledLayers) && rayShootEffectPrefab != null)
                //{
                //TactSenderForCollision.Play(PositionTag.Default, raycastHit);

                for (int i = 0; i < numCollisionEvents && i < maxCollisionsPerFrame; i++)
                {
                    Vector3 point = collisionEvents[i].intersection;
                    var go = Instantiate(rayShootEffectPrefab, point, Quaternion.identity);
                    Destroy(go, 0.1f);

                    //am.PlaySoundOnce(AudioManager.Sound.RadiationHit, go.transform, AudioManager.Priority.Spam);

                }
               // }
            } else if(other.layer == LayerMask.NameToLayer("Geiger"))
            {
                if(geiger == null)
                {
                    geiger = other.GetComponent<GeigerSound>();
                }
                //Debug.Log("adding" + particleManager.GetParticleMappingFactor() + "energy");
                geiger.AddDose(particleManager.GetParticleMappingFactor());
            }

        }
    }
}