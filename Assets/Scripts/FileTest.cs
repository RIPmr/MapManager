using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class FileTest : MonoBehaviour {

    //是否显示图片
    public static bool isShow;
    //图片文件路径集合
    List<string> fileName = new List<string>();
    //图片集合
    List<Texture2D> textureList = new List<Texture2D>();
    public Texture2D close;
    public Texture2D next;
    public Texture2D previous;
    private int mainTexNum;
    private float sW;
    private float sH;

    void Start() {
        sW = Screen.width;
        sH = Screen.height;
        GetAllFile(new DirectoryInfo(UnityEngine.Application.streamingAssetsPath + "/"));
        GetAllTexture();
    }

    void OnGUI() {
        GUI.depth = 0;
        if (!isShow) return;

        GUI.DrawTexture(new Rect(0, 0, sW, sH), textureList[mainTexNum]);

        //下一页
        ChangeTexture(next);
        if (GUI.Button(new Rect(sW / 2 + 20, sH - next.height - 20, next.width, next.height), "")) {
            if (mainTexNum == textureList.Count - 1)
                mainTexNum = 0;
            else
                mainTexNum += 1;
        }
        //上一页
        ChangeTexture(previous);
        if (GUI.Button(new Rect(sW / 2 - previous.width - 20, sH - previous.height - 20, previous.width, previous.height), "")) {
            if (mainTexNum == 0)
                mainTexNum = textureList.Count - 1;
            else
                mainTexNum -= 1;
        }
        //关闭
        ChangeTexture(close);
        if (GUI.Button(new Rect(sW - close.width - 10, 10, close.width, close.height), ""))
            isShow = false;
    }

    /// <summary>
    /// 依据文件路径载入图片
    /// </summary>
    private void GetAllTexture() {
        foreach (string s in fileName) {
            WWW www = new WWW("file://" + s);
            textureList.Add(www.texture);
        }
    }
    /// <summary>
    /// 获得所有图片文件路径
    /// </summary>
    /// <param name="info">目录</param>
    private void GetAllFile(FileSystemInfo info) {
        if (!info.Exists) return;
        DirectoryInfo dir = info as DirectoryInfo;
        if (dir == null) return;
        FileSystemInfo[] si = dir.GetFileSystemInfos();
        for (int i = 0; i < si.Length; i++) {
            FileInfo fi = si[i] as FileInfo;
            if (fi != null) {
                if (IsImage(fi.Extension))
                    fileName.Add(fi.FullName);
            } else
                GetAllFile(si[i]);
        }

    }
    /// <summary>
    /// 推断文件是不是图片
    /// </summary>
    /// <param name="name">文件名称</param>
    private bool IsImage(string name) {
        string[] imageName = { ".jpg", ".png", ".gif", ".bmp", ".psd", ".tga", ".psd", ".JPG", ".PNG", ".GIF", ".BMP", ".PSD", ".TGA", ".PSD" };

        for (int i = 0; i < imageName.Length; i++) {
            if (name.Equals(imageName[i]))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 改变按钮三种状态图片
    /// </summary>
    /// <param name="tex"></param>
    private void ChangeTexture(Texture2D tex) {
        GUI.skin.button.normal.background = tex;
        GUI.skin.button.hover.background = tex;
        GUI.skin.button.active.background = tex;
    }

}
