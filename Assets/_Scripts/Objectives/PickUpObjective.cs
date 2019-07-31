using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PickUpObjective : Objective
{
    [SerializeField]
    GameObject toPickUp;

    // Start is called before the first frame update
    void Start()
    {
        Valve.VR.InteractionSystem.Interactable.onAttachedToHandAction += this.HandleEvent;
    }

    public override void HandleEvent<GameObject>(GameObject pickedUp)
    {
        if (pickedUp.Equals(toPickUp))
        {
            CompleteObjective();
        }
    }
}
