using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class windowSlot {
    public GameObject window;
    public string name;
    public bool scriptControl = false;
}

public class WindowManeger : MonoBehaviour {

    #region shared instance
    private static WindowManeger instance;
    public static WindowManeger SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("Canvas");
                if (go != null) {
                    WindowManeger comp = go.GetComponent<WindowManeger>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<WindowManeger>();
                } else {
                    go = new GameObject("Canvas");
                    return go.AddComponent<WindowManeger>();
                }
            }
            return instance;
        }
    }
    #endregion

    public windowSlot[] windows = new windowSlot[0];
    public GameObject background;
    public DynamicButton dynaBut;

    public GameObject getWindow(string windowName) {
        int len = windows.Length;
        for(int i = 0; i < len; i++) {
            if( windows[i].name.Equals(windowName)) {
                return windows[i].window;
            }
        }
        return null;
    }

    public void hide(string windowName) {
        hideWithoutBack(windowName);
        background.SetActive(false);
    }

    public void hideWithoutBack(string windowName) {
        foreach (windowSlot obj in windows) {
            if (obj.name.Equals(windowName)) {
                if(!obj.scriptControl) obj.window.SetActive(false);
                // 特判
                if (windowName.Equals("update")) {
                    dynaBut.isActive = false;
                } else if (windowName.Equals("detailView")) {
                    DetailImageViewer.SharedInstance.isShowView(false);
                }
                break;
            }
        }
    }

    public void show(string windowName) {
        showWithoutBack(windowName);
        background.SetActive(true);
    }

    public void showWithoutBack(string windowName) {
        foreach (windowSlot obj in windows) {
            if (obj.name.Equals(windowName)) {
                if(!obj.scriptControl) obj.window.SetActive(true);
                // 特判
                if (windowName.Equals("update")) {
                    dynaBut.isActive = true;
                }else if (windowName.Equals("detailView")) {
                    DetailImageViewer.SharedInstance.isShowView(true);
                }
                break;
            }
        }
    }

    public void showWithMessage(string windowName, string message) {
        showWithMessageWithoutBack(windowName, message);
        background.SetActive(true);
    }

    public void showWithMessageWithoutBack(string windowName, string message) {
        foreach (windowSlot obj in windows) {
            if (obj.name.Equals(windowName)) {
                if(!obj.scriptControl) obj.window.SetActive(true);
                obj.window.GetComponent<MessageManager>().newMessage(message);
                break;
            }
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) show("setting");
    }

}
