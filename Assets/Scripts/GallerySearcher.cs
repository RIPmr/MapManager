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

public class GallerySearcher : MonoBehaviour {

    #region shared instance
    private static GallerySearcher instance;
    public static GallerySearcher SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("Canvas");
                if (go != null) {
                    GallerySearcher comp = go.GetComponent<GallerySearcher>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<GallerySearcher>();
                } else {
                    go = new GameObject("Canvas");
                    return go.AddComponent<GallerySearcher>();
                }
            }
            return instance;
        }
    }
    #endregion

    #region public parameters
    int SEARCHRESNUM = 0;
    #endregion

    #region private parameters
    string CompentPath;         //接受选择文件的路径
    string UnityPath;           //接受转成功后的路径 也就是Unity所需要的路径

    bool fileExist = false;
    XmlDocument xmlDoc;

    string filepath_search;
    #endregion

    public void openXml() {
        if (File.Exists(filepath_search)) {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath_search);
            fileExist = true;
        }
    }

    public void saveSearchRes() {
        xmlDoc.Save(filepath_search);
    }

    public void clearXml() {
        if (fileExist) {
            SEARCHRESNUM = 0;
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("results").ChildNodes;
            int resNum = nodeList.Count;
            for (int i = nodeList.Count - 1; i >= 0; i--) {
                XmlNode xmln = nodeList[i];
                if (xmln.Name == "resGal") xmln.ParentNode.RemoveChild(xmln);
            }
            //xmlDoc.Save(filepath_search);
            //Debug.Log("clearRes OK!");
        }
    }

    public void createXml() {
        if (!File.Exists(filepath_search)) {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("results");
            
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath_search);
        }
    }

    public bool checkElem(string name, string keyWords) {
        if (fileExist) {
            int Distance;
            double Similarity;

            Distance = MapSearcher.LevenshteinDistance(name, keyWords, out Similarity);

            if (Similarity > 0.10) {
                insertXmlNode(name, Similarity, SEARCHRESNUM);
                SEARCHRESNUM++;
                return true;
            }
        }
        return false;
    }

    public void insertXmlNode(string resName, double similarity, int id) {
        if (fileExist) {
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("results").ChildNodes;
            //int resNum = nodeList.Count;
            XmlNode root = xmlDoc.SelectSingleNode("results");

            XmlElement elmNew = xmlDoc.CreateElement("resGal");
            elmNew.SetAttribute("id", id.ToString());
            elmNew.SetAttribute("name", resName);
            elmNew.SetAttribute("similarity", similarity.ToString("0.####"));

            bool flag = false;

            foreach (XmlElement xe in nodeList) {
                if (similarity >= double.Parse(xe.GetAttribute("similarity"))) {
                    root.InsertBefore(elmNew, xe);
                    flag = true;
                    break;
                }
            }

            if (!flag) root.AppendChild(elmNew);

            //xmlDoc.Save(filepath_search);
            //Debug.Log("AddRes OK!");
        }
    }

    public int getResGals(ref List<string> sGalList) {
        sGalList.Clear();
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("results").ChildNodes;
        foreach (XmlElement res in nodeList) {
            string galName = res.GetAttribute("name");
            sGalList.Add(galName);
        }
        return nodeList.Count;
    }

    private void Start() {
        filepath_search = UnityEngine.Application.dataPath + @"/Xmls/galSearchRes.xml";
        createXml();
        openXml();
    }

}
