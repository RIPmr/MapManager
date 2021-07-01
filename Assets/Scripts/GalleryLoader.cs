using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GalleryLoader : MonoBehaviour {

    public MapManager MMcomp;

    void Start() {
        Button btnm = this.GetComponent<Button>();
        btnm.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick() {
        MessageHolder.SharedInstance.setMessage(MMcomp.name);
        WindowManeger.SharedInstance.show("load");
        SceneLoader.SharedInstance.startLoadingScene(null);
    }

}
