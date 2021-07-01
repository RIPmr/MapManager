using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteWindowManager : MonoBehaviour {

    #region shared instance
    private static DeleteWindowManager instance;
    public static DeleteWindowManager SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("DeleteWindow");
                if (go != null) {
                    DeleteWindowManager comp = go.GetComponent<DeleteWindowManager>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<DeleteWindowManager>();
                } else {
                    go = new GameObject("DeleteWindow");
                    return go.AddComponent<DeleteWindowManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    #region public parameters
    public List<string> folderPaths = new List<string>();
    public GameObject pathPanel;
    public GameObject pathButtonPrefab;
    #endregion

    #region private parameters
    private List<GameObject> pathButtons = new List<GameObject>();
    private int deltaNum = 0;
    #endregion

    public void getFolderList() {
        MainController.SharedInstance.getFolderPath(ref folderPaths);
    }

    public void openDeleteWindow() {
        getFolderList();
        int len = folderPaths.Count;
        if (len > 10) {
            deltaNum = len - 10;
            Vector2 newSize = pathPanel.GetComponent<RectTransform>().sizeDelta;
            newSize.y += 60.0f * (deltaNum + 3);
            pathPanel.GetComponent<RectTransform>().sizeDelta = newSize;
        }
        for(int i = 0; i < len; i++) {
            GameObject newButton = GameObject.Instantiate(pathButtonPrefab, pathButtonPrefab.transform.position, pathButtonPrefab.transform.rotation);
            newButton.transform.parent = pathPanel.transform;

            int pathLen = folderPaths[i].Length;
            string cutedPath;
            if (pathLen > 68) {
                cutedPath = folderPaths[i].Substring(0, 68);
                newButton.GetComponent<MessageManager>().newMessage(cutedPath + "... ...");
            } else newButton.GetComponent<MessageManager>().newMessage(folderPaths[i]);

            newButton.GetComponent<EventRegistTest>().path = folderPaths[i];
            newButton.GetComponent<EventRegistTest>().id = i;
            pathButtons.Add(newButton);
        }
        pathButtonPrefab.SetActive(false);
    }

    public void deleteButton(int id) {
        // 隐藏已删除xml信息的按钮
        //print(id + " " + pathButtons.Count);
        pathButtons[id].SetActive(false);
    }

    public void closeDeleteWindow() {
        Vector2 newSize = pathPanel.GetComponent<RectTransform>().sizeDelta;
        if(deltaNum != 0) newSize.y -= 60.0f * (deltaNum + 3);
        deltaNum = 0;
        pathPanel.GetComponent<RectTransform>().sizeDelta = newSize;

        int len = folderPaths.Count;
        for (int i = 0; i < len; i++) {
            // 删除按钮
            Destroy(pathButtons[i]);
        }
        pathButtons.Clear();
        pathButtonPrefab.SetActive(true);
    }

}
