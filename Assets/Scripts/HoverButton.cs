using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string hoverToChangeText;

    private string originText;

    public void OnPointerEnter(PointerEventData eventData) {
        originText = this.GetComponent<Text>().text;
        this.GetComponent<Text>().text = hoverToChangeText;
    }

    public void OnPointerExit(PointerEventData eventData) {
        this.GetComponent<Text>().text = originText;
    }
}