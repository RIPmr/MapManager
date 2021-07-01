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

public class MainController : MonoBehaviour {

    #region shared instance
    private static MainController instance;
    public static MainController SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("Main Camera");
                if (go != null) {
                    MainController comp = go.GetComponent<MainController>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<MainController>();
                } else {
                    go = new GameObject("Main Camera");
                    return go.AddComponent<MainController>();
                }
            }
            return instance;
        }
    }
    #endregion

    #region public parameters
    public static int PICNUMBER;
    #endregion

    #region private parameters
    string CompentPath;         //接受选择文件的路径
    string UnityPath;           //接受转成功后的路径 也就是Unity所需要的路径

    bool fileExist = false;
    XmlDocument xmlDoc;

    string filepath;            //xml保存的路径，这里放在Assets路径 注意路径。

    bool isAbort = false;
    #endregion

    public void ChoosePath() {
        FolderBrowserDialog fb = new FolderBrowserDialog();                     //创建控件并实例化
        fb.Description = "选择文件夹";
        fb.RootFolder = Environment.SpecialFolder.MyComputer;                   //设置默认路径
        fb.ShowNewFolderButton = false;                                         //创建文件夹按钮关闭

        //如果按下弹窗的OK按钮
        if (fb.ShowDialog() == DialogResult.OK) CompentPath = fb.SelectedPath;  //接受路径

        //将路径中的 \ 替换成 / 由于unity路径的规范必须转
        if (CompentPath == null || CompentPath == "") return;

        UnityPath = CompentPath.Replace(@"\", "/");
        ListFiles(new DirectoryInfo(UnityPath));
    }

    public void ListFiles(FileSystemInfo info) {
        if (!info.Exists) return;
        DirectoryInfo dir = info as DirectoryInfo;

        if (dir == null) return;                                                //不是目录
        XmlElement foldNode = addXmlNode(dir.ToString().Replace(@"\", "/"));
        if (foldNode == null) return;
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++) {
            FileInfo file = files[i] as FileInfo;
            if (file != null) ;//addXmlNode(ref foldNode, file.Name);              //是文件
            //print(file.FullName + "\t " + file.Length);
            else ListFiles(files[i]);                                           //对于子目录，进行递归调用
        }
    }

    public bool checkNodeExist(string name, ref XmlNodeList list) {
        foreach (XmlElement xe in list) if (name.Equals(xe.GetAttribute("path"))) return false;
        return true;
    }

    public XmlElement addXmlNode(string foldPath) {
        if (fileExist) {
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("folders").ChildNodes;
            int folderNum = nodeList.Count;
            XmlNode root = xmlDoc.SelectSingleNode("folders");

            if (checkNodeExist(foldPath, ref nodeList)) {
                XmlElement elmNew = xmlDoc.CreateElement("fold");
                elmNew.SetAttribute("id", folderNum.ToString());
                elmNew.SetAttribute("path", foldPath);

                root.AppendChild(elmNew);
                //xmlDoc.Save(filepath);
                //Debug.Log("AddFold OK!");
                return elmNew;
            }
        }
        return null;
    }

    public void addXmlNode(ref XmlElement fold, string picName, int id) {
        if (fileExist) {
            XmlNodeList nodeList = fold.ChildNodes;
            //int folderNum = nodeList.Count;

            if (checkNodeExist(picName, ref nodeList)) {
                XmlElement elmNew = xmlDoc.CreateElement("pic");
                elmNew.SetAttribute("id", id.ToString());
                elmNew.SetAttribute("name", picName);

                fold.AppendChild(elmNew);
                //xmlDoc.Save(filepath);
                //Debug.Log("AddPic OK!");
            }
        }
    }

    public void updateXmlNode() {
        if (fileExist) {
            //得到transforms下的所有子节点
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("transforms").ChildNodes;
            //遍历所有子节点
            foreach (XmlElement xe in nodeList) {
                //拿到节点中属性ID =0的节点
                if (xe.GetAttribute("id") == "0") {
                    //更新节点属性
                    xe.SetAttribute("id", "1000");
                    //继续遍历
                    foreach (XmlElement x1 in xe.ChildNodes) {
                        if (x1.Name == "z") {
                            //这里是修改节点名称对应的数值，而上面的拿到节点连带的属性。。。
                            x1.InnerText = "update00000";
                        }
                    }
                    break;
                }
            }
            xmlDoc.Save(filepath);
            //Debug.Log("UpdateXml OK!");
        }
    }

    public void startMapUpdate() {
        StartCoroutine(updateMapList());
    }

    public void startSearching() {
        string keyWords = WindowManeger.SharedInstance.getWindow("searchInput").GetComponent<Text>().text;
        if (keyWords.Equals("")) return;
        MapSearcher.SharedInstance.clearXml();
        StartCoroutine(searchMap(keyWords));
    }

    public void getPicsPathByID(int startID, int endID, ref List<string> sPathList, ref List<string> PicNameList) {
        sPathList.Clear();
        PicNameList.Clear();
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("folders").ChildNodes;
        foreach (XmlElement foldElem in nodeList) {
            XmlNodeList picList = foldElem.ChildNodes;
            string foldPath = foldElem.GetAttribute("path");
            foreach (XmlElement pic in picList) {
                int id = int.Parse(pic.GetAttribute("id"));
                if (id >= startID && id <= endID) {
                    string picName = pic.GetAttribute("name");
                    string picPath = foldPath + "/" + picName;
                    PicNameList.Add(picName);
                    sPathList.Add(picPath);
                } else if (id > endID) break;
            }
        }
    }

    public void getFolderPath(ref List<string> pathList) {
        pathList.Clear();
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("folders").ChildNodes;
        foreach (XmlElement foldElem in nodeList) {
            pathList.Add(foldElem.GetAttribute("path"));
        }
    }

    IEnumerator searchMap(string keyWords) {
        int nowSearchNum = 0;
        int resNum = 0;
        float percentage = 0.0f;
        WindowManeger.SharedInstance.show("update_s");
        // 遍历XML文档
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("folders").ChildNodes;
        foreach (XmlElement foldElem in nodeList) {
            XmlNodeList picList = foldElem.ChildNodes;
            string foldPath = foldElem.GetAttribute("path");
            foreach (XmlElement pic in picList) {
                string picName = pic.GetAttribute("name");
                string picPath = foldPath;
                bool isRes = MapSearcher.SharedInstance.checkElem(picName, picPath, keyWords);
                if (isRes) resNum++;
                nowSearchNum++;
                if (PICNUMBER > 0) percentage = (float)nowSearchNum / PICNUMBER;
                WindowManeger.SharedInstance.getWindow("update_s").GetComponent<MessageManager>().newMessage((percentage * 100.0).ToString("0.#"));
                WindowManeger.SharedInstance.getWindow("update_s").GetComponent<MessageManager>().newProgressive(percentage);
                yield return 0;
            }
        }
        MapSearcher.SharedInstance.saveSearchRes();
        int pageNum = resNum / 60;
        if ((resNum % 60) != 0) pageNum++;
        PageManager.SharedInstance.resetPage(pageNum);
        WindowManeger.SharedInstance.hide("update_s");
    }

    public void saveXmlFile() {
        xmlDoc.Save(filepath);
    }

    public void ABORTRIGHTNOW() {
        isAbort = true;
    }

    IEnumerator updateMapList() {
        PICNUMBER = 0;
        WindowManeger.SharedInstance.show("update");
        // 遍历XML文档
        XmlNode root = xmlDoc.SelectSingleNode("folders");
        XmlNodeList nodeList = root.ChildNodes;
        int length = nodeList.Count;
        GameObject updateWindow = WindowManeger.SharedInstance.getWindow("update");
        bool abortFlag = true;
        for (int i = 0; (i < length) && abortFlag; i++) {
            XmlElement xe = (XmlElement)nodeList[i];
            deleteXmlNode(ref xe);
            FileSystemInfo info = new DirectoryInfo(xe.GetAttribute("path"));
            if (!info.Exists) continue;
            DirectoryInfo dir = info as DirectoryInfo;
            //不是目录
            if (dir == null) continue;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int j = 0; j < files.Length; j++) {
                FileInfo file = files[j] as FileInfo;
                //是文件 
                if (file != null) {
                    bool exFlag = false;
                    string[] imageName = { ".jpg", ".png", ".bmp", ".psd", ".tga", ".psd", ".exr", ".tif", ".JPG", ".PNG", ".BMP", ".PSD", ".TGA", ".PSD", ".EXR", ".TIF" };
                    foreach (string exs in imageName) {
                        if (file.Extension.Equals(exs)) {
                            exFlag = true;
                            break;
                        }
                    }
                    if (exFlag) {
                        addXmlNode(ref xe, file.Name, PICNUMBER);
                        PICNUMBER++;
                        updateWindow.GetComponent<MessageManager>().newMessage(PICNUMBER.ToString());
                    }

                    if (isAbort) {
                        abortFlag = false;
                        break;
                    }

                    yield return 0;
                }
                //对于子目录，跳过
            }
        }
        XmlElement rc = (XmlElement)root;
        rc.SetAttribute("picNumber", PICNUMBER.ToString());
        saveXmlFile();
        WindowManeger.SharedInstance.hide("update");

        if (isAbort) {
            isAbort = false;
            WindowManeger.SharedInstance.showWithoutBack("single");
        }

        int pageNum = PICNUMBER / 60;
        if ((PICNUMBER % 60) != 0) pageNum++;
        PageManager.SharedInstance.resetPage(pageNum);

        //while (true) {
        // yield return new WaitForSeconds(0.1f);
        //}
    }

    public void clearXml() {
        if (fileExist) {
            PICNUMBER = 0;
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("folders").ChildNodes;
            int resNum = nodeList.Count;
            for (int i = nodeList.Count - 1; i >= 0; i--) {
                XmlNode xmln = nodeList[i];
                if (xmln.Name == "fold") xmln.ParentNode.RemoveChild(xmln);
            }
            //xmlDoc.Save(filepath_search);
            //Debug.Log("clearRes OK!");
        }
    }

    public void deleteXmlNode(ref XmlElement fold) {
        if (fileExist) {
            XmlNodeList nodeList = fold.ChildNodes;
            for (int i = nodeList.Count - 1; i >= 0; i--) {
                XmlNode xmln = nodeList[i];
                if (xmln.Name == "pic") xmln.ParentNode.RemoveChild(xmln);
            }
            //xmlDoc.Save(filepath);
            //Debug.Log("deletePics OK!");
        }
    }

    public void deleteXmlNode(string path) {
        if (fileExist) {
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("folders").ChildNodes;
            for (int i = nodeList.Count - 1; i >= 0; i--) {
                XmlElement xmln = (XmlElement)nodeList[i];
                if (xmln.GetAttribute("path").Equals(path)) xmln.ParentNode.RemoveChild(xmln);
            }
            //xmlDoc.Save(filepath);
            //Debug.Log("deleteFold OK!");
        }
    }

    public void showXmlNodes() {
        if (fileExist) {
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("floders").ChildNodes;
            //遍历每一个节点，拿节点的属性以及节点的内容
            foreach (XmlElement xe in nodeList) {
                //Debug.Log("Attribute :" + xe.GetAttribute("name"));
                //Debug.Log("NAME :" + xe.Name);
                foreach (XmlElement x1 in xe.ChildNodes) {
                    if (x1.Name == "y") {
                        //Debug.Log("VALUE :" + x1.InnerText);
                    }
                }
            }
            //Debug.Log("all = " + xmlDoc.OuterXml);
        }
    }

    public void getPicNumber() {
        if (fileExist) {
            XmlNode root = xmlDoc.SelectSingleNode("folders");
            XmlElement rc = (XmlElement)root;
            PICNUMBER = int.Parse(rc.GetAttribute("picNumber"));
        }
    }

    public void setPageWithoutUpdate() {
        getPicNumber();
        int pageNum = PICNUMBER / 60;
        if ((PICNUMBER % 60) != 0) pageNum++;
        PageManager.SharedInstance.resetPage(pageNum);
    }

    public void openXml() {
        if (File.Exists(filepath)) {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            fileExist = true;
        }
    }

    public void createXml() {
        //继续判断当前路径下是否有该文件
        if (!File.Exists(filepath)) {
            //创建XML文档实例
            XmlDocument xmlDoc = new XmlDocument();
            //创建root节点
            XmlElement root = xmlDoc.CreateElement("folders");
            root.SetAttribute("picNumber", "0");

            //节点添加至XMLDoc中
            xmlDoc.AppendChild(root);
            //把XML文件保存至本地
            xmlDoc.Save(filepath);
            //Debug.Log("createXml OK!");
        }
    }

    private void Awake() {
        GameObject mesHold = GameObject.Find("MeaageHolder_Static");
        string xmlDocName;
        if (mesHold == null) {
            print("cant find message holder!");
            xmlDocName = "my";
        } else xmlDocName = mesHold.GetComponent<MessageHolder>().getMessage();
        filepath = UnityEngine.Application.dataPath + @"/Xmls/" + xmlDocName + ".xml";
        createXml();
        openXml();
        // updateMapList();
        // startMapUpdate();
        setPageWithoutUpdate();
    }

}