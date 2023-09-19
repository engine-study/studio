
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public enum MessageType {Debug, Chat, System}

public class SPUIBase : MonoBehaviour
{
    public static SPUIBase I;
    public static bool CanInput {get{return canInput;}}
    public static bool CanMouseInput {get{return false;}}
    public static bool IsPointerOverUIElement {get{return isPointerOverUI;}}
    public static bool Submit {get{return submit;}}
    public static bool IsDragging {get{return dragging;}}
    public static bool IsMouseOnScreen {get{return !mouseOffscreen;}}
    public static bool IsSelectedTextField {get{return I && isSelectedTextField;}}
    public static bool IsInputtingTextField {get{return I && isInputtingTextField;}}
    public static bool FullscreenUI {get{return fullscreenUI;}}
    public static Camera Camera {get{return SPUIInstance.Camera;}}
    public static RectTransform DraggableParent {get{return SPUIInstance.DraggableParent;}}
    public static Canvas Canvas {get{return SPUIInstance.Canvas;}}
    public static RectTransform CanvasRect {get{return SPUIInstance.CanvasRect;}}
    public static AudioSource AudioSource {get{return SPUIInstance.AudioSource;}}
    public static SPWindowTheme GlobalTheme;
    protected static bool canInput = false, fullscreenUI; 

    bool isPregame = true; 
    static bool submit, fakeSubmit = false; 
    [SerializeField] protected static bool isPointerOverUI = false, isUISelected = false, isSelectedTextField = false, isInputtingTextField = false, mouseOffscreen = false, dragging = false;
    [SerializeField] protected SPThemeScriptable defaultGlobalTheme;
    [SerializeField] protected Selectable activeUI;


    [Header("Audio")]
    [SerializeField] protected AudioClip s_typing;
    [SerializeField] protected AudioClip s_login, s_message, s_hover, s_accept, s_reject, s_confirm, s_error, s_close, s_link, s_drag, s_drop;
    [SerializeField] protected AudioClip [] s_pops;

    [Header("Debug")]
    [SerializeField] protected SPWindow exclusiveUI;

    public static Action OnDragStart;
    public static Action OnDragEnd;
    
    int UILayer;

    public void Awake() {

        Init();

    }

    public void Start() {

    }

    public void Init() {
        
        I = this;

        if(defaultGlobalTheme) {GlobalTheme = defaultGlobalTheme.Theme;}
        else {GlobalTheme = new SPWindowTheme();}

        UILayer = LayerMask.NameToLayer("UI");
        
        Reset();

        isPregame = false; 

    }

    public void Reset() {
        
    }


    void Update() {

        UpdateDebug();

        if(!SPGlobal.Updating) {
            return;
        }

        UpdateInput();

        UpdateGameInput();

    }


    void UpdateDebug() {

        if(!SPGlobal.IsDebug && !Application.isEditor) {
            return;
        }
        
        if( (Input.GetKeyDown(KeyCode.Tilde) || Input.GetKeyDown(KeyCode.BackQuote)) && !Input.GetKey(KeyCode.LeftShift)) {
            ToggleMotherUI();
        }


    }

    void UpdateInput() {


        submit = SPUIBase.CanInput && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.E) || fakeSubmit); //Input.GetButtonDown("Submit")
        
        //Returns 'true' if we touched or hovering on Unity UI element.
        isPointerOverUI = CheckIfPointerIsOnUI();
        isUISelected = EventSystem.current && EventSystem.current.currentSelectedGameObject;

        activeUI = isUISelected ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;
        isSelectedTextField = activeUI ? activeUI.GetType() == typeof(TMP_InputField) : false;
        isInputtingTextField = isSelectedTextField ? ((TMP_InputField)activeUI).isFocused && !((TMP_InputField)activeUI).readOnly : false;

        mouseOffscreen = Input.mousePosition.x < 0f || Input.mousePosition.y < 0f || Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height;

        fullscreenUI = I.exclusiveUI != null;
        canInput = SPGlobal.Updating && !FullscreenUI && !I.isPregame && !IsInputtingTextField;

    }


    void UpdateGameInput() {

        if(fullscreenUI) {
            exclusiveUI.UpdateInput();
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {

            //first behaviour is to leave and clear and inputfield that we might have captured
            if(IsSelectedTextField) {
                EventSystem.current.SetSelectedGameObject(null);
            }
            //third behaviour is to exit any fullscreen UI we might have open
            else if(fullscreenUI) {
                ToggleTakeoverUI(exclusiveUI, false);
            } 
            //last behaviour is to open the game pause screen
            else {
                // OpenMenu();
            }
        }

        //CAN INPUT CHECK, RETURN OTHERWISE
        if(!CanInput)
            return;

        //show store
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {


        } else {
            
            if(Input.GetKeyDown(KeyCode.F)) {
                ToggleFullscreen();
            }

        }

        /*
        if(!playerList.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Tab)) {
            OpenPlayerList(true);
        } else if(playerList.gameObject.activeSelf && Input.GetKeyUp(KeyCode.Tab)){
            OpenPlayerList(false);
        }
        */

        /*
        if(!chatParent.activeSelf && Input.GetKeyDown(KeyCode.Z)) {
            chatRoom.ToggleChatRoom(true);
        }
        */

        //If we arent typing in chat, hide it after a few seconds
        /*
        if (!inputField.gameObject.activeInHierarchy) {
            hideChatCount -= Time.deltaTime;
            if (hideChatCount < 0f) 
                chatParent.SetActive(false);
        }*/

    }

    // public static void WorldToCanvas(Vector3 newPosition, RectTransform rect) {

    // }
    
    public static void WorldToCanvas(Vector3 newPosition, RectTransform target, Vector3 offset =  default(Vector3)) {
        Vector2 screen = Camera.WorldToScreenPoint(newPosition + offset);
        target.anchoredPosition = ScreenToCanvas(screen);
    }

    public static void TransformToCanvas(Transform newTransform, RectTransform target, Vector3 offset =  default(Vector3)) { 
        WorldToCanvas(newTransform.position, target, offset);
    }

    public static Vector2 ScreenToRay() {
        Vector2 hitPos = Vector2.zero;
        return hitPos;
    }

    public static Vector2 ScreenToWorld(Vector2 screenPos, RectTransform rectParent = null) {
        return SPUIBase.Camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y,Camera.transform.position.z + 1f));
    }

    public static Vector2 ScreenToCanvas(Vector2 screenPos, RectTransform rectParent = null) {
        Vector2 anchorPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent == null ? CanvasRect : rectParent, screenPos, Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera, out anchorPosition);
        return anchorPosition;
    }



    public static void SendSubmit() {
        fakeSubmit = true; 
        //SPPlayer.LocalPlayer.UseInteractable();
    }


    public static bool uiVisible = false; 
    public static void ToggleMotherUI() {
        ToggleMotherUI(!uiVisible);
    }
    public static void ToggleMotherUI(bool toggle) {
        uiVisible = toggle;
        for(int i = 0; i < SPCanvas.Canvases.Count; i++) {
            SPCanvas.Canvases[i].ToggleVisible(toggle);
        }
    }


    public void ToggleTakeoverUI(SPWindow window, bool toggle) {
        window.ToggleWindow(toggle);

        if(exclusiveUI != null && toggle) {
            exclusiveUI.ToggleWindow(false);
        }

        fullscreenUI = toggle ? window : null;

    }

   
    public static void SetDragging(bool toggle) {
        dragging = toggle;
                
        if(dragging) {
            SPUIBase.PlayDrag();
            SPCursorTexture.UpdateCursor(SPCursorState.Drag);

            OnDragStart?.Invoke();
        } else {
            SPCursorTexture.UpdateCursor(SPCursorState.Default);

            OnDragEnd?.Invoke();
        }
    }

    public void ToggleFullscreen() {
       ToggleFullscreen(!Screen.fullScreen);
    }

    public void ToggleFullscreen(bool toggle) {
        
        if(Application.platform == RuntimePlatform.WebGLPlayer) {
            if(toggle) {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,FullScreenMode.MaximizedWindow);
            } else {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,FullScreenMode.Windowed);
            }
        } else {
            if(toggle) {
                Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length -1].width, Screen.resolutions[Screen.resolutions.Length -1].height, FullScreenMode.MaximizedWindow);
            } else {
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            }
        }

    }

    private void WorldToCanvasPosition(RectTransform uiRect, Vector3 position) {

        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 temp = Camera.WorldToViewportPoint(position);

        //Calculate position considering our percentage, using our canvas size
        //So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
        temp.x *= CanvasRect.sizeDelta.x;
        temp.y *= CanvasRect.sizeDelta.y;

        //The result is ready, but, this result is correct if CanvasRect recttransform pivot is 0,0 - left lower corner.
        //But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
        //We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
        //returned value will still be correct.

        temp.x -= CanvasRect.sizeDelta.x * CanvasRect.pivot.x;
        temp.y -= CanvasRect.sizeDelta.y * CanvasRect.pivot.y;

        uiRect.anchoredPosition = temp;

    }

 
 
    private bool CheckIfPointerIsOnUI() {
        return CheckIfPointerOnUI(GetEventSystemRaycastResults());
    }
    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool CheckIfPointerOnUI(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }
 
 
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    

    public static void PlayUISound(SPActionType windowType) {
        if(windowType == SPActionType.Forward) { PlayAccept();}
        else if(windowType == SPActionType.Backward) { PlayReject(); }
        else if(windowType == SPActionType.Confirm) { PlayConfirm(); }
        else if(windowType == SPActionType.Link) { PlayLink(); }
        else if(windowType == SPActionType.Close) { PlayClose(); }
        else if(windowType == SPActionType.Text) { PlayTyping(); }
        
    }

    public static void PlayLogin() {  if(!I) return; PlaySound(I.s_login, .1f, false);}
    public static void PlayHover() {  if(!I) return; PlaySound(I.s_hover);}
    public static void PlayAccept() {  if(!I) return; PlaySound(I.s_accept);} 
    public static void PlayReject() {  if(!I) return; PlaySound(I.s_reject);}
    public static void PlayConfirm() {  if(!I) return; PlaySound(I.s_confirm);}
    public static void PlayClose() {  if(!I) return; PlaySound(I.s_close);}
    public static void PlayError() {  if(!I) return; PlaySound(I.s_error);}
    public static void PlayLink() {  if(!I) return; PlaySound(I.s_link);}
    public static void PlayDrag() {  if(!I) return; PlaySound(I.s_drag);}
    public static void PlayDrop() {  if(!I) return; PlaySound(I.s_drop);}
    public static void PlayTyping() {  if(!I) return; PlaySound(I.s_typing);}
    public static void PlayPop() {  if(!I) return; PlaySound(I.s_pops[UnityEngine.Random.Range(0,I.s_pops.Length)] );}
    public static void PlayMessage() {  if(!I) return; PlaySound(I.s_message);}


    public static void PlaySound(AudioClip [] clip, float volume, float pitch) {
        PlaySound(clip[UnityEngine.Random.Range(0,clip.Length)], volume, pitch);
    }   

    public static void PlaySound(AudioClip [] clip, float volume = 1f, bool pitchShift = true) {
        PlaySound(clip[UnityEngine.Random.Range(0,clip.Length)], volume, UnityEngine.Random.Range(.95f,1.05f));
    }   

    public static void PlaySound(AudioClip clip, float volume = 1f, bool pitchShift = true) {
        PlaySound(clip, volume, UnityEngine.Random.Range(.95f,1.05f));
    }   

    public static void PlaySound(AudioClip clip, float volume, float pitch) {
        if(!I)
            return;

        AudioSource.pitch = pitch;
        AudioSource.PlayOneShot(clip, volume);
    }


}
