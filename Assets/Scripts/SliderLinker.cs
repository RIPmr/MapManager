using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderLinker : MonoBehaviour {

    public Text[] textComp;
    public Slider[] sliderComp;
    public string prefix;
    public string suffix;

    public void silderChanged() {
        float num = GetComponent<Slider>().value;
        if(textComp != null)
            foreach(Text tc in textComp) {
                tc.text = prefix + num.ToString("0.#") + suffix;
            }
        if(sliderComp != null)
            foreach(Slider slc in sliderComp) {
                slc.value = num;
            }
    }

}
