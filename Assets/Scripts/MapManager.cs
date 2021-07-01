using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Runtime.InteropServices;
using System;
using System.IO;

public class MapManager : MonoBehaviour {

    public Texture2D originTex;

    public GameObject container;
    public Text title;

    public float rectScale;
    public string name;
    public string path;
    public int id;

    public string savePath;

    //public void UpdateShowTex(Texture2D originTex, string name, string path) {
    //    if (originTex == null) return;
    //    this.originTex = originTex;
    //    container.GetComponent<RawImage>().texture = this.originTex;
    //    float maxlen = Mathf.Max(originTex.width, originTex.height);
    //    float rescalePercentage = rectScale / maxlen;
    //    container.GetComponent<RectTransform>().sizeDelta = new Vector2(this.originTex.width * rescalePercentage, this.originTex.height * rescalePercentage);

    //    int pathLen = name.Length;
    //    string cutedPath;
    //    if (pathLen > 15) {
    //        cutedPath = name.Substring(0, 15);
    //        title.text =  cutedPath + "...";
    //    } else title.text =  name;

    //    this.name = name;
    //    this.path = path;
    //}

    public void linkMap(Texture2D originTex) {
        if (originTex == null) return;
        this.originTex = originTex;
    }

    public void UpdateFrame(string name, string path, int id = 0) {
        container.GetComponent<RawImage>().texture = this.originTex;
        if (originTex != null) {
            float maxlen = Mathf.Max(originTex.width, originTex.height);
            float rescalePercentage = rectScale / maxlen;
            container.GetComponent<RectTransform>().sizeDelta = new Vector2(this.originTex.width * rescalePercentage, this.originTex.height * rescalePercentage);
        }

        int pathLen = name.Length;
        string cutedPath;
        if (pathLen > 15) {
            cutedPath = name.Substring(0, 15);
            title.text = cutedPath + "...";
        } else title.text = name;

        this.name = name;
        this.path = path;
        this.id = id;
    }

    public void showImageDetail() {
        DetailImageViewer.SharedInstance.setImage(ref originTex);
        DetailImageViewer.SharedInstance.updateDetailMeaasges(name, "origin size : " + originTex.width.ToString() + " * " + originTex.height.ToString());
    }

    public void saveMap() {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "All Files\0*.*\0\0";
        ofn.file = new string(new char[2048]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath;
        ofn.title = "Choose Folder";
        ofn.defExt = "jpg";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        ofn.file = new string(name.ToCharArray());
        if (LocalDialog.GetSaveFileName(ofn)) {
            Debug.Log("Selected file with full save path: " + ofn.file);
            savePath = ofn.file;
            // TexTools.CopyAFile(path, savePath);
            CopyFile(path, savePath);
        }
    }

    void CopyFile(string srcPath, string tarPath) {
        string[] filesList = Directory.GetFiles(srcPath);
        foreach (string f in filesList) {
            string fTarPath = tarPath;
            if (File.Exists(fTarPath)) {
                File.Copy(f, fTarPath, true);
            } else {
                File.Copy(f, fTarPath);
            }
        }
    }

    private void Awake() {
        rectScale = container.GetComponent<RectTransform>().sizeDelta.x;
    }
}
