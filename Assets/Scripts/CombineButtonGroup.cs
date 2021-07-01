using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombineButtonGroup : MonoBehaviour {

    public Color activeColor;
    public int activeID;
    public Button[] buttonGroup;

    private ColorBlock originBlock, newBlock;

    private void Start() {
        originBlock = buttonGroup[0].colors;
        newBlock = buttonGroup[0].colors;
        newBlock.normalColor = activeColor;
        setActive(activeID);
    }

    public void setActive(int id) {
        int len = buttonGroup.Length;
        for (int i = 0; i < len; i++) {
            if (i == id) {
                buttonGroup[i].colors = newBlock;
            } else {
                buttonGroup[i].colors = originBlock;
            }
        }
    }

}
