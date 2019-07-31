using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class TeleportObjective : Objective
{
    [SerializeField]
    GameObject waypointObjective;

    void Start()
    {
        //Adding functions to run when a 
        Teleport.OnTeleportEvent += HandleEvent;
    }

    public override void HandleEvent<GameObject>(GameObject go)
    {
        if (go.Equals(waypointObjective))
        {
            CompleteObjective();
        }
    }
}
