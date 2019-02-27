using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestionHandler : MonoBehaviour
{

    public Text Qtext;
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;
    private List<char> answers = new List<char>();
    public int Qnum = 0;
    void Start()
    {
        OptionA.onClick.AddListener(() => { AClick(); });
        OptionB.onClick.AddListener(() => { BClick(); });
        OptionC.onClick.AddListener(() => { CClick(); });
        OptionD.onClick.AddListener(() => { DClick(); });
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Qnum == 0)
        {
            Qtext.GetComponent<Text>().text = "BEEP";
            OptionA.GetComponentInChildren<Text>().text = "Yes";
            OptionB.GetComponentInChildren<Text>().text = "No";
            OptionC.GetComponentInChildren<Text>().text = "What";
            OptionD.GetComponentInChildren<Text>().text = "DingleBerry!";

        }
        else if (Qnum == 1)
        {
            Qtext.GetComponent<Text>().text = "BOOP";
            OptionA.GetComponentInChildren<Text>().text = "es";
            OptionB.GetComponentInChildren<Text>().text = "o";
            OptionC.GetComponentInChildren<Text>().text = "hat";
            OptionD.GetComponentInChildren<Text>().text = "ingleBerry!";
        }
        else if (Qnum == 2)
        {
            Qtext.GetComponent<Text>().text = "BliP";
            OptionA.GetComponentInChildren<Text>().text = "Yes";
            OptionB.GetComponentInChildren<Text>().text = "yes";
            OptionC.GetComponentInChildren<Text>().text = "yEs";
            OptionD.GetComponentInChildren<Text>().text = "DongleBerry!";
        }
        else if (Qnum == 3)
        {
            Qtext.GetComponent<Text>().text = "BORP";
            OptionA.GetComponentInChildren<Text>().text = "No";
            OptionB.GetComponentInChildren<Text>().text = "No";
            OptionC.GetComponentInChildren<Text>().text = "No";
            OptionD.GetComponentInChildren<Text>().text = "DinkleBerg!";
        }
        else if (Qnum >= 4)
        {
            OptionA.onClick.RemoveAllListeners();
            OptionB.onClick.RemoveAllListeners();
            OptionC.onClick.RemoveAllListeners();
            OptionD.onClick.RemoveAllListeners();
            
            Qtext.GetComponent<Text>().text = answers[0] + " " + answers[1] + " " + answers[2] + " " + answers[3];
            OptionA.GetComponentInChildren<Text>().text = "That's";
            OptionB.GetComponentInChildren<Text>().text = "It.";
            OptionC.GetComponentInChildren<Text>().text = "Get";
            OptionD.GetComponentInChildren<Text>().text = "Out.";
        }



    }

    void AClick (){
        answers.Add('A');
        Qnum++;
    }

    void BClick()
    {
        answers.Add('B');
        Qnum++;
    }

    void CClick()
    {
        answers.Add('C');
        Qnum++;
    }

    void DClick()
    {
        answers.Add('D');
        Qnum++;
    }
}
