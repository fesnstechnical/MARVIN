using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeigerControler_Minh : MonoBehaviour
{
    public Transform doseReceptor;
    public TextMesh displayText;
    public float displayDoseRate;
    char[] Prefixes = new char[8] { 'G','M','K',' ','m','u', 'n', 'p' }; 
    float time = 0.0f;
    int pre = 0;
    float num = Mathf.Pow(10, 9);
    public int mode = 0;

    void Start()
    {
        displayText.text = "On";
    }

    void Update()
    {
        if (time > 0.03f)
        {
            
            if (mode == 0)
            {
                //ToCPS
                displayDoseRate = doseReceptor.GetComponent<DoseReceptor_Minh>().doseRateTotal;
                if (displayDoseRate < 999)
                {
                    displayText.text = displayDoseRate.ToString("000") + "\n"  + "cps";
                }
                else
                {
                    Convert();
                    displayText.text = displayDoseRate.ToString("000.00") + "\n" + Prefixes[pre] + "cps";
                }
                
            }
            else if (mode == 1)
            {
                //ToRem/s
                displayDoseRate = doseReceptor.GetComponent<DoseReceptor_Minh>().doseRateTotal;
                displayDoseRate = displayDoseRate / ((3.7f * Mathf.Pow(10, 7)));
                Convert();
                displayText.text = displayDoseRate.ToString("000.0") + "\n" + Prefixes[pre] + "Rem/s";
            }
            else if (mode == 2)
            {
                //ToSv/s
                displayDoseRate = doseReceptor.GetComponent<DoseReceptor_Minh>().doseRateTotal;
                displayDoseRate = (displayDoseRate / ((3.7f * Mathf.Pow(10, 7)))) / (100);
                Convert();
                displayText.text = displayDoseRate.ToString("000.0") + "\n" + Prefixes[pre] + "Sv/s";
            }
            else if (mode == 3)
            {
                //ToBED
                displayDoseRate = doseReceptor.GetComponent<DoseReceptor_Minh>().doseRateTotal;
                displayDoseRate = (displayDoseRate / ((3.7f * Mathf.Pow(10, 7)))) / (100) * Mathf.Pow(10, 7);
                Convert();
                displayText.text = displayDoseRate.ToString("000.0") + "\n" + Prefixes[pre] + "BED/s";
            }

        }
        time += Time.deltaTime;
    }

    void Convert()
    {
        num = Mathf.Pow(10, 9);
        pre = 0;

        for (int i = 0; i < 7; i++)
        {

            if (displayDoseRate / num < 999 && displayDoseRate / num > 1)
            {
                displayDoseRate = displayDoseRate / num;
                i = 7;
            }
            else
            {
                num /= 1000;
                pre++;
            }

        }

    }

}
