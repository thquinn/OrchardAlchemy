using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void Start() {
        if (!Application.isEditor) {
            canvasGroup.alpha = 1;
        }
    }

    void Update() {
        if (Time.frameCount > 10) {
            canvasGroup.alpha -= Time.deltaTime * 3;
        }
    }
}
