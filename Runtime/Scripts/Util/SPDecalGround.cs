using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPDecalGround : MonoBehaviour
{
    void LateUpdate() {
        Vector3 pos = transform.parent.position;
        pos.y = .025f;
        transform.position = pos;
    }
}
