using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoseReceptor_Minh : MonoBehaviour
{
    public float doseRateTotal = 0.0f;
    public float TotalDose = 0.0f;
    public GameObject[] Sources;
    float deltaD;
    Dictionary<string, float> DoseRate = new Dictionary<string, float>();
    float percentSurvive;

    Vector3 exitpoint = new Vector3(0, 0, 0);
    Vector3 entrypoint = new Vector3(0, 0, 0);
    Vector3 rayDirection = new Vector3(0, 0, 0);
    Vector3 ReceptorPosition = new Vector3(0, 0, 0);
    float distance = 0.0f;
    public List<RaycastHit> hitList = new List<RaycastHit>();
    
    RaycastHit[] hits;


    Dictionary<string, float> DB = new Dictionary<string, float>();
    Dictionary<string, float> ShieldList = new Dictionary<string, float>();

    float time;
    float value = 0;

    void Start()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 50);
        List<GameObject> sceneObj = new List<GameObject>();

        Sources = GameObject.FindGameObjectsWithTag("Source");
        DB = GameObject.Find("Attenuation Database").GetComponent<AttenuationDB>().acoef;

        foreach (GameObject source in Sources)
        {
            DoseRate.Add(source.name, source.GetComponent<SourceProfile>().DoseRate);
            //Debug.Log("source: " + source.name + "  DoseRate: " + DoseRate[source.name]);
        }

        int numbarrier = 0;
        int newnum = hitColliders.Length;
        for (int i = 0; i < hitColliders.Length; i++)
        {


            if (hitColliders[i].gameObject.tag == "Barrier")
            {
                sceneObj.Add(hitColliders[i].gameObject);
                numbarrier++;
            }
        }

        //foreach (KeyValuePair<string, float> i in DoseRate)
        //{
        //    Debug.Log("key: " + i.Key + " value: " + i.Value);
        //}


        for (int i = 0; i < sceneObj.Count; i++)
        {
            if (sceneObj[i].GetComponent<Shield>() != null)
            {


                if (DB.TryGetValue(sceneObj[i].GetComponent<Shield>().getName(), out value))
                {
                    float another=0;
                    if (!ShieldList.TryGetValue(sceneObj[i].gameObject.name, out another))
                    {
                        ShieldList.Add(sceneObj[i].gameObject.name, value);
                        //Debug.Log(sceneObj[i].name + ": "+ ShieldList[sceneObj[i].gameObject.name]);
                    }

                }

            }


        }


    }

    void FixedUpdate()
    {
        if (time > 0.3f)
        {
            UpdateDoseRate();
            time = 0;
        }

        time += Time.deltaTime;
        
    }

    void UpdateDoseRate()
    {
        doseRateTotal = 0;
        string colliderhit;

        foreach (GameObject source in Sources)
        {
            rayDirection = (source.transform.position - this.transform.position).normalized;
            distance = (this.transform.position - source.transform.position).magnitude;
            ReceptorPosition = this.transform.position;

            if (distance < 5.0f)
            {

            
                hits = Physics.RaycastAll(ReceptorPosition, rayDirection, distance + 0.5f);
                hitList.Clear();
                hitList = new List<RaycastHit>(hits);
                hitList = hitList.OrderBy(x => Vector3.Distance(ReceptorPosition, x.transform.position)).ToList();

                //Debug.DrawLine(this.transform.position, source.transform.position);
                //Debug.DrawRay(this.transform.position, rayDirection);
               // Debug.Log(source.name);
                for (int i = 0; i < hitList.Count - 1; i++)
                {
                    //Debug.DrawLine(hitList[i].point, hitList[i].point + Vector3.up * 5, Color.green);
                    //Debug.Log(hitList[i].collider.name);
                    if (hitList[i].collider.tag == "Barrier")
                    {
                        
                        entrypoint = hitList[i].point;
                    

                        Ray ray = new Ray(hitList[i + 1].point, -rayDirection);
                        RaycastHit getExit;
                        if (Physics.Raycast(ray, out getExit, (ReceptorPosition - source.transform.position).magnitude))
                        {
                            if (getExit.collider.name == hitList[i].collider.name)
                            {
                                colliderhit = getExit.collider.name;
                                //Debug.Log("Exit:" + getExit.collider.name);
                                exitpoint = getExit.point;
                                //Debug.DrawLine(exitpoint, exitpoint + Vector3.up * 5, Color.blue);
                                //Debug.DrawLine(entrypoint, exitpoint, Color.red);
                                //Debug.Log("entry: " + entrypoint+ " exit: "+getExit.point);
                                if (ShieldList.TryGetValue(colliderhit, out value))
                                {
                                    percentSurvive += ShieldList[colliderhit] * (entrypoint - exitpoint).magnitude;
                                    //Debug.Log("Got Attenuation: " + ShieldList[colliderhit]);
                                }
                            }

                        }


                    }

                }

                percentSurvive = Mathf.Exp(-percentSurvive);
                deltaD = (this.transform.position - source.transform.position).magnitude;
                doseRateTotal += (DoseRate[source.name] / (deltaD * deltaD))* percentSurvive;
                //Debug.Log("Source: " + source.name + " distance: "+(deltaD));
                //Debug.Log("Source: " + source.name + " %attenution: " + (1-percentSurvive));
                percentSurvive = 0.0f;
            }
        }
        doseRateTotal += Random.Range(-doseRateTotal*0.10f, doseRateTotal*0.10f);
        TotalDose += doseRateTotal;
        
    }

}
