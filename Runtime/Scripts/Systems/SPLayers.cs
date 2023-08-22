using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLayers {

    public const int PlayerLayer = 6;
    public const int LocalPlayerLayer = 9;
    public const int GroundLayer = 10;
    public const int CharacterLayer = 7;

    public static LayerMask MaskPlayers = (1 << PlayerLayer); 
    public static LayerMask InvertMaskPlayers = ~(MaskPlayers);

}

/// <summary>
/// A list of tags.
/// </summary>
[System.Serializable]
public class TagMask
{

    [SerializeField]
    public string[] m_tags = new string[] { "Player" };

    public string[] tags
    {
        get { return m_tags; }
        set { m_tags = value; }
    }

    public bool IsInTagMask(string tag)
    {
        if (tags == null || tags.Length == 0) return true;
        for (int i = 0; i < tags.Length; i++)
        {
            if (string.Equals(tag, tags[i])) return true;
        }
        return false;
    }

}