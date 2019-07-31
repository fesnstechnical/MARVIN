using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SourceProfile : MonoBehaviour
{
    public float Activity; //Bq
    public float DoseRate; //(Rem/s)
    Dictionary<string, float> DR = new Dictionary<string, float>();

    public enum Sources { Cs_137, Co_60, K_40 };
    public Sources source;

    void Start()
    {
        DR.Add("Cs_137", (0.38184f/3600f)*1000); //Rem/s at 1cm and 1Ci
        DR.Add("Co_60", (1.37011f/3600f)*1000);  //Rem/s at 1cm and 1Ci
        DR.Add("K_40", (0.081696f/3600f)*1000);  //Rem/s at 1cm and 1Ci

        DoseRate = (DR[source.ToString()]*10) * (Activity/(3.7f*Mathf.Pow(10,7))); //convert Rem/s to mSv/s and scale it with Activity
        DoseRate = DoseRate / 10 * (3.7f * Mathf.Pow(10, 7)); //convert mSv/s to count/s
    }

}
