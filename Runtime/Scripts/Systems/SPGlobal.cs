using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum DevMode {Debug, Release}
public class SPGlobal : MonoBehaviour
{
    public static System.Action<bool> OnDebug;

    public static SPGlobal I;
    public static bool IsQuitting = false;
    public static bool IsMobile;
    public static bool FirstFrame = true; 
    public static bool IsDebug{get{return I.Debug ;}}
    public static bool Updating {get{return true;}}
    public static bool LocalPlayer {get{return true;}}
    public bool Debug { 
        get { return debug; } 
        set { debug = value && I.devMode != DevMode.Release; OnDebug?.Invoke(debug); } 
    }

    [SerializeField] DevMode devMode;
    [SerializeField] bool debug = false;

    void Awake() {

        Application.quitting += Quit;

        if(I != null) {
            Destroy(gameObject);
        } else {
            I = this;
        }

        if(Application.isEditor || UnityEngine.Debug.isDebugBuild) {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
        } else {
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 1;
        }
        
        Application.runInBackground = true;

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

        ToggleDebug(debug);

    }

    public static void ToggleDebug(bool toggle) {
        I.Debug = toggle;
    }

    void OnDestroy() {
        I = null;
        Application.quitting -= Quit;
        Quit();
    }

    void Quit() {
        IsQuitting = true;
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
