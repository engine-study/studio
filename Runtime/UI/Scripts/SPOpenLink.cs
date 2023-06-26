using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class SPOpenLink : MonoBehaviour
{

    public string link; 

    public void OpenLink(string newLink) {
        link = newLink;
        OpenLink();
    }

    public void OpenLink() {

        Debug.Log("Opening " + link);
        //Application.OpenURL(link);
    	//Application.ExternalEval("window.open('"+link+"');");

        OpenLinkSameWindow(link);
    }

    void OpenLinkSameWindow(string newLink) {
        Application.OpenURL(link);
    }


}