using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuTransfer : MonoBehaviour
{
    public void MenuSceneTransfer() {
        SceneManager.LoadScene( "menutest" );
    }
}
