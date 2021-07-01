using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeleteGalleryController : MonoBehaviour {

    public MapManager MMcomp;

    void Start() {
        Button btnm = this.GetComponent<Button>();
        btnm.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick() {
        GalleryManager.SharedInstance.deleteGallery(MMcomp.name, MMcomp.id);
        Destroy(MMcomp.gameObject);
    }

}
