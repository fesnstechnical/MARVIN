using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CollisionObjective : Objective
{
    [SerializeField]
    GameObject objectOfInterest;

    [SerializeField]
    bool specificObject = true;

    [SerializeField]
    List<GameObject> collisionObjectives;

    // Start is called before the first frame update
    void Start()
    {
        objectOfInterest.AddComponent<CollisionEvent>();
        CollisionEvent.OnCollisionAction += this.HandleEvent;
    }

    public override void HandleEvent<GameObject>(GameObject collidedWith)
    {
        if (specificObject)
        {
            for (int i = 0; i < collisionObjectives.Count; i++)
            {
                if (collisionObjectives[i].Equals(collidedWith))
                {
                    CompleteObjective();
                    return;
                }
            }
        }

        else
        {
            CompleteObjective();
        }
    }
}
