using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour {
    public Text textObj;
    public Slider slidObj;
    public Button btnObj;

    public string prefix;
    public string suffix;

    public void newMessage(string message) {
        textObj.text = prefix + message + suffix;
    }

    public void newProgressive(float percentage) {
        if(slidObj == null) {
            print("none slider!");
            return;
        }
        slidObj.value = percentage;
    }

}
