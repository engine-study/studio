using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public enum ImageStatus{Null, Loading, Success, CantDisplay}
public class SPImageFrame : SPWindow, IPointerUpHandler
{


    public Texture2D ImageTexture {get{return imageTexture;}}

    [Header("Fields")]
    [SerializeField] Texture2D imageTexture;
    [SerializeField] protected ImageStatus status;

    [Header("Image")]
    [SerializeField] protected SPImage imageData;
    [SerializeField] protected bool hideIfNoImage = false;
    [SerializeField] protected bool maskImage = true;
    [SerializeField] protected bool canChangeParentSize = false;
    [SerializeField] protected Mask imageMask;
    [SerializeField] protected Image imageMaskImage;
    [SerializeField] protected RawImage image;
    [SerializeField] protected RectTransform imageParentRect;
    [SerializeField] protected RectTransform imageRect;
    [SerializeField] protected GameObject warningParent;
    [SerializeField] protected GameObject warningMissingBlock, warningCannotDisplay, warningLoading;
    [SerializeField] protected GameObject maskBorder,stretchBorder;
    [SerializeField] protected TextMeshProUGUI extensionText;
    [SerializeField] protected SPRawText svgText;
    protected bool isBackupTexture = false; 

    protected Vector2 initialSize;

    protected override void OnEnable() {
        base.OnEnable();        

        if(hasInit) {
            StartCoroutine(FixCoroutine());
        }
   
    }

    protected override void OnDisable() {
        base.OnDisable();

    }

    // public override void DataUpdate() {

    //     base.DataUpdate(); 

    //     // image.texture = null;
    //     // image.color = Color.black - Color.black;
        
    //     if(imageData == null) {
            
    //         imageTexture = null;
    //         svgText.gameObject.SetActive(false);

    //         SetStatus(ImageStatus.Null);
    //         SetTexture(null);
    //         return;

    //     } else {

    //         SetupImage(imageData);

    //     }
    // }

    public void SetImageData(SPImage newImage) {

        if(imageData != null) {
            imageData.OnUpdated -= DataUpdate;
        }

        imageData = newImage;

        if(newImage != null) {
            newImage.OnUpdated += DataUpdate;
        }

    }

    public void SetupImage(SPImage newImage) {

        // Debug.Log("Setup Image:", gameObject);

        SetImageData(newImage);

        if(newImage == null) {
           SetStatus(ImageStatus.Null);
           SetTexture(null);
           return;
        } else if(!newImage.HasInit) {
           SetStatus(ImageStatus.Loading);
        } else if(newImage.HasInit && !newImage.CanDisplay ) {
           SetStatus(ImageStatus.CantDisplay);
        } else {
           SetStatus(ImageStatus.Success);
        }

        svgText.gameObject.SetActive(!string.IsNullOrEmpty(newImage.ImageText));
        if(svgText.gameObject.activeSelf) {
            svgText.UpdateField(newImage.ImageText);
        }
            
        if(newImage.CanDisplay) {

        } else {
            extensionText.text = newImage.Extension;

        }
    
        SetTexture(newImage.Texture);
    }
    
    public void SetupImage(Texture2D newImage) {
        
        imageTexture = newImage;
        svgText.gameObject.SetActive(false);

        SetStatus(ImageTexture == null ? ImageStatus.Null : ImageStatus.Success);
        SetTexture(newImage);

    }
    

    public void SetTexture(Texture2D newImage) {

        // Debug.Log("Setting image texture to " + Block?.ImageTexture?.name, gameObject);

        imageTexture = newImage;
        image.texture = ImageTexture;
        image.color = ImageTexture ? Color.white : Color.black - Color.black;

        if(hideIfNoImage) {
            ToggleWindow(status == ImageStatus.Success);
        }

        if(image.texture != null && Active) {
            StartCoroutine(FixCoroutine());
        }

    }

    IEnumerator FixCoroutine() {
        yield return null;
        InitImageRatio();
    }
    // async void Resize() {
    //     await ResizeTask();
    // }

    // async public Task ResizeTask() {
    //     await Task.Yield();
    //     InitImageRatio();
    // }

    public void SetStatus(ImageStatus newStatus) {
        
        status = newStatus;

        warningParent.SetActive(newStatus != ImageStatus.Success && newStatus != ImageStatus.Null);

        warningMissingBlock.SetActive(newStatus == ImageStatus.Null);
        warningCannotDisplay.SetActive(newStatus == ImageStatus.CantDisplay);
        warningLoading.SetActive(newStatus == ImageStatus.Loading);
    }


    public void InitImageRatio() {

        //Debug.Log(imageTexture.width.ToString() + imageTexture.height.ToString());
        //Debug.Log("aspect: " + ((float)imageTexture.width/(float)imageTexture.height));
        //scale the image to the correct aspect ratio

        //Canvas.ForceUpdateCanvases();
        //imageParentRect.ForceUpdateRectTransforms();

        //Debug.Log(gameObject.name + " texture wxh: " + imageTexture.width + " " + imageTexture.height);
        //Debug.Log(gameObject.name + " image wxh: " + imageRect.rect.width + " " + imageRect.rect.height);
        //Debug.Log(gameObject.name + " parent wxh: " + imageParentRect.rect.width + " " + imageParentRect.rect.height);

        if(initialSize == Vector2.zero) {
            float largerSize = Mathf.Max(imageParentRect.rect.width, imageParentRect.rect.height);
            initialSize = new Vector2(largerSize,largerSize);
        }

        maskBorder.SetActive(maskImage);
        stretchBorder.SetActive(!maskImage);
        
        imageMask.enabled = maskImage;
        imageMaskImage.enabled = maskImage;

        if(image.texture == null || ImageTexture.width == ImageTexture.height) {
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialSize.x); // .width = initialSize.x * (imageTexture.width/imageTexture.height);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialSize.y);
        } else if(ImageTexture.width > ImageTexture.height) {
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialSize.x * ((float)ImageTexture.width/(float)ImageTexture.height)); // .width = initialSize.x * (imageTexture.width/imageTexture.height);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialSize.y);
        } else {
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialSize.x); // .width = initialSize.x * (imageTexture.width/imageTexture.height);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialSize.y * ((float)ImageTexture.height/(float)ImageTexture.width));
        }

        if(canChangeParentSize) {
            imageParentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageRect.rect.width);
            imageParentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageRect.rect.height);
        }

    }

    public virtual void OnPointerUp(PointerEventData eventData) {
        
        if(eventData.dragging) {
            return;
        }

        Debug.Log("Image click viewer");

    }
}
