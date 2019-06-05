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
        public int TotalCollisions;
        public float TritiumIntake;
        public float TritiumTotal;
        float totalparticles;
        void Start()
        {
            part = GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
            TotalCollisions = 0;
            totalparticles = part.startLifetime * part.emissionRate;
        
        }

    
        void OnParticleCollision(GameObject other)
        {
            
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
            Ray ray = new Ray(collisionEvents[0].intersection, collisionEvents[0].velocity);
            RaycastHit raycastHit;
            TotalCollisions = TotalCollisions + collisionEvents.Count;

            TritiumIntake = TotalCollisions *  (TritiumTotal/totalparticles);

            if (Physics.Raycast(ray, out raycastHit, 10))
            {
                //TactSenderForCollision.Play(PositionTag.Default, raycastHit);
                var go = Instantiate(rayShootEffectPrefab, collisionEvents[0].intersection, Quaternion.identity);
                Destroy(go, 0.1f);
                //TactSenderForCollision.Play(PositionTag.Default, collisionEvents[0].intersection,other.GetComponent<Collider>());
                Debug.DrawLine(collisionEvents[0].intersection,raycastHit.point, Color.red);
                Debug.Log(collisionEvents[0].intersection.x + "&" + collisionEvents[0].intersection.y);
            }
                
            
        }
    }
}