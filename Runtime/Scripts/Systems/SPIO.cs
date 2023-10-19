using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using Newtonsoft.Json.Linq;
using System.IO;

public class SPIO : MonoBehaviour {

    public static SPIO Instance;
    public static string SaveDirectory {get{return Application.persistentDataPath + "/";}}

    public static string URL_BASE {get{return "https://engine.study/" + Application.productName + "/";}}
    public const string FILE_UPLOADER = "uploader.php";
    public const string FILE_EXTENSION = ".txt";

    public const string DIR_BLOCK = "Blocks/";
    public const string DIR_BACKEND = "Files/";
    public const string FILE_CHAT = "chat.txt";

    
    void Awake() {
        Instance = this;
    }

    void OnDestroy() {
        Instance = null;
    }


    public static Coroutine DownloadJSON(LoadReference reference, string data ="", string method = "GET") {
        return Instance.StartCoroutine(Instance.DownloadJSONCoroutine(reference, data, method));
    }

    protected IEnumerator DownloadJSONCoroutine(LoadReference reference, string data ="", string method = "GET") {

        int attempts = 3;
        bool success = false; 

        reference.wasSuccessful = false; 

        while(attempts > 0 && !success) {

            attempts--;

            DownloadHandler dh = new DownloadHandlerBuffer();
            UploadHandler uh = null;

            if(!string.IsNullOrEmpty(data)) {
                uh = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(data));
            }

            using(UnityWebRequest www = new UnityWebRequest(reference.URI, method, dh, uh)) {
                                    
                if(reference.headers != null && reference.headers.Length > 0) {
                    for(int i = 0; i < reference.headers.Length; i++) {
                        //www.SetRequestHeader("X-API-KEY", apiKey);        
                        www.SetRequestHeader(reference.headers[i].name, reference.headers[i].value);   
                    }
                }

                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success) {

                    success = true; 

                    // Debug.Log("Download: " + reference.URI);

                    //Debug.Log(www.downloadedBytes);
                    reference.wasSuccessful = true; 
                    reference.loadedObject = www.downloadHandler.text;

                } else {

                    reference.wasSuccessful = false; 
                    reference.loadedObject = null;

                    Debug.Log("Download: " + reference.URI);
                    Debug.Log("ERROR: " + www.error);

                    if(www.responseCode == 404) {
                        yield break;
                    }

                    if(www.responseCode == 429) {
                        yield return new WaitForSeconds(Random.Range(10f,15f));
                    }

                    //Debug.Log(www.downloadedBytes);
                    yield return new WaitForSeconds(1f);
                }     
            }

        
        }

        //Debug.Log("Loading done.");

    }

    public static Coroutine UploadJSON(LoadReference reference, string fileName, string data) { // string name,

        LoadReferenceField filename = new LoadReferenceField("fileName", fileName);
        //LoadReferenceField fileID = new LoadReferenceField("fileID", name);
        LoadReferenceField file = new LoadReferenceField("data", data);
        LoadReferenceField security = new LoadReferenceField("pleasedonthurtus", SPEncryption.EncryptAddress(fileName));

        reference.uploadData = new LoadReferenceField[3]{filename,file,security};
        
        return Instance.StartCoroutine(Instance.UploadJSONCoroutine(reference));

    }

    
    //automatically appends uploader.php
    protected IEnumerator UploadJSONCoroutine(LoadReference reference) {
    
        WWWForm form = new WWWForm();

        for(int i = 0; i < reference.uploadData.Length; i++) {
            //Debug.Log(reference.uploadData[i].name + " " + reference.uploadData[i].value);
            form.AddField(reference.uploadData[i].name, reference.uploadData[i].value);
        }
        
        //DownloadHandler handler = new DownloadHandlerBuffer();
        using( UnityWebRequest www = UnityWebRequest.Post(reference.URI + FILE_UPLOADER, form)) {
            
            www.downloadHandler = new DownloadHandlerBuffer();

            reference.webRequest = www;

            Debug.Log("Upload " + reference.uploadData[0].value + " to " + www.uri);

            yield return www.SendWebRequest();

            if( www.result == UnityWebRequest.Result.Success) {
                //Debug.Log("Online save complete!");
                // Debug.Log("Saved to " + www.uri);
                // Debug.Log(reference.uploadData[1].value.ToString());
                reference.wasSuccessful = true; 

            } else {
                Debug.Log("Failed save to " + www.uri);
                Debug.Log(www.error);
                reference.wasSuccessful = false; 
            }

            Debug.Log("Echo " + www.downloadHandler.text);

        }
      

    }

    public static string FormatJSON(JSONNode jsonNode) {

        // if(SPBlock.VerboseJSON) Debug.Log("Formatting JSON");

        if(jsonNode == null) {
            return "";
        }

        // if(SPBlock.VerboseJSON) Debug.Log(jsonNode.ToString());
        // return jsonNode.ToString();

        return JToken.Parse(jsonNode.ToString()).ToString(Newtonsoft.Json.Formatting.Indented);
    }

    
    public static void WriteJSONToDisk(JSONNode json, string path) {

        WriteStringToDisk(FormatJSON((JSONNode)json), path);
  
    }

    public static void WriteStringToDisk(string newString, string path) {
        string savePath = path;
         
        StreamWriter sw = new StreamWriter(savePath);

        sw.Write(newString);
        sw.Close();
    }

    public static JSONNode ReadJSONFromDisk(string path) {


        string readPath = path;

        if(!File.Exists(readPath)) {
            Debug.Log("Could not find " + readPath);
            return null;
            //Debug.Log("Creating file.");
            //WriteJSONToDisk(new JSONObject(), readPath);
        }

        Debug.Log("Opening " + readPath);

        StreamReader sr = new StreamReader(readPath);
        string resultstring = sr.ReadToEnd();
        sr.Close();

        JSONNode result = JSON.Parse(resultstring);

        return result;
    }
    
}


public struct LoadReferenceField {
    public string name, value; 
    public LoadReferenceField(string newName, string newValue) {name = newName; value = newValue;}
}


public class LoadReference {
    public string URI = "";
    public LoadReferenceField [] headers = null;
    public LoadReferenceField [] uploadData = null;
    public object loadedObject = null;
    public bool wasSuccessful = false;
    public UnityWebRequest webRequest;
    public LoadReference(string newURI = null, LoadReferenceField [] newHeaders = null, LoadReferenceField [] newUpload = null) {
        URI = newURI;
        headers = newHeaders;
        uploadData = newUpload;
    }


}
