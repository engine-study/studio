using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class SPOpenLink : MonoBehaviour
{

    public string link; 
    public string prefix;
    public SPWindowSelectable selectable;

    public void OpenLink(string newLink) {
        link = newLink;
        OpenLink();
    }


    public void OpenSelectableText() {
        link = prefix + selectable.Field;
        OpenLink();
    }

    public void OpenLink() {

        Debug.Log("Opening " + link);
        //Application.OpenURL(link);
    	//Application.ExternalEval("window.open('"+link+"');");

        OpenLinkSameWindow(link);
    }

    void OpenLinkSameWindow(string newLink) {
        Application.OpenURL(newLink);
    }


}