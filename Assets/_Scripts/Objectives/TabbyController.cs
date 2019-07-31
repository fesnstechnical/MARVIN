using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabbyController : MonoBehaviour
{
    [SerializeField]
    Canvas tabbyCanvas;

    [SerializeField]
    TextMeshProUGUI objectiveName;

    [SerializeField]
    TextMeshProUGUI objectiveDescription;

    [SerializeField]
    ObjectiveManager objectiveManager;

    // Start is called before the first frame update
    void Start()
    {
        ObjectiveManager.OnAdvanceObjectives += UpdateObjectiveText;
        UpdateObjectiveText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateObjectiveText()
    {
        objectiveName.text = objectiveManager.GetCurrentObjective().GetName();
        objectiveDescription.text = objectiveManager.GetCurrentObjective().GetDescription();
    }
}
