using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;
using TMPro;
using System;

public class UserPointer : MonoBehaviour {

    private bool menuOpen = false;
    private bool menuObjectFocus = false;
    private bool lastMode = true;

    private bool hudOpen = true;

    private bool laserPointer = false;

    private Canvas menu;
    private Canvas hud;

    private InformationBlock selectedBlock;

    private GameObject[] objectFocused = new GameObject[] {};
    private GameObject[] nonObjectFocused = new GameObject[] { };

    private TextMeshPro hudDoseRate;
    private Slider healthBar;

    public AudioSource audioSource;
    public AudioClip shieldRecharge;
    public AudioClip shieldLow;



    public void Start() {
        


    }

    private void Update() {

        if ( menu == null ) {

            if ( GameObject.Find( "UserInterface" ) != null ) {

                menu = GameObject.Find( "UserInterface" ).GetComponent<Canvas>();

            }

           

        }

        if ( GameObject.Find( "HUD" ) != null ) {

            hud = GameObject.Find( "HUD" ).GetComponent<Canvas>();
       
        }

        if ( GameObject.Find( "HUD HealthBar" ) != null ) {

            healthBar = GameObject.Find( "HUD HealthBar" ).GetComponent<Slider>();

        }

        hudDoseRate = GameObject.Find( "HUD Dose Rate" ).GetComponent<TextMeshPro>();

        if ( objectFocused.Length == 0 ) {

            GameObject Button_InterfaceOption1 = GameObject.Find( "Button_InterfaceOption1" ); //But arrays start at 0 >:(
            GameObject Button_InterfaceOption2 = GameObject.Find( "Button_InterfaceOption2" );
            GameObject Button_InterfaceOption3 = GameObject.Find( "Button_InterfaceOption3" );

            GameObject TextBox_Information = GameObject.Find( "TextBox_Information" );

            GameObject Button_Menu_NormalVision = GameObject.Find( "Button_Menu_NormalVision" );
            GameObject Button_Menu_XRayVision = GameObject.Find( "Button_Menu_XRayVision" );

            GameObject Button_Menu_Reset = GameObject.Find( "Button_Menu_Reset" );


            objectFocused = new GameObject[] { Button_InterfaceOption1 , Button_InterfaceOption2 , Button_InterfaceOption3 , TextBox_Information };
            nonObjectFocused = new GameObject[] { Button_Menu_NormalVision , Button_Menu_XRayVision , Button_Menu_Reset };

        }


        if ( menu != null ) {

            if ( SteamVR_Actions._default.Menu.GetStateUp( SteamVR_Input_Sources.LeftHand ) ) { //Left hand opens & closes the menu

                toggleMenu();

            }

        }


        if ( SteamVR_Actions._default.Menu.GetStateUp( SteamVR_Input_Sources.RightHand ) ) { //Right hand controls laser pointer

            laserPointer = !laserPointer;
            

        }



        if ( menuOpen ) {
            
            if ( lastMode != menuObjectFocus ) {
                
                lastMode = menuObjectFocus;
                
                foreach ( GameObject gameObject in objectFocused ) {
                    
                    gameObject.SetActive( menuObjectFocus );

                }

                foreach ( GameObject gameObject in nonObjectFocused ) {

                    gameObject.SetActive( !menuObjectFocus );

                }

                if ( objectFocused.Length >= 3 && menuObjectFocus && selectedBlock != null ) {

                    GameObject[] buttons = new GameObject[] { objectFocused[ 0 ] , objectFocused[ 1 ] , objectFocused[ 2 ] };
                    for ( int i = 0 ; i < buttons.Length ; i++ ) {

                        buttons[ i ].SetActive( false );

                    }

                    if ( selectedBlock.executables != null ) {

                        //Set the button texts
                        for ( int i = 0 ; i < ( selectedBlock.executables.Count <= 3 ? selectedBlock.executables.Count : 3 )  ; i++ ) {

                            buttons[ i ].SetActive( true );
                            buttons[ i ].GetComponentInChildren<Text>().text = selectedBlock.executables[ i ].buttonText;

                        }

                    }

                    objectFocused[ 3 ].GetComponentInChildren<Text>().text = selectedBlock.information;
                    

                }

            }

        }

        if ( laserPointer ) {
            
            GetComponent<SteamVR_LaserPointer>().thickness = 0.002f;

            RaycastHit hit;
            Ray raycast = new Ray( transform.position , transform.forward );


            if ( SteamVR_Actions._default.InteractUI.GetStateUp( SteamVR_Input_Sources.RightHand ) ) {

                bool noHit = true;


                if ( Physics.Raycast( raycast , out hit , 1000.0f ) ) {

                    if ( hit.transform != null ) {

                        GameObject hitObject = hit.transform.gameObject;

                        if ( hitObject.GetComponent<Button>() != null ) {

                            hit.transform.GetComponent<Button>().onClick.Invoke();
                            noHit = false;

                        }
                        else {

                            GameObject informationBlockObject = recurseFindInformationBlock( hitObject );
                            if ( informationBlockObject != null ) {

                                selectedBlock = informationBlockObject.GetComponent<InformationBlock>();
                                menuObjectFocus = true;

                                Controller.getController().getVisionModeController().setInformationBlockHighlightState( false );

                                noHit = false;

                            }

                        }

                    }

                }

                if ( noHit ) {
                    
                    menuObjectFocus = false;
                    selectedBlock = null;

                }

            }

        }
        else {
            
            GetComponent<SteamVR_LaserPointer>().thickness = 0.0f;
        

        }
        
        if ( hud != null ) {

            updateHUD();


        }

    }

    private bool isTakingSignificantDose = false;
    private float minDoseTriggerRate = 10; //uSv
    private float barSpeed = 0.01f;
    private float healthBarProgression = 1;

    private void updateHUD() {
        
        float doseAccumulated = Controller.getController().getUser().getAccumulatedDose();
        float doseRate = Controller.getController().getUser().getDoseRate() * 1e3f;

        float setValue = healthBar.value;

        if ( doseRate > minDoseTriggerRate ) {
            
            if ( !isTakingSignificantDose ) {

                isTakingSignificantDose = true;
                playSound( shieldLow , true );

            }

            setValue -= barSpeed;
            if ( setValue < 0 ) { setValue = 0; }

        }
        else {

            if ( isTakingSignificantDose ) {

                stopSound();
                playSound( shieldRecharge , false );
                isTakingSignificantDose = false;
                
            }

            setValue += barSpeed;
            if ( setValue > 1 ) { setValue = 1; }

        }


        healthBar.value = setValue;

        //Text

        hudDoseRate.text = ( doseRate.ToString( "F2" ) + " uSv/hr" );

    }

    public void toggleMenu() {

        menuOpen = !menuOpen;

        menu.enabled = menuOpen;
        hud.enabled = !hud.enabled;

        menu.GetComponentInChildren<BoxCollider>().enabled = menuOpen;


        if ( menuOpen ) { menuObjectFocus = false; }

        Controller.getController().getVisionModeController().setInformationBlockHighlightState( menuOpen );

    }

    void SendMessage( string message ) {

        if ( selectedBlock != null ) {

            if ( message == "Button_InterfaceOption1" ) {
                
                GameObject.Find( "Dose controller" ).GetComponent<MenuInterpretor>().sendMessage( selectedBlock.executables[ 0 ].controllerTriggerName );

            }
            else if ( message == "Button_InterfaceOption2" ) {

                GameObject.Find( "Dose controller" ).GetComponent<MenuInterpretor>().sendMessage( selectedBlock.executables[ 1 ].controllerTriggerName );

            }
            else if ( message == "Button_InterfaceOption3" ) {

                GameObject.Find( "Dose controller" ).GetComponent<MenuInterpretor>().sendMessage( selectedBlock.executables[ 2 ].controllerTriggerName );


            }

        }

    }

    private GameObject recurseFindInformationBlock( GameObject baseObject ) {

        if ( baseObject.GetComponent<InformationBlock>() != null ) {

            return baseObject;

        }
        else {

            if ( baseObject.transform.parent != null ) {

                return recurseFindInformationBlock( baseObject.transform.parent.gameObject );

            }
            else {

                return null;

            }

        }

    }

    private void stopSound() {

        if ( audioSource != null ) {
            
            audioSource.Stop();

        }

    }

    private bool isPlaying() {

        if ( audioSource!= null ) {

            return audioSource.isPlaying;

        }

        return false;

    }

    private void playSound( AudioClip clip , bool loop ) {

        if ( clip != null && audioSource != null ) {

            stopSound();

            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();

        }

    }

}
