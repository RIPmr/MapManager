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

public class MapSearcher : MonoBehaviour {

    #region shared instance
    private static MapSearcher instance;
    public static MapSearcher SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("Canvas");
                if (go != null) {
                    MapSearcher comp = go.GetComponent<MapSearcher>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<MapSearcher>();
                } else {
                    go = new GameObject("Canvas");
                    return go.AddComponent<MapSearcher>();
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

    /// <summary>
    /// 编辑距离（Levenshtein Distance）
    /// </summary>
    /// <param name="source">源串</param>
    /// <param name="target">目标串</param>
    /// <param name="similarity">输出：相似度，值在0～１</param>
    /// <param name="isCaseSensitive">是否大小写敏感</param>
    /// <returns>源串和目标串之间的编辑距离</returns>
    public static int LevenshteinDistance(string source, string target, out double similarity, bool isCaseSensitive = false) {
        if (string.IsNullOrEmpty(source)) {
            if (string.IsNullOrEmpty(target)) {
                similarity = 1;
                return 0;
            } else {
                similarity = 0;
                return target.Length;
            }
        } else if (string.IsNullOrEmpty(target)) {
            similarity = 0;
            return source.Length;
        }

        string From, To;
        if (isCaseSensitive) {   // 大小写敏感
            From = source;
            To = target;
        } else {   // 大小写无关
            From = source.ToLower();
            To = target.ToLower();
        }

        // 初始化
        int m = From.Length;
        int n = To.Length;
        int[,] H = new int[m + 1, n + 1];
        for (int i = 0; i <= m; i++) H[i, 0] = i;  // 注意：初始化[0,0]
        for (int j = 1; j <= n; j++) H[0, j] = j;

        // 迭代
        for (int i = 1; i <= m; i++) {
            char SI = From[i - 1];
            for (int j = 1; j <= n; j++) {   // 删除（deletion） 插入（insertion） 替换（substitution）
                if (SI == To[j - 1])
                    H[i, j] = H[i - 1, j - 1];
                else
                    H[i, j] = Mathf.Min(H[i - 1, j - 1], Mathf.Min(H[i - 1, j], H[i, j - 1])) + 1;
            }
        }

        // 计算相似度
        int MaxLength = Mathf.Max(m, n);   // 两字符串的最大长度
        similarity = ((double)(MaxLength - H[m, n])) / MaxLength;

        return H[m, n];    // 编辑距离
    }

    //private void Start() {
    //    int Distance;
    //    double Similarity;
    //    print("------------------------------------");
    //    print("源串 = ");
    //    string Source = "abcdefghijk";

    //    print("目标串 = ");
    //    string Target = "k";

    //    Distance = LevenshteinDistance(Source, Target, out Similarity);
    //    print("编辑距离 = " + Distance.ToString());
    //    print("相似度 = " + Similarity.ToString("0.####"));
    //}

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
                if (xmln.Name == "resPic") xmln.ParentNode.RemoveChild(xmln);
            }
            //xmlDoc.Save(filepath_search);
            //Debug.Log("clearRes OK!");
        }
    }

    public void createXml() {
        if (!File.Exists(filepath_search)) {
            //创建XML文档实例
            XmlDocument xmlDoc = new XmlDocument();
            //创建root节点
            XmlElement root = xmlDoc.CreateElement("results");

            //节点添加至XMLDoc中
            xmlDoc.AppendChild(root);
            //把XML文件保存至本地
            xmlDoc.Save(filepath_search);
            //Debug.Log("createXml OK!");
        }
    }

    public bool checkElem(string name, string path, string keyWords) {
        if (fileExist) {
            int Distance;
            double Similarity;

            Distance = LevenshteinDistance(name, keyWords, out Similarity);

            if (Similarity > 0.18) {
                insertXmlNode(name, path, Similarity, SEARCHRESNUM);
                SEARCHRESNUM++;
                return true;
            }
        }
        return false;
    }

    public void insertXmlNode(string resName, string foldPath, double similarity, int id) {
        if (fileExist) {
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("results").ChildNodes;
            //int resNum = nodeList.Count;
            XmlNode root = xmlDoc.SelectSingleNode("results");

            XmlElement elmNew = xmlDoc.CreateElement("resPic");
            elmNew.SetAttribute("id", id.ToString());
            elmNew.SetAttribute("name", resName);
            elmNew.SetAttribute("path", foldPath);
            elmNew.SetAttribute("similarity", similarity.ToString("0.####"));

            bool flag = false;

            foreach (XmlElement xe in nodeList) {
                if (similarity >= double.Parse(xe.GetAttribute("similarity"))) {
                    root.InsertBefore(elmNew, xe);
                    flag = true;
                    break;
                }
            }

            if (!flag) {
                root.AppendChild(elmNew);
            }

            //xmlDoc.Save(filepath_search);
            //Debug.Log("AddRes OK!");
        }
    }

    public void getPicsPathByID(int startID, int endID, ref List<string> sPathList, ref List<string> PicNameList) {
        sPathList.Clear();
        PicNameList.Clear();
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("results").ChildNodes;
        int nowCheckIdD = 0;
        foreach (XmlElement pic in nodeList) {
            int id = nowCheckIdD++;
            if (id >= startID && id <= endID) {
                string picName = pic.GetAttribute("name");
                string foldPath = pic.GetAttribute("path");
                string picPath = foldPath + "/" + picName;
                PicNameList.Add(picName);
                sPathList.Add(picPath);
            } else if (id > endID) break;
        }
    }

    public void changeMapPanelMode(string mode) {

        string keyWords = WindowManeger.SharedInstance.getWindow("searchInput").GetComponent<Text>().text;
        if (keyWords.Equals("")) return;

        if (mode.Equals("search")) {
            MapPanelManager.SharedInstance.showSearchRes = true;
            MapPanelManager.SharedInstance.updateMapPanel(1);
        } else {
            MapPanelManager.SharedInstance.showSearchRes = false;
            // MapPanelManager.SharedInstance.updateMapPanel(1);
            MainController.SharedInstance.setPageWithoutUpdate();
        }
    }

    private void Start() {
        filepath_search = UnityEngine.Application.dataPath + @"/Xmls/searchRes.xml";
        createXml();
        openXml();
    }

}
