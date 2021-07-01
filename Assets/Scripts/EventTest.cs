using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class MyEventTrigger {
    public int id;
    public string name;
    public string message;

    public MyClickEvent m_MyEvent = new MyClickEvent();
}

[Serializable]
public class MyClickEvent : UnityEvent<string, string> { }

public class EventTest : MonoBehaviour {

    public Button yourButton;

    void Start() {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    [SerializeField]
    public MyEventTrigger[] m_MyEventTrigger = new MyEventTrigger[0];

    void TaskOnClick() {
        // Debug.Log("You have clicked the button!");
        int len = m_MyEventTrigger.Length;
        for (int i = 0; i < len; i++) {
            m_MyEventTrigger[i].m_MyEvent.Invoke(m_MyEventTrigger[i].name, m_MyEventTrigger[i].message);
        }
    }

}
