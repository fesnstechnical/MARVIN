using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class QuestionHandler : MonoBehaviour
{
    public string fileName;
    public string folderName = "/File/";    //  by default
    public Text Qtext;
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;
    public Button OptionE;
    private List<int> answers = new List<int>();
    public int Qnum = 0;
    private StreamWriter sw;
    bool write = true;
    private string path;

    void Start() {
        Debug.Log( "hi" );
        path = Application.dataPath + folderName + fileName;    //  store path
        if (File.Exists(path))  //  check if the file already exists
        {
            Debug.Log(folderName + " Exists!");
            sw = new StreamWriter(path, false);
        }
        else { Debug.Log(path + " doesn't exist! Making file"); sw = new StreamWriter(path, true); }    //  if not, make it
        sw.Close();

        OptionA.GetComponentInChildren<Text>().text = "1";
        OptionB.GetComponentInChildren<Text>().text = "2";
        OptionC.GetComponentInChildren<Text>().text = "3";
        OptionD.GetComponentInChildren<Text>().text = "4";
        OptionE.GetComponentInChildren<Text>().text = "5";
    }

  

    // Update is called once per frame
    void Update()
    {
        
        if (Qnum == 0)
        {
            Qtext.GetComponent<Text>().text = "I thought the system/controls were easy to use";
           

        }
        else if (Qnum == 1)
        {
            Qtext.GetComponent<Text>().text = "I feel confident about using the system";
           
        }
        else if (Qnum == 2)
        {
            Qtext.GetComponent<Text>().text = "I understand more about radiation";
          

        }
        else if (Qnum >= 3)
        {
            OptionA.onClick.RemoveAllListeners();
            OptionB.onClick.RemoveAllListeners();
            OptionC.onClick.RemoveAllListeners();
            OptionD.onClick.RemoveAllListeners();
            OptionE.onClick.RemoveAllListeners();

            
            OptionA.GetComponentInChildren<Text>().text = "That's";
            OptionB.GetComponentInChildren<Text>().text = "It.";
            OptionC.GetComponentInChildren<Text>().text = "Now";
            OptionD.GetComponentInChildren<Text>().text = "Get";
            OptionE.GetComponentInChildren<Text>().text = "Out.";
            if ( write ) {
                saveResultsToFile();
                write = false;
            }
        }



    }

    public void AClick (){
        answers.Add(1);
        Qnum++;
    }

    public void BClick()
    {
        answers.Add(2);
        Qnum++;
    }

    public void CClick()
    {
        answers.Add(3);
        Qnum++;
    }

    public void DClick()
    {
        answers.Add(4);
        Qnum++;
    }

    public void EClick() {
        answers.Add( 5 );
        Qnum++;
    }

    //runs into fille access error: access to path "filepath" is denied
    void saveResultsToFile() {
        sw = new StreamWriter( path , false ); //  open file

        sw.WriteLine( "Q1: " + answers[ 0 ] );
        sw.WriteLine( "Q2: " + answers[ 1 ] );
        sw.WriteLine( "Q3: " + answers[ 2 ] );
        sw.WriteLine( "======================" );

        sw.Close(); //  close file
    }
}
