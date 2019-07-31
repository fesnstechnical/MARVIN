using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PSOnTrigger_Minh : MonoBehaviour {

    public ParticleSystem ps;
    Dictionary<string, float> DB = new Dictionary<string, float>();
    
    float acoef = 1.0f;
    int oldnum = 0;
    int ind = 0;
    float value = 0;
    public List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    string Source;
    Dictionary<string, float> ShieldList = new Dictionary<string, float>();

    Vector3 oldposition = new Vector3(0, 0, 0);

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        DB = GameObject.Find("Attenuation Database").GetComponent<AttenuationDB>().acoef;
        GameObject[] sceneObj = UnityEngine.Object.FindObjectsOfType<GameObject>();
        Collider[] hitColliders = Physics.OverlapSphere(ps.transform.position, 1);
        oldposition = this.transform.position;

        //Source = this.GetComponent<Source>().isotopes[0].radioIsotope.ToString();

        List<GameObject> obj = new List<GameObject>();
        int numbarrier = 0;
        int num = 0;
        int newnum = hitColliders.Length;
        for (int i = 0; i < hitColliders.Length; i++)
        {
           

            if (hitColliders[i].gameObject.tag == "Barrier")
            {
                obj.Add(hitColliders[i].gameObject);
                numbarrier++;
            }
        }

        obj = obj.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();

        for (int i = 0; i < numbarrier; i++)
        {

            ps.trigger.SetCollider(num, obj[i].GetComponent<Collider>());
            ind++;
            num++;
                
            if (ind == 5)
            {
                    ind = 0;
            }
            
            oldnum = newnum;

        }

        for (int i = 0; i < sceneObj.Length; i++)
        {
            if (sceneObj[i].GetComponent<Shield>() != null)
            {


                if ( DB.TryGetValue(sceneObj[ i ].GetComponent<Shield>().getName() , out value) ) {

                    float another;
                    if (!ShieldList.TryGetValue(sceneObj[i].gameObject.name, out another))
                    {
                        ShieldList.Add(sceneObj[i].gameObject.name, value);
                    }
                    
                }

            }
            
            
        }

    }

    void Update()
    {
        if ((this.transform.position - oldposition).magnitude > 1.0f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(ps.transform.position, 1);
            List<GameObject> obj = new List<GameObject>();
            int numbarrier = 0;
            int num = 0;
            int newnum = hitColliders.Length;
            for (int i = 0; i < hitColliders.Length; i++)
            {


                if (hitColliders[i].gameObject.tag == "Barrier")
                {
                    obj.Add(hitColliders[i].gameObject);
                    numbarrier++;
                }
            }

            obj = obj.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();

            for (int i = 0; i < numbarrier; i++)
            {

                ps.trigger.SetCollider(num, obj[i].GetComponent<Collider>());
                ind++;
                num++;

                if (ind == 5)
                {
                    ind = 0;
                }

                oldnum = newnum;
            }

            oldposition = this.transform.position;
        }

    }

    public void reloadList() {

        ShieldList = new Dictionary<string , float>();
        GameObject[] sceneObj = UnityEngine.Object.FindObjectsOfType<GameObject>();

        for ( int i = 0 ; i < sceneObj.Length ; i++ ) {
            if ( sceneObj[ i ].GetComponent<Shield>() != null ) {


                if ( DB.TryGetValue(sceneObj[ i ].GetComponent<Shield>().getName() , out value) ) {

                    ShieldList.Add(sceneObj[ i ].gameObject.name , value);
                }

            }


        }

    }
    
    private void OnParticleTrigger()
    {
        
        int enterParticleNumber = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        
        Vector3 exitpoint = new Vector3(0,0,0);
        Vector3 entrypoint = new Vector3(0, 0, 0);
        

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
                if (getName.collider.tag == "Barrier")
                {
                    if (ShieldList.TryGetValue(colliderhit, out value))
                    {
                        acoef = ShieldList[colliderhit];
                    }
                }
                
            }

            entrypoint = p.position;

            foreach (RaycastHit hit in hits)
            {

                if (hit.collider.gameObject.name == colliderhit)
                {
                    exitpoint = hit.point;
                    break;
                }
                
            }

            if (Random.Range(0, 1000) < 1000*(1-Mathf.Exp(-acoef*(exitpoint-entrypoint).magnitude)))
            {

                p.velocity = new Vector3(0, 0, 0);
                p.remainingLifetime = 1;      
                
            }
            enter[i] = p;

        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
   
    }

}
