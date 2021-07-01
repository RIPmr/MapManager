using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPanelManager : MonoBehaviour {

    #region shared instance
    private static MapPanelManager instance;
    public static MapPanelManager SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("MapPanel");
                if (go != null) {
                    MapPanelManager comp = go.GetComponent<MapPanelManager>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<MapPanelManager>();
                } else {
                    go = new GameObject("MapPanel");
                    return go.AddComponent<MapPanelManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    public int mapNum = 60;
    public RectTransform containerPanel;
    public Scrollbar slider;
    public MapManager[] MMers;
    public bool showSearchRes = false;

    //public void updateMapPanel(int page) {

    //    int startID = (page - 1) * 60;
    //    int endID = startID + 59;

    //    FileReader.SharedInstance.loadMaps(startID, endID, showSearchRes);

    //    for (int i = 0; i < mapNum; i++) {
    //        if (i < FileReader.SharedInstance.allTex2d.Count) {
    //            MMers[i].gameObject.SetActive(true);
    //            MMers[i].UpdateShowTex(FileReader.SharedInstance.allTex2d[i], FileReader.SharedInstance.fileNames[i], FileReader.SharedInstance.filePaths[i]);
    //        } else {
    //            MMers[i].gameObject.SetActive(false);
    //        }
    //    }
    //}

    public void updateMapPanel(int page) {
        int startID = (page - 1) * 60;
        int endID = startID + 59;

        //FileReader.SharedInstance.loadMaps(startID, endID, showSearchRes);
        StartCoroutine(FileReader.SharedInstance.loadMaps(startID, endID, showSearchRes));
    }

    public void setPanelLength(int picNum) {
        if (containerPanel == null) {
            print("container is null!");
            return;
        }
        int rowNum = picNum / 5;
        rowNum += ((picNum % 5 > 0) ? 1 : 0);
        Vector2 newSize = containerPanel.sizeDelta;
        newSize.y = 302.0f * (rowNum - 2);
        containerPanel.sizeDelta = newSize;
        slider.value = 1.0f;
    }

    public void linkContainers(int id) {
        MMers[id].linkMap(FileReader.SharedInstance.allTex2d[id]);
    }

    public void updateContainer(int id, bool isOutOfBound) {
        if (isOutOfBound) {
            MMers[id].gameObject.SetActive(false);
        } else {
            MMers[id].gameObject.SetActive(true);
            MMers[id].UpdateFrame(FileReader.SharedInstance.fileNames[id], FileReader.SharedInstance.filePaths[id]);
        }
    }

    //public IEnumerator updateView() {
    //    for (int i = 0; i < mapNum; i++) {
    //        if (i < FileReader.SharedInstance.allTex2d.Count) {
    //            MMers[i].gameObject.SetActive(true);
    //            MMers[i].UpdateShowTex(FileReader.SharedInstance.allTex2d[i], FileReader.SharedInstance.fileNames[i], FileReader.SharedInstance.filePaths[i]);
    //        } else {
    //            MMers[i].gameObject.SetActive(false);
    //            continue;
    //        }
    //        // print(i);
    //        yield return 0;
    //    }
    //}

    private void Awake() {
        MMers = new MapManager[mapNum];
        Transform[] grandFa;
        grandFa = GetComponentsInChildren<Transform>();

        for (int i = 0; i < mapNum; i++) {
            foreach (Transform tr in grandFa) {
                if (tr.name.Equals("MapContainer (" + i + ")")) {
                    MMers[i] = tr.GetComponent<MapManager>();
                }
                continue;
            }
        }
    }

}
