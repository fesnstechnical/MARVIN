using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//this script loads the NRU scene.
//it slows unity down to a crawl after however...
public class SceneTransfer : MonoBehaviour
{
  
    public void NRUTransfer()
    {
        SceneManager.LoadScene("NRU");
    }
}
