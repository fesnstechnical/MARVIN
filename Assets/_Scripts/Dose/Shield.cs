using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shield : MonoBehaviour {

    //Used to contain information
    public Types shield;
    public string nom;

    private string[] names = new string[] { "Brick" , "Wood" , "Water" , "Concrete (Ordinary)" , "Concrete (ERC Lab)" , "Soft Tissue (General)" , "Polyethylene (Plastic)" , "Mylar" , "Polystryrene (Styrofoam)" , "Lead Glass" , "Lead" , "Aluminium" };

    public string getName() {

        if ( nom != null ) {

            if ( nom.Length > 1 ) {

                return nom;

            }

        }
        
        return names[ ( int ) shield ];

    }

    public Collider getCollider() {

        return GetComponent<Collider>();

    }


    public enum Types {

        Brick,
        Wood,
        Water,
        Concrete_ordinary,
        Concerete_erc,
        Tissue,
        Polyetheylene_plastic,
        Mylar,
        Polystryrene_stryofoam,
        Lead_glass,
        Lead,
        Aluminium

    }

}
