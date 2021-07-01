using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class EventRegistTest : MonoBehaviour {

    public Button btn;
    public GameObject confirmButton;
    public int id;
    public string path;
    public string delegateMethod;


    public delegate void buttonFunction();
    private buttonFunction handlerButtonFunction;

    void Start() {
        Button btnm = this.GetComponent<Button>();
        btnm.onClick.AddListener(TaskOnClick);
        if (delegateMethod.Equals("pathButtonMethod")) handlerButtonFunction = pathButtonMethod;
        else if (delegateMethod.Equals("clearButtonMethod")) handlerButtonFunction = clearButtonMethod;
    }

    void TaskOnClick() {
        // 注册按钮的点击事件
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(delegate () {
            // this.Btn_Test();
            handlerButtonFunction();
        });
        btn.onClick.AddListener(delegate () {
            WindowManeger.SharedInstance.hide("check");
        });
        // print("method registerd");
    }

    // 按钮点击事件的方法
    void pathButtonMethod() {
        // print("path");
        // 删除xml该文件夹信息
        MainController.SharedInstance.deleteXmlNode(path);
        MainController.SharedInstance.saveXmlFile();
        // 已变更路径信息，设置flag
        confirmButton.GetComponent<ConfirmUpdateChecker>().isPathChanged = true;
        // 隐藏按钮
        DeleteWindowManager.SharedInstance.deleteButton(id);
        // print("registerd method played");
    }

    void clearButtonMethod() {
        // print("clear");
        MainController.SharedInstance.clearXml();
        MainController.SharedInstance.startMapUpdate();
        DeleteWindowManager.SharedInstance.closeDeleteWindow();
        WindowManeger.SharedInstance.hide("delete");
    }
}