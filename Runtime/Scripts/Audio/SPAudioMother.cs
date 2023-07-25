using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAudioMother : MonoBehaviour
{
    public static SPAudioMother Instance;
    public SPAudioSource [] pool;

    void Awake() {
        Instance = this;
    }

    void OnDestroy() {
        Instance = null;
    }

    public static SPAudioSource GetSource(Vector3 position) {
        Instance.pool[0].transform.position = position;
        return Instance.pool[0];
    }
}
