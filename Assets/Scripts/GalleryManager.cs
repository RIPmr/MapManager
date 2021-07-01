using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using UnityEngine;
using System.Text;
using System.Xml;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour {

    #region shared instance
    private static GalleryManager instance;
    public static GalleryManager SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("Main Camera");
                if (go != null) {
                    GalleryManager comp = go.GetComponent<GalleryManager>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<GalleryManager>();
                } else {
                    go = new GameObject("Main Camera");
                    return go.AddComponent<GalleryManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    #region public parameters
    public int GALNUMBER;
    public GameObject title;
    public RectTransform mainPanel;
    public Scrollbar slider;
    public GameObject galleryButton;
    public GameObject resGalButton;
    public GameObject newButton;
    public Text inputer;
    public Text searchInputer;
    public bool showSearchRes = false;
    #endregion

    #region private parameters
    bool fileExist = false;
    XmlDocument xmlDoc;
    string filepath;
    List<string> resGalNames;
    List<GameObject> allGalleryContainers;
    List<GameObject> resGalleryContainers;
    #endregion

    public void startSearching() {
        string input = searchInputer.text;
        if (input.Equals("")) return;

        if (resGalleryContainers == null) {
            resGalleryContainers = new List<GameObject>();
        } else {
            foreach (GameObject obj in resGalleryContainers) {
                Destroy(obj);
            }
            resGalleryContainers.Clear();
        }

        resGalNames = new List<string>();
        if (!showSearchRes) allGalleryContainers = new List<GameObject>();
        // 隐藏所有containers
        getAllContainers();
        foreach (GameObject obj in allGalleryContainers) {
            obj.SetActive(false);
        }
        newButton.SetActive(false);
        // 置flag true
        showSearchRes = true;
        // 开始搜索
        StartCoroutine(searchGallery(input));
        // 修改loader从galSearchRes.xml读取galleries
        // 重新创建containers
        // 提供All方法
    }

    public void showAllGalleries() {
        foreach (GameObject obj in resGalleryContainers) {
            Destroy(obj);
        }
        foreach (GameObject obj in allGalleryContainers) {
            obj.SetActive(true);
        }
        newButton.SetActive(true);
        setPanelLength();
        showSearchRes = false;
    }

    private void getAllContainers() {
        Transform[] grandFa;
        grandFa = mainPanel.GetComponentsInChildren<Transform>();

        foreach (Transform tr in grandFa) {
            if (tr.name.Equals("MapContainer (0)(Clone)")) {
                allGalleryContainers.Add(tr.gameObject);
            }
        }
    }

    IEnumerator searchGallery(string keywords) {
        int nowLoaded = 0;
        float percentage = 0.0f;
        WindowManeger.SharedInstance.show("search");
        // 遍历XML文档
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("galleries").ChildNodes;
        GallerySearcher.SharedInstance.clearXml();
        foreach (XmlElement galElem in nodeList) {
            // 加载预览
            //Texture2D thumbnail = loadThumbnail(galElem.GetAttribute("name"));
            //addGallary(thumbnail, galElem.GetAttribute("name"), int.Parse(galElem.GetAttribute("id")));

            GallerySearcher.SharedInstance.checkElem(galElem.GetAttribute("name"), keywords);

            nowLoaded++;
            if (GALNUMBER > 0) percentage = (float)nowLoaded / GALNUMBER;
            WindowManeger.SharedInstance.getWindow("search").GetComponent<MessageManager>().newMessage((percentage * 100.0).ToString("0.#"));
            WindowManeger.SharedInstance.getWindow("search").GetComponent<MessageManager>().newProgressive(percentage);
            yield return 0;
        }
        GallerySearcher.SharedInstance.saveSearchRes();
        int num = GallerySearcher.SharedInstance.getResGals(ref resGalNames);
        showSearchResults(num);
        WindowManeger.SharedInstance.hide("search");
    }

    private void showSearchResults(int totalNum) {
        setPanelLength(totalNum);
        foreach (string name in resGalNames) {
            showResGalContainer(loadThumbnail(name), name);
        }
    }

    public void showResGalContainer(Texture2D thumbnail, string name) {
        string newName = name;
        GameObject newButObj = Instantiate(resGalButton, newButton.transform.position, newButton.transform.rotation);
        newButObj.transform.parent = mainPanel.transform;
        resGalleryContainers.Add(newButObj);

        if (thumbnail != null) {
            int maxLen = Mathf.Max(thumbnail.width, thumbnail.height);

            //if (maxLen > 400) {
            //    //print("resized thumbnail");
            //    thumbnail = TexTools.scaleTextureBilinear(thumbnail, (float)400 / maxLen);
            //}

            newButObj.GetComponent<MapManager>().linkMap(thumbnail);
        }
        newButObj.GetComponent<MapManager>().UpdateFrame(newName, "");
    }

    public void addGallary() {
        string newName = inputer.text;
        if (newName == null || newName.Equals("")) return;
        GALNUMBER++;
        title.GetComponent<MessageManager>().newMessage(GALNUMBER.ToString());
        // 添加记录到xml
        int nid = insertXmlNode(newName);

        WindowManeger.SharedInstance.hide("input");
        GameObject newButObj = Instantiate(galleryButton, newButton.transform.position, newButton.transform.rotation);
        newButObj.transform.parent = mainPanel.transform;

        newButObj.GetComponent<MapManager>().UpdateFrame(newName, "", nid);

        int count = mainPanel.transform.childCount;         //参数为物体在当前所在的子物体列表中的顺序
        newButton.transform.SetSiblingIndex(count - 1);     //count-1指把child物体在当前子物体列表的顺序设置为最后一个，0为第一个

        setPanelLength();
    }

    public void addGallary(Texture2D thumbnail, string name, int id) {
        string newName = name; // xml中读取的已有Gallery
        //WindowManeger.SharedInstance.hide("input");
        GameObject newButObj = Instantiate(galleryButton, newButton.transform.position, newButton.transform.rotation);
        newButObj.transform.parent = mainPanel.transform;

        if (thumbnail != null) newButObj.GetComponent<MapManager>().linkMap(thumbnail);
        newButObj.GetComponent<MapManager>().UpdateFrame(newName, "", id);

        int count = mainPanel.transform.childCount;         //参数为物体在当前所在的子物体列表中的顺序
        newButton.transform.SetSiblingIndex(count - 1);     //count-1指把child物体在当前子物体列表的顺序设置为最后一个，0为第一个

        setPanelLength();
    }

    public bool checkNodeExist(string name, ref XmlNodeList list) {
        foreach (XmlElement xe in list) if (name.Equals(xe.GetAttribute("name"))) return false;
        return true;
    }

    public int insertXmlNode(string name) {
        if (fileExist) {
            XmlNode root = xmlDoc.SelectSingleNode("galleries");
            XmlNodeList nodeList = root.ChildNodes;

            int id_old = -1;

            foreach (XmlElement xe in nodeList) {
                int id_now = int.Parse(xe.GetAttribute("id"));
                if (id_now != id_old + 1) {
                    XmlElement elmNew = xmlDoc.CreateElement("gallery");
                    elmNew.SetAttribute("id", (id_old + 1).ToString());
                    elmNew.SetAttribute("name", name);
                    root.InsertBefore(elmNew, xe);
                    XmlElement rootElem = (XmlElement)root;
                    rootElem.SetAttribute("galNumber", GALNUMBER.ToString());
                    xmlDoc.Save(filepath);
                    return (id_old + 1);
                } else id_old++;
            }

            return addXmlNode(name);

        }
        return -1;
    }

    public int addXmlNode(string name) {
        if (fileExist) {
            XmlNode root = xmlDoc.SelectSingleNode("galleries");
            XmlNodeList nodeList = root.ChildNodes;
            int folderNum = nodeList.Count;

            //if (checkNodeExist(name, ref nodeList)) {
            XmlElement elmNew = xmlDoc.CreateElement("gallery");
            elmNew.SetAttribute("id", folderNum.ToString());
            elmNew.SetAttribute("name", name);

            root.AppendChild(elmNew);

            XmlElement rootElem = (XmlElement)root;
            rootElem.SetAttribute("galNumber", GALNUMBER.ToString());
            xmlDoc.Save(filepath);

            return folderNum;
            //}
        }
        return -1;
    }

    public void deleteGallery(string name, int id) {
        GALNUMBER--;
        title.GetComponent<MessageManager>().newMessage(GALNUMBER.ToString());
        setPanelLength();
        // 从xml中删除记录
        deleteXmlNode(name, id);
        // 删除本地xml文件
        deleteXmlDoc(name);
    }

    public void deleteXmlDoc(string name) {
        string xmlpath = UnityEngine.Application.dataPath + @"/Xmls/" + name + ".xml";
        if (File.Exists(xmlpath)) {
            File.Delete(xmlpath);
        }
    }

    public void deleteXmlNode(string name, int id) {
        if (fileExist) {
            XmlNode root = xmlDoc.SelectSingleNode("galleries");
            XmlNodeList nodeList = root.ChildNodes;
            for (int i = nodeList.Count - 1; i >= 0; i--) {
                XmlElement xmln = (XmlElement)nodeList[i];
                if (xmln.GetAttribute("name").Equals(name) && xmln.GetAttribute("id").Equals(id.ToString())) xmln.ParentNode.RemoveChild(xmln);
            }
            XmlElement rootElem = (XmlElement)root;
            rootElem.SetAttribute("galNumber", GALNUMBER.ToString());
            xmlDoc.Save(filepath);
        }
    }

    public Texture2D loadThumbnail(string galleryName) {
        string xmlpath = UnityEngine.Application.dataPath + @"/Xmls/" + galleryName + ".xml";
        if (File.Exists(xmlpath)) {
            XmlDocument xmlDoc_temp = new XmlDocument();
            xmlDoc_temp.Load(xmlpath);

            XmlNode root = xmlDoc_temp.SelectSingleNode("folders");
            XmlNodeList nodeList = root.ChildNodes;
            XmlElement rootElem = (XmlElement)root;
            int picNum = int.Parse(rootElem.GetAttribute("picNumber"));
            if (picNum < 1) return null;

            foreach (XmlElement foldElem in nodeList) {
                XmlNodeList picList = foldElem.ChildNodes;
                string foldPath = foldElem.GetAttribute("path");
                foreach (XmlElement pic in picList) {
                    string picName = pic.GetAttribute("name");
                    string picPath = foldPath + "/" + picName;
                    Texture2D newTex = FileReader.SharedInstance.getMapByPath(picPath);
                    if (newTex != null) return newTex;
                }
            }

        }
        return null;
    }

    IEnumerator readGalleries() {
        int nowLoaded = 0;
        float percentage = 0.0f;
        WindowManeger.SharedInstance.show("update_s");
        // 遍历XML文档
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("galleries").ChildNodes;
        foreach (XmlElement galElem in nodeList) {
            // 加载预览
            Texture2D thumbnail = loadThumbnail(galElem.GetAttribute("name"));

            //if (thumbnail != null) {
            //    int maxLen = Mathf.Max(thumbnail.width, thumbnail.height);
            //    if (maxLen > 400) {
            //        //print("resized thumbnail");
            //        thumbnail = TexTools.scaleTextureBilinear(thumbnail, (float)400 / maxLen);
            //    }
            //}

            addGallary(thumbnail, galElem.GetAttribute("name"), int.Parse(galElem.GetAttribute("id")));

            nowLoaded++;
            if (GALNUMBER > 0) percentage = (float)nowLoaded / GALNUMBER;
            WindowManeger.SharedInstance.getWindow("update_s").GetComponent<MessageManager>().newMessage((percentage * 100.0).ToString("0.#"));
            WindowManeger.SharedInstance.getWindow("update_s").GetComponent<MessageManager>().newProgressive(percentage);
            yield return 0;
        }
        WindowManeger.SharedInstance.hide("update_s");
    }

    public void openXml() {
        if (File.Exists(filepath)) {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            fileExist = true;
        }
    }

    public void createXml() {
        if (!File.Exists(filepath)) {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("galleries");
            root.SetAttribute("galNumber", "0");

            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
        }
    }

    public void setPanelLength() {
        if (mainPanel == null) {
            print("container is null!");
            return;
        }
        int rowNum = GALNUMBER / 6;
        rowNum += ((GALNUMBER % 6 > 0) ? 1 : 0);
        Vector2 newSize = mainPanel.sizeDelta;
        if (rowNum > 0) rowNum--;
        newSize.y = 360.0f * rowNum;
        mainPanel.sizeDelta = newSize;
    }

    public void setPanelLength(int totalNum) {
        if (mainPanel == null) {
            print("container is null!");
            return;
        }
        int rowNum = totalNum / 6;
        rowNum += ((totalNum % 6 > 0) ? 1 : 0);
        Vector2 newSize = mainPanel.sizeDelta;
        if (rowNum > 0) rowNum--;
        newSize.y = 360.0f * rowNum;
        mainPanel.sizeDelta = newSize;
    }

    public void setPage() {
        getPicNumber();
        //int pageNum = GALNUMBER / 60;
        //if ((GALNUMBER % 60) != 0) pageNum++;
        //PageManager.SharedInstance.resetPage(pageNum);

        title.GetComponent<MessageManager>().newMessage(GALNUMBER.ToString());
        StartCoroutine(readGalleries());
    }

    public void getPicNumber() {
        if (fileExist) {
            XmlNode root = xmlDoc.SelectSingleNode("galleries");
            XmlElement rc = (XmlElement)root;
            GALNUMBER = int.Parse(rc.GetAttribute("galNumber"));
        }
    }

    private void Awake() {
        GALNUMBER = 1;
        filepath = UnityEngine.Application.dataPath + @"/Xmls/xmlDocHolder.xml";
        createXml();
        openXml();
        setPage();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && WindowManeger.SharedInstance.getWindow("input").active == true) {
            addGallary();
        }
    }

}
