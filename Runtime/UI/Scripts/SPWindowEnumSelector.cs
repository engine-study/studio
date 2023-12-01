using System;
using System.Collections.Generic;
using UnityEngine;

public class SPWindowEnumSelector : SPWindow
{
    [Header("Window Enum")]
    [SerializeField] string EnumName;
    [SerializeField] SPButton prefab;
    [SerializeField] bool ignoreLast;

    [Header("Debug")]
    [SerializeField] SPButton selected;
    [SerializeField] List<SPButton> windows;
    Array enumValues;
    protected override void Start() {
        base.Start();
        
        windows = new List<SPButton>();

        if(prefab == null) prefab = transform.GetChild(0)?.GetComponent<SPButton>();
        prefab.ToggleWindow(false);
        
        Type enumType = Type.GetType(EnumName);
        if (enumType == null || !enumType.IsEnum) {
            Debug.LogError("Invalid enum type " + EnumName, this);
            return;
        }

        enumValues = Enum.GetValues(enumType);
        
        int totalLength = ignoreLast ? enumValues.Length - 1 : enumValues.Length;
        for (int i = 0; i < totalLength; i++) {
            SPButton newButton = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            newButton.ToggleWindow(true);
            windows.Add(newButton);
            newButton.OnClickedDetail += UpdateSelection;
            newButton.UpdateField(enumValues.GetValue(i).ToString());
        }
    }

    protected virtual void UpdateSelection(SPButton button) {
        selected = button;
        UpdatedEnum(windows.IndexOf(button));
    }

    protected virtual void UpdatedEnum(int index) {

    }
}
