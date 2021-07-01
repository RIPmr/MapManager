using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmUpdateChecker : MonoBehaviour {

    public bool isPathChanged = false;

    void Start() {
        Button btnm = this.GetComponent<Button>();
        btnm.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick() {
        if (isPathChanged) {
            isPathChanged = false;
            MainController.SharedInstance.startMapUpdate();
        }
    }

}
