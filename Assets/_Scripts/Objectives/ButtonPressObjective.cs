using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ButtonPressObjective : Objective
{
    [SerializeField]
    Valve.VR.SteamVR_Action_Boolean actionObjective;

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    public override void HandleEvent<T>(T obj)
    {
        throw new NotImplementedException();
    }

    void CheckInput()
    {
        if (actionObjective.GetStateDown(Valve.VR.SteamVR_Input_Sources.Any))
        {
            CompleteObjective();
        }
    }
}
