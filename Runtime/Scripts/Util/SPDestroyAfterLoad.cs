using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SPDestroyAfterLoad : MonoBehaviour
{

    [SerializeField] float deleteTime = 1f;
    void Awake() {
        GameObject.DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += Delete; 
    }

    void OnDestroy() {
        SceneManager.activeSceneChanged -= Delete; 
    }

    void Delete(Scene old, Scene newScene) {
        StartCoroutine(DeleteCoroutine());
    }

    IEnumerator DeleteCoroutine() {
        yield return new WaitForSeconds(deleteTime);
        GameObject.Destroy(gameObject);
    }

}
