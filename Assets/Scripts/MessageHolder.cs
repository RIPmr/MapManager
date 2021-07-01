using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageHolder : MonoBehaviour {

    #region shared instance
    private static MessageHolder instance;
    public static MessageHolder SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("MeaageHolder_Static");
                if (go != null) {
                    MessageHolder comp = go.GetComponent<MessageHolder>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<MessageHolder>();
                } else {
                    go = new GameObject("MeaageHolder_Static");
                    return go.AddComponent<MessageHolder>();
                }
            }
            return instance;
        }
    }
    #endregion

    public string message;

    public void setMessage(string message) {
        this.message = message;
    }

    public string getMessage() {
        return message;
    }

    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

}
