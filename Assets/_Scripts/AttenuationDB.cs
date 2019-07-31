using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttenuationDB : MonoBehaviour
{
    
    public Dictionary<string, float> acoef = new Dictionary<string, float>();
    

    void Start()
    {

        
        string[] materialNames = Shield.names; //Material names
        foreach ( string materialName in materialNames ) {
            
            acoef.Add( materialName , Controller.getController().getDoseController().getAverageMaterialAttenuationCoefficient( materialName ) );


        }
        
        /**
        acoef.Add("Brick", 1.0f);
        acoef.Add("Wood", 1.0f);
        acoef.Add("Water", 1.0f);
        acoef.Add("Concrete_ordinary", 2.0f);
        acoef.Add("Concerete_erc", 3.0f);
        acoef.Add("Tissue", 1.0f);
        acoef.Add("Polyetheylene_plastic", 1.0f);
        acoef.Add("Mylar", 1.0f);
        acoef.Add("Polystryrene_stryofoam", 1.0f);
        acoef.Add("Lead_glass", 2.5f);
        acoef.Add("Lead", 4.0f);
        acoef.Add("Aluminium", 1.0f); */


        //foreach (KeyValuePair<string, float> i in acoef)
        //{
        //    Debug.Log("key: " + i.Key + " value: " + i.Value);


        //}

    }

    
}
