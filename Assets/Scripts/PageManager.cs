using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour {

    #region shared instance
    private static PageManager instance;
    public static PageManager SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("Page");
                if (go != null) {
                    PageManager comp = go.GetComponent<PageManager>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<PageManager>();
                } else {
                    go = new GameObject("Page");
                    return go.AddComponent<PageManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    public int maxPage;
    public int nowPage;
    public Text pageInput;

    public GameObject nextButton, prevButton;
    //public Scrollbar scrollBar;

    public void updatePage() {
        if (nowPage == maxPage) nextButton.SetActive(false);
        else nextButton.SetActive(true);
        if (nowPage == 1) prevButton.SetActive(false);
        else prevButton.SetActive(true);
        MapPanelManager.SharedInstance.updateMapPanel(nowPage);
        GetComponent<MessageManager>().newMessage(nowPage.ToString() + " / " + maxPage);
        //scrollBar.value = 1.0f;
    }

    public void resetPage(int max) {
        if (max <= 0) max = 1;
        nowPage = 1;
        maxPage = max;
        nextButton.SetActive(true);
        prevButton.SetActive(true);
        updatePage();
    }

    public void nextPage() {
        if (nowPage == maxPage) return;
        nowPage++;
        //MapPanelManager.SharedInstance.updateMapPanel(nowPage);
        updatePage();
    }

    public void prevPage() {
        if (nowPage == 1) return;
        nowPage--;
        //MapPanelManager.SharedInstance.updateMapPanel(nowPage);
        updatePage();
    }

    public void jumpPage(int pageIndex) {
        if (pageIndex < 1) pageIndex = 1;
        else if (pageIndex > maxPage) pageIndex = maxPage;
        nowPage = pageIndex;
        //MapPanelManager.SharedInstance.updateMapPanel(nowPage);
        updatePage();
    }

    public void inputJumpPage() {
        jumpPage(int.Parse(pageInput.text));
    }
}
