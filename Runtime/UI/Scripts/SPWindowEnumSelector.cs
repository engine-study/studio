using System;
using System.Collections.Generic;
using UnityEngine;

public class SPWindowEnumSelector : SPWindow
{
    [Header("Window Enum")]
    [SerializeField] string EnumName;
    [SerializeField] GameObject prefab;
    [SerializeField] bool ignoreLast;

    [Header("Debug")]
    [SerializeField] List<GameObject> windows;

    protected override void Start() {
        base.Start();
        
        windows = new List<GameObject>();
        prefab.SetActive(false);
        
        Type enumType = Type.GetType(EnumName);
        if (enumType == null || !enumType.IsEnum)
        {
            Debug.LogError("Invalid enum type.");
            return;
        }

        Array enumValues = Enum.GetValues(enumType);
        int totalLength = ignoreLast ? enumValues.Length - 1 : enumValues.Length;
        for (int i = 0; i < totalLength; i++) {
            GameObject newWindow = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            newWindow.SetActive(true);
            windows.Add(newWindow);
        }
    }
}
