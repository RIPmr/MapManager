using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    #region shared instance
    private static SceneLoader instance;
    public static SceneLoader SharedInstance {
        get {
            if (instance == null) {
                GameObject go = GameObject.Find("MeaageHolder_Static");
                if (go != null) {
                    SceneLoader comp = go.GetComponent<SceneLoader>();
                    if (comp != null) {
                        return comp;
                    }
                    return go.AddComponent<SceneLoader>();
                } else {
                    go = new GameObject("MeaageHolder_Static");
                    return go.AddComponent<SceneLoader>();
                }
            }
            return instance;
        }
    }
    #endregion

    public bool autoLoadOnAwake = true;
    public bool clearMessageHolder = false;
    public Slider loadingSlider;
    public string sceneName = "Gallery";
    public Text loadingText;

    private float loadingSpeed = 1;
    private float targetValue;
    private AsyncOperation operation;


    private void Awake() {
        if (autoLoadOnAwake) Screen.SetResolution(1600, 900, false);
    }

    void Start() {
        loadingSlider.value = 0.0f;
        if (autoLoadOnAwake) StartCoroutine(AsyncLoading());
    }

    public void startLoadingScene(string name) {
        if (name != null) sceneName = name;
        if (clearMessageHolder) Destroy(GameObject.Find("MeaageHolder_Static"));
        StartCoroutine(AsyncLoading());
        autoLoadOnAwake = true;
    }

    IEnumerator AsyncLoading() {
        operation = SceneManager.LoadSceneAsync(sceneName);
        //阻止当加载完成自动切换
        operation.allowSceneActivation = false;
        yield return operation;
    }

    void Update() {
        if (autoLoadOnAwake) {
            targetValue = operation.progress;

            if (operation.progress >= 0.9f) {
                //operation.progress的值最大为0.9
                targetValue = 1.0f;
            }

            if (targetValue != loadingSlider.value) {
                //插值运算
                loadingSlider.value = Mathf.Lerp(loadingSlider.value, targetValue, Time.deltaTime * loadingSpeed);
                if (Mathf.Abs(loadingSlider.value - targetValue) < 0.01f) {
                    loadingSlider.value = targetValue;
                }
            }

            if (loadingText != null) loadingText.text = ((int)(loadingSlider.value * 100)).ToString("0.#") + " %";

            if ((int)(loadingSlider.value * 100) == 100) {
                //允许异步加载完毕后自动切换场景
                operation.allowSceneActivation = true;
            }
        }
    }

}

