using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SPWindowParent : SPWindow
{

    [Header("Parent")]
    [SerializeField] protected GameObject content;
    [SerializeField] protected GameObject loading;

    [SerializeField] protected bool addSubcanvas = true;
    [SerializeField] protected bool addRaycaster = true;
    bool animateWindow = false;
    [SerializeField] protected SPThemeScriptable themeOverride;

    protected List<SPWindow> windows;
    float animateLerp = 0f; 
    Vector2 startPos;

    public override void Init() {

        if(hasInit) {
            // Debug.LogError("Double init");
            return;
        }

        base.Init();

        if(addSubcanvas) gameObject.AddComponent<Canvas>();
        if(addRaycaster) gameObject.AddComponent<GraphicRaycaster>();

        startPos = rect.anchoredPosition;

        windows = GetComponentsInChildren<SPWindow>(true).ToList();

        if(themeOverride) {

            SetTheme(themeOverride.Theme);

            for(int i = 0; i < windows.Count; i++) {
                windows[i].SetTheme(themeOverride.Theme);
            }
        }
            
        // for(int i = 0; i < windows.Count; i++) {
        //     windows[i].Init();
        // }

        this.gameObject.BroadcastMessage("Init",SendMessageOptions.DontRequireReceiver);

    }

    protected override void Destroy()
    {
        base.Destroy();

    }

    // public virtual void SetBlock(SPBlock newBlock) {

    //     Log("Set Block: " + (newBlock == null ? "null" : newBlock.AddressTrunc));

    //     if(window) {
    //         window.SetBlock(newBlock);
    //         SPCoreLoader.LoadCore(window.Core);
    //     }
    // }

    public virtual void ToggleLoadingComplete(bool toggle) {

        // Debug.Log("Toggle Loading " + toggle);

        if(content)
            content.SetActive(toggle);

        if(loading)
            loading.SetActive(!toggle);
    }

    // public override void DataUpdate() {

    //     ToggleLoadingComplete(Core.HasInit);

    //     base.DataUpdate();
        
    // }
    
    protected override void OnEnable() {
        base.OnEnable();

        if(animateWindow) {
            StartCoroutine(AnimateEnableCoroutine());
        }

    }

    protected override void OnDisable() {
        base.OnDisable();

    }

    
    public override void UpdateColor() {

        bgColor = Theme.parentTheme.bgColor;
        borderColor = Theme.parentTheme.color;
        graphicsColor = Theme.parentTheme.color;

        ApplyGraphics();

    }

    IEnumerator AnimateEnableCoroutine() {

        rect.anchoredPosition = startPos - Vector2.up * 50f; // rect.sizeDelta.y;
        animateLerp = 0f; 

        while(animateLerp < 1f) {
            animateLerp = Mathf.Clamp01(animateLerp + Time.deltaTime * 5f);
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, startPos, animateLerp * animateLerp);
            yield return null;
        }
    }

}
