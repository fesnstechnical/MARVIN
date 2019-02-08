using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionHandler : MonoBehaviour
{
    /*
         * List of Missions:
         * [0] - testing
         * 
     */

    public List<Mission> missions;

    private void Awake()
    {
        //  initialize all events here
        missions.Add(new Mission());  //  make new event stack
        missions[0].objectives.Add(new Event()); //  make new event
        missions[0].name = "testing";
        missions[0].objectives[0].eventDescription = "hi, i am an objective!";
    }
}
public class Mission    //  public for the sake of the events list. not meant to be called as is
{
    public List<Event> objectives;
    public string name;
    public bool isComplete, isActive; //  isComplete and isActive CANNOT both be true at the same time
}

public class Event  //  public for the sake of the events list. not meant to be called as is
{
    public string eventDescription;
    public bool isComplete, isActive;   //  isComplete and isActive CANNOT both be true at the same time
    public List<GameObject> waypoints;
    public float waypointRadius = 5.0f;  //  radius around a waypoint
}