using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SPActionProvider : MonoBehaviour
{
    public List<SPInteract> Interacts; 

    void Awake() {
        Interacts = GetComponentsInChildren<SPInteract>(true).ToList();
    }


}
