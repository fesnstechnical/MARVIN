using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//(BETA - Code not fully optimize, for DEMO and Testing purposes only)
public class PSOnTrigger : MonoBehaviour {

    public ParticleSystem ps;
    int oldnum = 0;
    int ind = 0;

    public List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    //public List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();

    
    void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();

        GameObject[] sceneObj = GameObject.FindGameObjectsWithTag("Shielding");


        //Debug.Log(ps.trigger.maxColliderCount);

        for (int i = 0; i < sceneObj.Length; i++)
        {

            ps.trigger.SetCollider(i, sceneObj[i].GetComponent<Collider>());
            Debug.Log(i);
        }
        //Debug.Log(ps.trigger.GetCollider(8).name);
        //float thickness = gameObject.GetComponent<SizeControl>().thickness;
    }
    
    void Update()
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(ps.transform.position , 40);
        List<GameObject> obj = new List<GameObject>();
        int numbarrier = 0;
        int num = 0;
        int newnum = hitColliders.Length;
        //hitColliders = hitColliders.OrderBy(x => Vector2.Distance(this.transform.position, x.transform.position)).ToList();
        //Debug.Log(newnum);
        //Debug.Log(oldnum);
        //if (newnum != oldnum)
        //{
        for ( int i = 0 ; i < hitColliders.Length ; i++ ) {
            if ( hitColliders[i].gameObject.tag == "Shielding" ) {
                obj.Add(hitColliders[i].gameObject);
                numbarrier++;

            }
        }
        
        obj = obj.OrderBy(x => Vector3.Distance(this.transform.position , x.transform.position)).ToList();

        int n = obj.Count;

        if ( n > 6 ) {

            n = 6;

        }

        for ( int i = 0 ; i < n ; i++ ) {
            if ( obj[i].tag == "Shielding" )//hitColliders[i].gameObject.tag == "Barrier")
            {
                ps.trigger.SetCollider(num , obj[i].GetComponent<Collider>());
                ind++;
                num++;

                if ( ind == 5 ) {
                    ind = 0;
                }
            }
            oldnum = newnum;
        }
            
        //}

        

    }
    private void OnParticleTrigger()
    {
        
        int enterParticleNumber = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        //SizeControl size = GameObject.Find("Wall").GetComponent<SizeControl>();
        Vector3 exitpoint = new Vector3(0,0,0);
        Vector3 entrypoint = new Vector3(0, 0, 0);
        //Debug.Log(size.thickness);
        
        //int exitParticleNumber = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);

        for (int i=0; i< enterParticleNumber; i++)
        {
            ParticleSystem.Particle p = enter[i];
            Ray ray = new Ray(p.position+p.velocity, -p.velocity.normalized);
            Ray namehit = new Ray(p.position, p.velocity.normalized);
            RaycastHit[] hits = Physics.RaycastAll(ray,p.velocity.magnitude);
            RaycastHit getName;
            string colliderhit = "N/A";

            if (Physics.Raycast (namehit,out getName,0.1F))
            {
                colliderhit = getName.collider.name;
               // Debug.Log(colliderhit);
            }

            

            entrypoint = p.position;

            
            foreach (RaycastHit hit in hits)
            {
                //Debug.DrawLine(hit.point, hit.point + Vector3.up * 5, Color.red);
                //Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject.name == colliderhit)
                {
                    //Debug.Log(colliderhit);
                    exitpoint = hit.point;
                    Debug.DrawLine(exitpoint, entrypoint,Color.red);
                    //Debug.Log(hit.point);
                    //Debug.Log((exitpoint - entrypoint).magnitude);
                    break;
                }
            }

            if (Random.Range(0, 1000) < 1000*(1-Mathf.Exp(-1.2F*(exitpoint-entrypoint).magnitude)))
            {
                
                //Debug.Log(1000*(1-Mathf.Exp(-0.5F * (exitpoint - entrypoint).magnitude)));
                p.velocity = new Vector3(0, 0, 0);
                p.remainingLifetime = 1;      
                
            }
            enter[i] = p;

        }

        //for (int i = 0; i < exitParticleNumber; i++)
        //{
        //    ParticleSystem.Particle p = exit[i];
        //    p.velocity = new Vector3(0, 0, 0);
        //    p.startColor = new Color32(255, 0, 0, 255);
        //    exit[i] = p;

        //}


        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        //ps.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);

        
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	
}
