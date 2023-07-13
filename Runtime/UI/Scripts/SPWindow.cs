using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SPWindow : MonoBehaviour
{
    public bool Active {get{return gameObject.activeInHierarchy;}}
    public SPWindowTheme Theme {get{return themeLocal != null? themeLocal : GlobalTheme;}}
    public static SPWindowTheme GlobalTheme {get{if(globalTheme == null) globalTheme = new SPWindowTheme(); return globalTheme;}}
    protected static SPWindowTheme globalTheme;

    //add some delegate so changing theme changes all windows or something
    public static void SetThemeGlobal(SPWindowTheme newTheme) {globalTheme = newTheme;}
    public void SetThemeLocal(SPWindowTheme newTheme) {themeLocal = newTheme; UpdateColor();}
    public RectTransform Rect {get{return rect;}}
    public System.Action<bool> OnToggleWindow;

    private static Hashtable uniqueUI;

    [Header("Window")]
    public bool isolateDebug = false;
    [SerializeField] protected string uniqueUITag = null;
    [SerializeField] protected Image border;
    [SerializeField] protected Image bg;
    [SerializeField] protected RectTransform rect;
    [SerializeReference] protected Graphic [] _graphics;
    [SerializeReference] protected TextMeshProUGUI [] _texts;
    [SerializeReference] protected Image [] _images;
    [System.NonSerialized] protected SPWindowTheme themeLocal = null;

    [Header("Fields")]
    [SerializeField] protected bool hasInit = false;
    public bool HasInit{get{return hasInit;}}


    public void Log(string log) {
        if(isolateDebug) Debug.Log(log, gameObject);
    }
    public void LogError(string log) {
        if(isolateDebug) Debug.LogError(log, gameObject);
    }

    public virtual void Init() {

        if(hasInit) {
            // Debug.LogError(gameObject.name + ": Double init", gameObject);
            return;
        }

        if(!rect) rect = GetComponent<RectTransform>();

        RegisterUI(true,uniqueUITag);

        UpdateColor();
        
        hasInit = true;
    }

    protected virtual void Destroy() {

        RegisterUI(false, uniqueUITag);

    }



    //sometimes we invert the buttons when they are inside other buttons for visibility
    protected Color graphicsColor = Color.black;
    protected Color bgColor = Color.black;
    protected Color borderColor = Color.black;

    public virtual void UpdateColor() {
        
        bgColor = Theme.defaultTheme.bgColor;
        borderColor = Theme.defaultTheme.color;
        graphicsColor = Theme.defaultTheme.color;

        ApplyGraphics();
    }

    public virtual void UpdateColor(SPWindowTheme.SPTheme newTheme) {

        bgColor = newTheme.bgColor;
        graphicsColor = newTheme.color;
        borderColor = newTheme.color;

        ApplyGraphics();
    }

    public virtual void ApplyGraphics() {
        if(bg) {bg.color = bgColor;} 
        if(border) {border.color = borderColor;}

        for(int i = 0; i < _graphics.Length; i++) { _graphics[i].color = graphicsColor;}
        for(int i = 0; i < _texts.Length; i++) { _texts[i].color = graphicsColor;}
        for(int i = 0; i < _images.Length; i++) {_images[i].color = graphicsColor;}
    }

    protected void RegisterUI(bool toggle, string uniqueUITag) {

        if(string.IsNullOrEmpty(uniqueUITag)) {
            return;
        }


        if(uniqueUI == null) {
            uniqueUI = new Hashtable();
        }

        if(toggle) {
            
            if(uniqueUI.Contains(uniqueUITag)) {
                LogError("Duplicate UI tag on " + gameObject.name);
                return;
            }

            uniqueUI.Add(uniqueUITag, this);

        } else {

            if(!uniqueUI.Contains(uniqueUITag)) {
                LogError("No UI tag to remove on " + gameObject.name);
                return;
            }

            uniqueUI.Remove(uniqueUITag);

        }

    }

    protected virtual void DataUpdate() {
        
    }

    public static SPWindow GetWindow(string uniqueUITag) {

        if(string.IsNullOrEmpty(uniqueUITag)) {
            // LogError("Empty tag");
            return null;
        }

        if(!uniqueUI.Contains(uniqueUITag)) {
            // LogError("No UI with tag " + uniqueUITag);
            return null;
        }

        return (SPWindow)uniqueUI[uniqueUITag];
    }
    
    protected virtual void Awake() {
        if(!hasInit) {
            Init();
        }
    }
    protected virtual void OnDestroy() {
        if(hasInit) {
            Destroy();
        }
    }

    protected virtual void Start() {

    }

    protected virtual void OnEnable() {
        if(!hasInit) {
            Init();
        }
    }

    protected virtual void OnDisable() {

    }

    public virtual void UpdateInput() {
        
    }    

    public virtual void ToggleWindow(bool toggle) {
        gameObject.SetActive(toggle);
        OnToggleWindow?.Invoke(toggle);
    }

    public void ToggleWindow() { ToggleWindow(!gameObject.activeSelf);}
    public void ToggleWindowOpen() { ToggleWindow(true);}
    public void ToggleWindowClose() { ToggleWindow(false);}

}

[System.Serializable]
public class SPWindowTheme {

    public SPTheme parentTheme = new SPTheme();
    public SPTheme defaultTheme = new SPTheme();
    public SPTheme linkTheme = new SPTheme();
    public SPTheme addressTheme = new SPTheme();
    public SPTheme identityTheme = new SPTheme();
    public Color highlightColor = Color.green, readOnlyHighlight = Color.gray;
    public float borderWidth = .5f;
    public float textSize = 18f;
    public float paragraphSize = 18f;

    [System.Serializable]
    public class SPTheme {
        public Color color = Color.white;
        public Color bgColor = Color.black;
        public SPTheme(){}
        public SPTheme(Color newColor, Color newBGColor) {
            color = newColor;
            bgColor = newBGColor;
        }

    }
}