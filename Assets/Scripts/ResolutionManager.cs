using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour {

    public void setResolution1600() {
        Screen.SetResolution(1600, 900, false);
    }

    public void setResolution800() {
        Screen.SetResolution(800, 450, false);
    }

}
