using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using B83.Image.BMP;
using UnityEngine.UI;

public class FileReader : MonoBehaviour {

    #region shared instance
    private static FileReader instance;
    public static FileReader SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("Canvas");
                if (go != null) {
                    FileReader comp = go.GetComponent<FileReader>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<FileReader>();
                } else {
                    go = new GameObject("Canvas");
                    return go.AddComponent<FileReader>();
                }
            }
            return instance;
        }
    }
    #endregion

    // 储存获取到的图片
    public bool linkMapAutomatically = true;
    public List<Texture2D> allTex2d = new List<Texture2D>();
    public List<string> filePaths = new List<string>();
    public List<string> fileNames = new List<string>();
    BMPLoader loader = new BMPLoader();

    private void Awake() {
        if (linkMapAutomatically) {
            int len = MapPanelManager.SharedInstance.mapNum;
            for (int i = 0; i < len; i++) {
                Texture2D tt = new Texture2D(2, 2);
                allTex2d.Add(tt);
                MapPanelManager.SharedInstance.linkContainers(i);
            }
        }
    }

    public void loadMapsFromFolder(string foldPath) {
        List<string> filePaths = new List<string>();
        string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG|*.PSD|*.TGA|*.EXR|*.TIF";
        string[] ImageType = imgtype.Split('|');
        for (int i = 0; i < ImageType.Length; i++) {
            //获取path文件夹下所有的图片路径
            string[] dirs = Directory.GetFiles(foldPath, ImageType[i]);
            for (int j = 0; j < dirs.Length; j++) {
                filePaths.Add(dirs[j]);
            }
        }

        for (int i = 0; i < filePaths.Count; i++) {
            Texture2D tx = new Texture2D(100, 100);
            tx.LoadImage(getImageByte(filePaths[i]));
            allTex2d.Add(tx);
        }

        //for (int i = 0; i < filePaths.Count; i++) {
        //    WWW www = new WWW("file://" + filePaths[i]);
        //    allTex2d.Add(www.texture);
        //}
    }

    //public void loadMaps(int startID, int endID, bool isSearch) {

    //    if (isSearch) {
    //        MapSearcher.SharedInstance.getPicsPathByID(startID, endID, ref filePaths, ref fileNames);
    //    } else {
    //        MainController.SharedInstance.getPicsPathByID(startID, endID, ref filePaths, ref fileNames);
    //    }

    //    allTex2d.Clear();

    //    for (int i = 0; i < filePaths.Count; i++) {
    //        Texture2D tx = new Texture2D(100, 100);
    //        tx.LoadImage(getImageByte(filePaths[i]));
    //        allTex2d.Add(tx);
    //    }
    //}

    //public IEnumerator loadMaps(int startID, int endID, bool isSearch) {

    //    if (isSearch) {
    //        MapSearcher.SharedInstance.getPicsPathByID(startID, endID, ref filePaths, ref fileNames);
    //    } else {
    //        MainController.SharedInstance.getPicsPathByID(startID, endID, ref filePaths, ref fileNames);
    //    }

    //    allTex2d.Clear();

    //    WindowManeger.SharedInstance.show("load");

    //    for (int i = 0; i < filePaths.Count; i++) {
    //        Texture2D tx = new Texture2D(100, 100);
    //        tx.LoadImage(getImageByte(filePaths[i]));
    //        allTex2d.Add(tx);
    //        float loadPercentage = (float)(i + 1) / filePaths.Count;
    //        WindowManeger.SharedInstance.getWindow("load").GetComponent<MessageManager>().newMessage((loadPercentage * 100.0).ToString("0.#"));
    //        WindowManeger.SharedInstance.getWindow("load").GetComponent<MessageManager>().newProgressive(loadPercentage);
    //        yield return 0;
    //        //print(i);
    //    }

    //    WindowManeger.SharedInstance.hide("load");

    //    StartCoroutine(MapPanelManager.SharedInstance.updateView());
    //}

    IEnumerator LoadAssets(string path, int id) {
        WWW www = new WWW(path);
        yield return www;
        if (www.isDone && www.error == null) {
            allTex2d[id] = www.texture;
        }
    }

    public IEnumerator loadMaps(int startID, int endID, bool isSearch) {

        if (isSearch) {
            MapSearcher.SharedInstance.getPicsPathByID(startID, endID, ref filePaths, ref fileNames);
        } else {
            MainController.SharedInstance.getPicsPathByID(startID, endID, ref filePaths, ref fileNames);
        }

        WindowManeger.SharedInstance.showWithoutBack("load");

        MapPanelManager.SharedInstance.setPanelLength(filePaths.Count);

        for (int i = 0; i < filePaths.Count; i++) {

            ////////////////////////////////////////////////////////////////////////////////////////////
            //Debug.Log("file://" + filePaths[i]);
            //StartCoroutine(LoadAssets("file://" + filePaths[i], i));
            ////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////
            int pathLen = filePaths[i].Length;
            string extension = filePaths[i].Substring(pathLen - 4, 4);
            //print(extension);
            if(extension.Equals(".bmp") || extension.Equals(".BMP")) {
                BMPImage img = loader.LoadBMP(filePaths[i].Replace("/", @"\"));
                byte[] bmpBytes = img.ToTexture2D().EncodeToPNG();
                allTex2d[i].LoadImage(bmpBytes);
            } else allTex2d[i].LoadImage(getImageByte(filePaths[i]));
            ////////////////////////////////////////////////////////////////////////////////////////////

            MapPanelManager.SharedInstance.updateContainer(i, false);
            float loadPercentage = (float)(i + 1) / filePaths.Count;
            WindowManeger.SharedInstance.getWindow("load").GetComponent<MessageManager>().newMessage((loadPercentage * 100.0).ToString("0.#"));
            WindowManeger.SharedInstance.getWindow("load").GetComponent<MessageManager>().newProgressive(loadPercentage);
            yield return 0;
            //print(i);
        }

        int len = MapPanelManager.SharedInstance.mapNum;
        for (int i = filePaths.Count; i < len; i++) {
            MapPanelManager.SharedInstance.updateContainer(i, true);
        }

        WindowManeger.SharedInstance.hideWithoutBack("load");

        //StartCoroutine(MapPanelManager.SharedInstance.updateView());
    }

    public Texture2D getMapByPath(string path) {
        Texture2D newTex = new Texture2D(2, 2);
        if (!File.Exists(path)) return null;
        newTex.LoadImage(getImageByte(path));
        return newTex;
    }

    /// <summary>
    /// 根据图片路径返回图片的字节流byte[]
    /// </summary>
    /// <param name="imagePath">图片路径</param>
    /// <returns>返回的字节流</returns>
    private byte[] getImageByte(string imagePath) {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }

}
