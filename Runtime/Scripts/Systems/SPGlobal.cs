using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SPGlobal : MonoBehaviour
{
    public static SPGlobal I;
    public static bool IsMobile;
    public static bool FirstFrame = true; 
    public static bool IsDebug{get{return I.debug && Application.platform != RuntimePlatform.WebGLPlayer;}}
    public static bool DebugItems{get{return I.debugItems;}}
    public static bool Beta{get{return I.beta;}}
    public static bool Testnet{get{return I.testnet;}}
    public static bool Updating {get{return true;}}
    public static bool LocalPlayer {get{return true;}}
    
    [SerializeField] protected bool debug = false;
    [SerializeField] protected bool debugItems = false;
    [SerializeField] protected bool beta = false;
    [SerializeField] protected bool testnet = false;

    void Awake() {
        
        if(I != null) {
            Destroy(gameObject);
        } else {
            I = this;
        }

        if(Application.isEditor || Debug.isDebugBuild) {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 30;
        }

        if(Application.platform == RuntimePlatform.WindowsPlayer) {
            //BorderlessWindow.SetFramelessWindow();
            //BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width, Screen.height);
        } else {

        }

        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).SendMessage("Awake", SendMessageOptions.DontRequireReceiver); 
        }

        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).SendMessage("Init", SendMessageOptions.DontRequireReceiver); 
        }

    }



    void Update() {
        FirstFrame = false; 
    }


    
    #if UNITY_EDITOR
    [MenuItem("Engine/Find Player &w")]
    static void FindPlayer()
    {
        SPPlayer player;

        if(Application.isPlaying) {
            player = SPPlayer.LocalPlayer;
        } else {
            player = FindObjectOfType<SPPlayer>(); 
        }

        if(!player) {return;}

        Selection.activeGameObject = player.gameObject;

    }
    #endif
}
