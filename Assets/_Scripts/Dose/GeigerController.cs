

using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GeigerController : DoseBody
{
    [SerializeField]
    private bool pickedUp = false;
    [SerializeField]
    private bool active = false;
    private Transform toolTransform;
    private Interactable interactable;
    private Rigidbody toolBody;


    private Collider collider;

    private Transform handTransform;
    private Hand hand;

    private GameObject handRenderPrefab;

    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();

    //Components
    TextMesh doseTextMesh;
    AudioSource audioSourceGeigerClick;
    AudioSource audioSourceEffects;
    public MeshRenderer meshRendererStatusSphere;

    //Settings
    private string[] modes = { "CPS", "CPM", "Sv/hr" };
    private float[] scales = { 0.001f, 1f, 1000f, 10e6f };
    private string[] prefixes = { "m", "", "k", "M" };

    int mode = 1;
    int scale = 2;

    [SerializeField]
    [Tooltip("Displayed readings will fluctuate by a maximum of this percentage (up or down) from the actual amount")]
    float readingErrorPercentage = 0.1f;

    [SerializeField]
    [Tooltip("Displayed reading error is smoothed by a this percentage each frame. At 1 then they do not fluctuate. At 0 then they change at random each frame.")]
    float readingErrorSmoothing = 0.5f;

    private float readingErrorMultiplier = 1.0f;

    [SerializeField]
    float displayVal = 0.0f;


    public override void secondaryStart()
    {

        toolTransform = GetComponent<Transform>();
        interactable = GetComponent<Interactable>();
        toolBody = GetComponent<Rigidbody>();

        //handTransform = rightHand.GetComponent<Transform>();
        //hand = rightHand.GetComponent<Hand>();

        doseReceptors.Add(new DoseReceptor(1, 0.02f, toolTransform));
        efficiency = 0.025f;

        //components
        doseTextMesh = GetComponentInChildren<TextMesh>();
        audioSourceGeigerClick = GetComponents<AudioSource>()[1];
        audioSourceEffects = GetComponents<AudioSource>()[0];

        doseTextMesh.text = "";

        collider = GetComponent<BoxCollider>();


    }

    public bool getActive()
    {

        return active;

    }


    //On a scale of 1 to 100
    public float getIntensity()
    {
        float value = getValue();

        if (value > 1000)
        {

            value = 1000;

        }

        return value / 10;

    }

    public float getValue()
    {

        float value;

        if (mode == 0 || mode == 1)
        { //Counts

            value = getCountRate();

            if (mode == 1)
            {

                value = value / 60; //CPS to CPM

            }

        }
        else
        { //Dose

            value = getDoseRate() / 1000;

        }

        value = value / scales[scale];

        return value;

    }

    // adds some randomness to readings
    public float getValueWithRandomErrorWithRandomError()
    {
        float newRandomError = Random.Range(-1.0f, 1.0f) * readingErrorPercentage;
        readingErrorMultiplier = Mathf.Lerp(newRandomError, readingErrorMultiplier, readingErrorSmoothing);

        float value;

        if (mode == 0)
        {
            value = getCountRate();
        }
        else if (mode == 1)
        {
            value = getCountRate() / 60; //CPS to CPM
        }
        else
        { //Dose
            value = getDoseRate() / 1000;
        }

        // apply reading scaling
        value = value / scales[scale];

        // apply error
        value += (readingErrorMultiplier * value);

        return value;
    }



    // Update is called once per frame
    void Update()
    {
        

        if (pickedUp)
        {
            if(handTransform != null)
            {
                toolTransform.position = handTransform.position;
                toolTransform.rotation = handTransform.rotation * Quaternion.Euler(0, -90, 0);
            }
        }

        if (active)
        {

            displayVal = getValueWithRandomErrorWithRandomError();


            if (displayVal >= 0)
            {

                doseTextMesh.text = ((int)displayVal) + "\n" + prefixes[scale] + modes[mode]; //Cast to int so we dont get long decimals

            }
            else if (displayVal < -10)
            {

                doseTextMesh.text = ("LUDICROUS\n" + prefixes[scale] + modes[mode]); //Prepare ship, ..., for LUDICROUS speed. What's the matter colonel Sanders, chicken?
                //https://youtu.be/mk7VWcuVOf0?t=46

            }
            else
            {

                doseTextMesh.text = (0) + "\n" + prefixes[scale] + modes[mode]; //Cast to int so we dont get long decimals

            }

        }
        else
        {

            doseTextMesh.text = "";

        }

        checkInputs();

    }

    private float totalTime = 0f;

    private void checkInputs()
    {

        if (pickedUp)
        {

            float inputTrigger = SteamVR_Actions._default.Squeeze.GetAxis(SteamVR_Input_Sources.Any);

            bool triggerDown = inputTrigger == 1f;

            if (triggerDown)
            {

                totalTime += Time.deltaTime;

            }

            if (!triggerDown)
            {

                if (totalTime != 0)
                {

                    audioSourceEffects.Play();

                    if (totalTime < 1f)
                    {

                        //Change modes
                        if ((mode + 1) >= modes.Length)
                        {

                            mode = 0;

                        }
                        else
                        {

                            mode++;

                        }

                    }
                    else
                    { //Long click

                        //Change scale
                        if ((scale + 1) >= scales.Length)
                        {

                            scale = 0;

                        }
                        else
                        {

                            scale++;

                        }

                    }


                }

                totalTime = 0f;

            }

        }

    }

    private void updateActive()
    {

        if (active)
        {

            GameObject rightHand = GameObject.Find("RightHand");
            GameObject leftHand = GameObject.Find("LeftHand");

            if ((rightHand.GetComponent<Transform>().position - toolTransform.position).magnitude < (leftHand.GetComponent<Transform>().position - toolTransform.position).magnitude)
            {

                //Right hand is closest, so that must've picked it up
                handTransform = rightHand.GetComponent<Transform>();
                hand = rightHand.GetComponent<Hand>();

            }
            else
            {

                handTransform = leftHand.GetComponent<Transform>();
                hand = leftHand.GetComponent<Hand>();

            }

            //Play active sound, change color, turn on text mesh
            //meshRendererStatusSphere.material.SetColor("_Color", Color.green);
            collider.enabled = false; //Disabling the colliders since most people will put the geiger counter close to the source, and it'll launch it in the air

        }
        else
        {

            //meshRendererStatusSphere.material.SetColor("_Color", Color.black);
            collider.enabled = true;

        }

    }

    private void updateTransform()
    {

        if (pickedUp)
        {

            toolBody.useGravity = false;
            interactable.highlightOnHover = false;
            toolTransform.SetParent(handTransform);

            handRenderPrefab = hand.renderModelPrefab;

            //handRenderPrefab.GetComponent<MeshRenderer>().enabled = false;
            //hand.SetRenderModel(handRenderPrefab);



        }
        else
        {
            Debug.Log( "what" );
            toolTransform.SetParent(null);

            toolBody.useGravity = true;
            interactable.highlightOnHover = true;
            toolTransform.position = handTransform.position;

            //hand.SetRenderModel(handRenderPrefab);

        }


    }

    void SendMessage(string message)
    {

        if (message == "Pickup")
        {

            if (!pickedUp)
            {

                active = true;
                updateActive();
                pickedUp = true;
            }

        }
        else if (message == "Drop")
        {

            if (pickedUp)
            {
                active = false;
                updateActive();
                pickedUp = false;
            }

            updateTransform();

        }

    }

    public override List<DoseReceptor> getDoseReceptors()
    {

        return doseReceptors;

    }
}
