using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class Controller : MonoBehaviour {

    public bool particlesInitiallyEnalbed = true;

    private static Controller controller;

    private DoseController doseController;
    private ScenarioHandler scenarioHandler;
    private GammaGunController gammaGunController;
    private PlatformMover platfomrMover;
    private VisionModeController visionModeController;

    private bool particlesEnabled;

    private bool enableNetworking = false;
    
    // Start is called before the first frame update
    void Start() {
        
        Debug.Log( "Controller online" );

        controller = this;
        doseController = GameObject.Find( "Dose controller" ).GetComponent<DoseController>();
        scenarioHandler = GameObject.Find( "Dose controller" ).GetComponent<ScenarioHandler>();

        if ( GameObject.Find( "End-barrel" ) != null ) {

            gammaGunController = GameObject.Find( "End-barrel" ).GetComponent<GammaGunController>();

        }

        if ( GameObject.Find( "Gamma rail" ) != null ) {
            
            platfomrMover = GameObject.Find( "Gamma rail" ).GetComponent<PlatformMover>();

        }

        visionModeController = this.GetComponent<VisionModeController>();
        

        particlesEnabled = particlesInitiallyEnalbed;
        updateParticleGenerators();

        if ( enableNetworking ) {

            startNetwork();

        }

    }

    private int tick = 0;

    // Update is called once per frame
    void Update() {

        tick++;

        if ( tick == 50 && false ) {
            
            List<DoseBody> doseBoides = doseController.getDoseBodies();

            foreach ( DoseBody doseBody in doseBoides ) {
                
                if ( doseBody.getIsPlayer() ) {

                    streamWrite( "doseRate:" + doseBody.getDoseRate() );
                    streamWrite( "doseTotal:" + doseBody.getAccumulatedDose() );

                }

            }

            tick = 0;


        }
        
    }


    public void toggleParticles() {

        particlesEnabled = !particlesEnabled;

    }

    private void updateParticleGenerators() {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
    
        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<ParticleSystem>() != null ) {

                gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = particlesEnabled;

            }

        }
        
    }



    //=======================================NETWORK======================================================

    private TcpListener listener;
    private Socket socket;
    private Stream stream;
    private bool stopThread = false;
    private Thread readThread;

    private void startNetwork() {

        listener = new TcpListener( 2055 );
        Debug.Log( "Starting server" );

        listener.Start();

        Debug.Log( "Starting thread" );
        Thread thread = new Thread( new ThreadStart( Service ) );
        thread.Start();


    }

    private void Service() {

        socket = listener.AcceptSocket();
        stream = new NetworkStream( socket );

        Debug.Log( "Connected" );

        readThread = new Thread( new ThreadStart( StreamReader ) );
        readThread.Start();

    }

    private void streamWrite( String message ) {
        
        if ( stream != null ) {

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] bufffer = encoder.GetBytes( message + "\n" );

            stream.Write( bufffer , 0 , bufffer.Length );
            stream.Flush();

        }

    }

    private void StreamReader() {

        ASCIIEncoding encoder = new ASCIIEncoding();

        while ( true && !stopThread ) {

            byte[] message = new byte[ 4096 ];
            int bytesRead = stream.Read( message , 0 , 4096 );
            
            string inputText = ( encoder.GetString( message , 0 , bytesRead ) );
            inputText = inputText.Trim();
            
            if ( inputText == "ping" ) {

                streamWrite( "ping" );
         
            }
            else {

                Debug.Log( inputText );

            }

            Thread.Sleep( 200 );

        }

    }

    void OnApplicationQuit() {

        stopThread = true;

        if ( readThread != null ) {

            readThread.Abort();

        }

    }

    public static Controller getController() {

        return controller;

    }

    public DoseController getDoseController() {

        return doseController;

    }

    public ScenarioHandler getScenarioHandler() {

        return scenarioHandler;

    }

    public GammaGunController getGammaGunController() {

        return gammaGunController;

    }

    public PlatformMover getPlatformMover() {

        return platfomrMover;

    }

    public VisionModeController getVisionModeController() {

        return visionModeController;

    }

    //Something is rotten in the sate of Denmark

}
