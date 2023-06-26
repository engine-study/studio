using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SPDataType {Address, Name, String, Float, Int, MultiString, PageURL, ImageURL, InputString}
public enum SPDataField {address, name, link, description, URI, blockType, playerType, token, chain, _Count}

public class SPHelper : MonoBehaviour {
    
    public static SPHelper Instance;

    [SerializeField] protected Transform cartesian, isometric;
    public Texture2D [] users;
    public Texture2D [] DAOs;
    public Texture2D [] institutions;
    public Texture2D [] NPCs;
    public Texture2D [] items;
    public Texture2D [] unknowns;

    public const string PLAYER_STRING = "PLAYER";
    public const string GUEST_STRING = "GUEST";
    public const string NPC_STRING = "NPC";    
    public const string TOKEN_STRING = "TOKEN";
    public const string CONTRACT_STRING = "CONTRACT";

    public static string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";

    void Awake() {
        Instance = this; 
    }

    void OnDestroy() {
        Instance = null;
    }

    public static Vector3 LocalToCamera(Vector3 local) {local.y = 0f; return SPInput.Camera.transform.TransformVector(local); }
    public static Vector3 CameraToLocal(Vector3 camera) {camera.y = 0f; return SPInput.Camera.transform.InverseTransformVector(camera).normalized; }
    public static Vector3 IsometricToCartesian(Vector3 isoVector) {return Instance.cartesian.InverseTransformVector(isoVector);}
    public static Vector3 CartesianToWorld(Vector3 localCartesian) {return Instance.cartesian.TransformVector(localCartesian);}
    public static Vector3 CartesianToIsometric(Vector3 cartVector) {return Instance.isometric.InverseTransformVector(cartVector);}
    public static Vector3 IsometricToWorld(Vector3 localVector) {return Instance.isometric.TransformVector(localVector);}


    public static string GiveTruncatedHash(string hash) {
        if(string.IsNullOrEmpty(hash)) {
            //Debug.Log("Empty hash");
            return null;
        }

        if(hash.Length > 9) {
            if(hash[hash.Length -1] == ']') {
                return hash.Substring(0,5) + "..." + hash.Substring(hash.IndexOf('[') - 4) ;
            } else {
                return hash.Substring(0,5) + "..." + hash.Substring(hash.Length - 4);
            }
        } else {
            return hash;
        }
        
    }


    public static bool IsValidAddress(string newAddress) {
        return !string.IsNullOrEmpty(newAddress) && newAddress.Length > 1 && newAddress.Substring(0,2) == "0x";
    }


    public static bool IsValidLink(string newLink) {
        Uri uriResult;
        return Uri.TryCreate(newLink, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public static string GiveAddress(SPPlayerType type) {

        string random = "";
        string example = "0xc6b0562605D35eE710138402B878ffe6F2E23807";
        string fakeString = "";

        if(type == SPPlayerType.Guest) {
            fakeString = GUEST_STRING;
        } else if(type == SPPlayerType.NPC) {
            fakeString = NPC_STRING;
        } else {
            fakeString = "";
        }

        for(int i = 0; i < example.Length - fakeString.Length; i++) {

            if(UnityEngine.Random.Range(0f,1f) < .5f) {
                random += UnityEngine.Random.Range(0,10).ToString();
            } else {
                random += (char)('a' + UnityEngine.Random.Range (0,26));
            }

        }

        return fakeString + random;

    }


    public static string GiveString() {
        string newString = "";
        int randomLength = UnityEngine.Random.Range(2,10);

        for(int i = 0; i < randomLength; i++) {
             newString += chars[UnityEngine.Random.Range(0,chars.Length)];
        }

        return newString;
    }
    
}
