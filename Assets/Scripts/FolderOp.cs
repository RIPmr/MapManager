using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Runtime.InteropServices;
using System;


public class FolderOp : MonoBehaviour {

    private float tim1 = 0;
    private int tim2 = 0;
    private String fname;
    public bool styleChanged = false;
    public RawImage ri;

    void Start() {
        fname = "From Sunset To Sunrise";
    }
    void Update() {

        if (styleChanged) {
            styleChanged = false;
        }

        tim1 += Time.deltaTime;
        if (tim1 >= 60) {
            tim2++;
            tim1 = 0;
        }
    }
    void OnGUI() {
        if (GUI.Button(new Rect(0, 0, 100, 25), "OpenDialog")) {
            OpenFileName ofn = new OpenFileName();
            ofn.structSize = Marshal.SizeOf(ofn);
            ofn.filter = "All Files\0*.*\0\0";
            ofn.file = new string(new char[256]);
            ofn.maxFile = ofn.file.Length;
            ofn.fileTitle = new string(new char[64]);
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = UnityEngine.Application.dataPath;
            ofn.title = "Choose Folder";
            ofn.defExt = "jpg";
            ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            if (LocalDialog.GetOpenFileName(ofn)) {
                StartCoroutine(WaitLoad(ofn.file, ofn.fileTitle));
                Debug.Log("Selected file with full path: " + ofn.file);
            }

        }

        if (GUI.Button(new Rect(0, 0, 100, 75), "SaveDialog")) {
            OpenFileName ofn = new OpenFileName();
            ofn.structSize = Marshal.SizeOf(ofn);
            ofn.filter = "All Files\0*.*\0\0";
            ofn.file = new string(new char[256]);
            ofn.maxFile = ofn.file.Length;
            ofn.fileTitle = new string(new char[64]);
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = UnityEngine.Application.dataPath;
            ofn.title = "Choose Folder";
            ofn.defExt = "jpg";
            ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            if (LocalDialog.GetSaveFileName(ofn)) {
                //StartCoroutine(WaitLoad(ofn.file, ofn.fileTitle));
                Debug.Log("Selected file with full save path: " + ofn.file);
            }

        }

        //if (GUI.Button(new Rect(730, 0, 50, 25), "Quit")) {
        //    Application.Quit();
        //}

    }

    IEnumerator WaitLoad(string fileName, string fn) {
        WWW wwwTexture = new WWW("file://" + fileName);

        Debug.Log(wwwTexture.url);

        yield return wwwTexture;

        ri.texture = wwwTexture.texture;
        ri.SetNativeSize();

        tim1 = tim2 = 0;
        fname = fn;
        for (int i = 0; ; i++) {
            if (fname[i] == '.') {
                fname = fname.Substring(0, i);
                break;
            }
        }

    }

}