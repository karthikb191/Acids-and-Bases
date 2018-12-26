using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnim : MonoBehaviour {
    Animator cameraAnimator;
	// Use this for initialization
	void Start () {
        cameraAnimator = Camera.main.GetComponent<Animator>();
	}

    public void GoDown() {
        cameraAnimator.SetBool("GoDown", true);
    }
    public void GoUp() {
        cameraAnimator.SetBool("GoDown", false);
    }
}
