using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicButton : MonoBehaviour {

    private Vector2 mousePos;

    public int updatePerFrame = 2;
    public float adjustDist = 1.0f;
    public float decRate = 0.1f;
    public float destSize = 18.22f;
    public bool isActive = false;

    private int nowUpdateNum = 0;
    private RectTransform rcComp;

    private void Start() {
        rcComp = this.GetComponent<RectTransform>();
    }

    void Update () {
        if (isActive) {
            if (nowUpdateNum < updatePerFrame) nowUpdateNum++;
            else if (nowUpdateNum >= updatePerFrame) {

                nowUpdateNum = 0;
                mousePos = Input.mousePosition;
                float dist = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.y), mousePos);
                Vector2 newSize = rcComp.sizeDelta;
                if (dist <= adjustDist) {
                    if (newSize.y < destSize) newSize.y += decRate;
                    else if(newSize.y > destSize) newSize.y = destSize;
                } else {
                    if (newSize.y > 0.0f) newSize.y -= decRate;
                    else if (newSize.y < 0.0f) newSize.y = 0.0f;
                }
                rcComp.sizeDelta = newSize;

            }
        }
	}
}
