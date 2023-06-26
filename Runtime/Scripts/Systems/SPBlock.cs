using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// using Hashtable = ExitGames.Client.Photon.Hashtable;
using SimpleJSON;
using System;
using Newtonsoft.Json;

public enum SPBlockType {Undefined = 0, Player, DAO, Collection, Item, _Count}
public enum SPChainType {Undefined = 0, Ethereum, Rinkleby, Offline, Engine, _Count}
public enum SPPlayerType {Undefined = 0, Player, Guest, NPC}



[System.Serializable]
public class SPData : ScriptableObject {

    public bool HasInit {get{return GetHasInit();}}
    public bool IsLoading {get{return LoadCoroutine != null;}}
    public Coroutine LoadCoroutine {get{return loadCoroutine;}}

    public bool GotJSON {get{return gotJSON;}}
    public string JSONString {get{return jsonString;}}
    public JSONObject JSONObject {get{return jsonObject;}}
    public JSONObject JSONAPI {get{return jsonAPI;}}

    public virtual bool GetHasInit() {return isInit;}
    public virtual void SetLoadCoroutine(Coroutine newLoadCoroutine) {if(LoadCoroutine != null) {Debug.LogError("Already loading.");} loadCoroutine = newLoadCoroutine;}
    public Action OnUpdated;
    public Action OnLoaded;
    public Action OnInited;
    public static bool VerboseLoading = false; 
    public static bool VerboseJSON = false; 
    public const string GlobalDataVersion = "0.0";
    public static string DataVersion{get{return "0.1";}}
    public virtual string GetDataName() {return "UNDEFINED";}

    [Header("Data")]
    [SerializeField] protected bool isCreated = false;
    [SerializeField] protected bool isInit = false; 

    [Header("JSON")]
    [SerializeField] protected bool gotJSON = false;
    [SerializeField] public bool apiLoaded = false;
    [SerializeField] protected string jsonString = "";
    [SerializeField] protected string jsonAPIString = "";
    [SerializeField] protected JSONObject jsonObject = null;
    [SerializeField] protected JSONObject jsonAPI = null;    
    protected Coroutine loadCoroutine = null;


    public virtual void Created() {

        isCreated = true; 

        OnUpdated?.Invoke();
        OnLoaded?.Invoke();
        
    }

    public virtual void Init() {

        loadCoroutine = null;
        isInit = true; 

        OnUpdated?.Invoke();
        OnInited?.Invoke();

    }

    public virtual object[] BlockToData() {
        if(VerboseJSON) Debug.Log("BlockToData"); 
        // return new object[1] {SPIO.FormatJSON((JSONObject)Serialize())};
        Serialize();
        SetJSON(JSONObject);
        if(VerboseJSON) Debug.Log("Returning data."); 
        return new object[1] {jsonObject.ToString()};
    }
    
    public virtual void DataToBlock(object [] data) {

        if(data == null) {
            Debug.LogError("No data");
            return;
        }

        if(VerboseJSON) Debug.Log("DataToBlock: " + (string)data[0]); 

        //JSONNode.LoadFromFile
        Deserialize(JSON.Parse((string)data[0]), GlobalDataVersion);
    }

    public virtual JSONNode Serialize() {

        if(jsonObject == null) {
            jsonObject = new JSONObject();
        }

        jsonObject["version"] = GlobalDataVersion;
        return jsonObject;
    }

    public virtual void Deserialize(JSONNode newObject, string version = GlobalDataVersion) {

        if(newObject == null) {
            Debug.LogError("Deserializing null");
        } else {
            string fileVersion = newObject.AsObject["version"];
            if(fileVersion == null || fileVersion != version) {
                Debug.LogError(fileVersion == null ? "No version" : "Wrong version");
            }
        }

        SetJSON(newObject);
    }

    public virtual void SetJSON(JSONNode newJSON) {

        if(newJSON == null) {

            if(VerboseJSON) Debug.LogError("Setting JSON to null");
            jsonObject = new JSONObject();
            jsonString = "";
            gotJSON = false;
            return;
        }


        jsonObject = newJSON.AsObject;
        jsonString = SPIO.FormatJSON((JSONObject)JSONObject);

        // Debug.Log("Setting JSON: " + json.ToString());

        gotJSON = true;
    }

}

[System.Serializable]
public class SPBlock : SPData {

    public static Hashtable Blocks = new Hashtable();
    public static Hashtable Players = new Hashtable();
    public static Hashtable Collections = new Hashtable();
    public static Hashtable Items = new Hashtable();

    public bool IsValid {get{return true;}}

    public string Address {get{return address;}}
    public string AddressTrunc {get{return addressTrunc;}}
    public string Contract {get{return contract;}}
    public string Token {get{return token;}}
    public string Name {get{return name;}}
    public string Description {get{return description;}}
    public string Link {get{return link;}}
    public string URI {get{return uri;}}
    public int BlockID {get{return blockID;}}
    public SPBlockType BlockType {get{return blockType;}}
    public SPPlayerType PlayerType {get{return playerType;}}
    public SPChainType ChainType {get{return chainType;}}
    public string Network {get{return network;}}

    public bool IsGuest {get{return playerType == SPPlayerType.Guest;}}
    public bool IsNPC {get{return playerType == SPPlayerType.NPC;}}

    public SPImage Image {get{return image;}}
    public Texture2D ImageTexture {get{return image?.Texture;}}
   

    [Header("Info")]
    [NonSerialized] protected string address = null;
    [SerializeField] protected int blockID = -1;
    [SerializeField] protected SPBlockType blockType = SPBlockType.Undefined;
    [SerializeField] protected SPChainType chainType = SPChainType.Undefined;
    [SerializeField] protected SPPlayerType playerType = SPPlayerType.Player;
    [SerializeField] protected SPBlock owner;
    [SerializeField] protected bool isLocalBlock = false;

    [Header("Data")]
    [SerializeField] protected string contract = "";
    [SerializeField] protected string token = "";
    [SerializeField] protected string name = "";
    [SerializeField] protected string link = "";

    [TextArea(0,30)]
    [SerializeField] protected string description = "";
    [SerializeField] protected string uri = "";
    [SerializeField] protected string network = "";
    [SerializeField] protected SPImage image = new SPImage();

    //set to null by default so we dont have infinte recursive pockets 

    [Header("Fields")]
    [SerializeField] protected Hashtable hashtable = new Hashtable();
    [SerializeField] protected string addressTrunc = null;

    public const char TokenChar = '#';

    public static string CombineAddress(string newContract, string newToken) {return newContract + (string.IsNullOrEmpty(newToken) ? "" : TokenChar + newToken);}
    public static string[] SplitAddress(string newAddress) {
        int tokenIndex = newAddress.IndexOf(TokenChar);
        bool hasToken = tokenIndex > -1;

        string newContract = hasToken ? newAddress.Substring(0,newAddress.IndexOf(TokenChar)) : newAddress;
        string newToken = hasToken ? newAddress.Substring(newAddress.IndexOf(TokenChar) + 1) : "";
        return new string[2] {newContract,newToken};
        
    }

    
}


[System.Serializable]
public class SPImage : SPData {
    
    public Texture2D Texture {get{return texture == null ? backupTexture : texture;}}
    public Texture2D TextureRaw {get{return texture;}}

    public string URI {get{return uri;}}
    public string Extension {get{return extension;}}
    public string ImageText {get{return imageText;}}
    public bool CanDisplay {get{return supportedFormat; }} // && (extension == ".jpg" || extension == ".jpeg" || extension == ".png");
    public override string GetDataName() {return "Image";}

    [Header("Image")]
    [SerializeField] protected Texture2D texture;
    [SerializeField] protected string imageText;
    [SerializeField] protected string extension = "";
    [SerializeField] protected string uri;
    [SerializeField] bool supportedFormat = false; 
    protected Texture2D backupTexture = null;

    public SPImage() {}

    public void SetImage(Texture2D newTexture) {

        if(backupTexture == null) {
            backupTexture = null; //SPHelper.GiveBackupTexture(SPBlockType.Collection);
        }
        
        if(newTexture == texture) {
            return;
        }

        texture = newTexture;
        supportedFormat = newTexture != null;

    }

    public void SetURI(string newURI) {
        
        uri = newURI;
        extension = string.IsNullOrEmpty(newURI) ? "" : Path.GetExtension(newURI).ToLower();

    }

    public void SetImageText(string newSVGText) {
        imageText = newSVGText;
    }

    public override JSONNode Serialize() {
        JSONObject jsonObject = base.Serialize().AsObject;
        jsonObject[SPDataField.URI.ToString()] = URI;
        return jsonObject;
    }
    
    public override void Deserialize(JSONNode newJSON, string version) {
        base.Deserialize(newJSON, version);

        if(newJSON == null) {
            return;
        }

        SetURI(newJSON[SPDataField.URI.ToString()]);
    }


}

