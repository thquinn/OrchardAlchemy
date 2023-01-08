using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceScript : MonoBehaviour
{
    public AudioSource ambienceNear, ambienceDistant;

    Camera cam;

    void Start() {
        cam = Camera.main;
    }
    void Update() {
        ambienceNear.volume = 3 / cam.orthographicSize;
        ambienceDistant.volume = (cam.orthographicSize * 2) / (cam.orthographicSize * 2 + 30);
    }
}
