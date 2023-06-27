using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Networked : MonoBehaviour
{
    public bool IsLocal {get{return isLocal;}}
    [SerializeField] protected bool isLocal;

}
