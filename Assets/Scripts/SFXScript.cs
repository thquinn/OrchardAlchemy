using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXScript : MonoBehaviour
{
    public static SFXScript instance;

    public AudioSource sfxFling, sfxLift, sfxPlace;

    void Start() {
        instance = this;
    }

    public void SFXFling(float volume) {
        sfxFling.PlayOneShot(sfxFling.clip, volume);
    }
    public void SFXLift(float volume) {
        sfxLift.PlayOneShot(sfxLift.clip, volume);
    }
    public void SFXPlace(float volume) {
        sfxPlace.PlayOneShot(sfxPlace.clip, volume);
    }
}
