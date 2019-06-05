using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ERCTransfer : MonoBehaviour
{
    public void ERCSceneTransfer() {
        SceneManager.LoadScene( "ERC_Basement" );
    }
}
