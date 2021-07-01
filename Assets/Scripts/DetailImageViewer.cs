using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailImageViewer : MonoBehaviour {

    #region shared instance
    private static DetailImageViewer instance;
    public static DetailImageViewer SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("DetailViewManager");
                if (go != null) {
                    DetailImageViewer comp = go.GetComponent<DetailImageViewer>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<DetailImageViewer>();
                } else {
                    go = new GameObject("DetailViewManager");
                    return go.AddComponent<DetailImageViewer>();
                }
            }
            return instance;
        }
    }
    #endregion

    public Texture2D originTex;

    public bool newStyle = false;
    public GameObject st1_obj, st2_obj;
    public GameObject[] containers;
    public RectTransform[] rtComps;
    public Slider rotateCon, tile_x_con, tile_y_con, offset_x_con, offset_y_con;
    public Text picName;
    public Text picSize;

    public float nowShowScale = 1.0f;

    private Vector2 originMousePos;
    private bool grab = false;
    private bool canGrab = false;
    private int stNum = 0;

    public void isShowView(bool isShow) {
        canGrab = isShow;
        if (isShow) {
            if (newStyle) st2_obj.SetActive(true);
            else st1_obj.SetActive(true);
        } else {
            st2_obj.SetActive(false);
            st1_obj.SetActive(false);
        }
    }

    public void setStyle(bool isNewStyle) {
        newStyle = isNewStyle;
        stNum = newStyle ? 1 : 0;
    }

    public void setImage(ref Texture2D newImage) {
        originTex = newImage;
        containers[stNum].GetComponent<RawImage>().texture = originTex;
    }

    public void setNative() {
        containers[stNum].GetComponent<RawImage>().SetNativeSize();
        //rtComps[stNum].position = Vector3.zero;
        nowShowScale = 1.0f;
    }

    public void fitContainer() {
        rtComps[stNum].localScale = Vector3.one;
        rtComps[stNum].sizeDelta = new Vector2(100.0f, 100.0f);
        //rtComps[stNum].position = Vector3.zero;
        nowShowScale = 1.0f;
    }

    public void updateImage() {
        rtComps[stNum].localScale = new Vector3(nowShowScale, nowShowScale, nowShowScale);
    }

    public void updateDetailMeaasges(string name, string size) {
        picName.text = name;
        picSize.text = size;
    }

    private void Start() {
        containers[stNum].GetComponent<RawImage>().texture = originTex;
        setNative();
    }

    public void rotateControl() {
        rtComps[stNum].rotation = Quaternion.Euler(0, 0, rotateCon.value);
    }

    public void tileControl() {
        Rect newRect = new Rect(containers[0].GetComponent<RawImage>().uvRect);
        newRect.width = tile_x_con.value;
        newRect.height = tile_y_con.value;
        containers[0].GetComponent<RawImage>().uvRect = newRect;
    }

    public void offsetControl() {
        Rect newRect = new Rect(containers[0].GetComponent<RawImage>().uvRect);
        newRect.x = offset_x_con.value;
        newRect.y = offset_y_con.value;
        containers[0].GetComponent<RawImage>().uvRect = newRect;
    }

    void Update() {
        //鼠标滚轮的效果
        //Camera.main.fieldOfView 摄像机的视野
        //Camera.main.orthographicSize 摄像机的正交投影
        //Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (nowShowScale > 0.01f) nowShowScale -= 0.1f * nowShowScale;
            else if (nowShowScale <= 0.01f) nowShowScale = 0.01f;
            updateImage();
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (nowShowScale < 2.0f) nowShowScale += 0.1f * nowShowScale;
            else if (nowShowScale >= 2.0f) nowShowScale = 2.0f;
            updateImage();
        }

        if (canGrab && Input.GetMouseButtonDown(0) && (!newStyle) && Input.mousePosition.x < Screen.currentResolution.width / 2) {
            originMousePos = Input.mousePosition;
            grab = true;
        } else if (canGrab && Input.GetMouseButtonDown(0) && newStyle) {
            originMousePos = Input.mousePosition;
            grab = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            grab = false;
        }

        if (grab) {
            if (originMousePos.x != Input.mousePosition.x && originMousePos.y != Input.mousePosition.y) {
                Vector2 deltaPos = new Vector2(Input.mousePosition.x - originMousePos.x, Input.mousePosition.y - originMousePos.y);
                rtComps[stNum].position = new Vector3(rtComps[stNum].position.x + deltaPos.x, rtComps[stNum].position.y + deltaPos.y, rtComps[stNum].position.z);
                originMousePos = Input.mousePosition;
            }
        }

    }
}
