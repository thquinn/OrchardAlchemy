using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public BoardManagerScript boardManagerScript;
    public TextMeshProUGUI tmpMoney;
    public CanvasGroup canvasGroupMoney;

    float lastMoney, vMoney;

    void Update() {
        lastMoney = Mathf.SmoothDamp(lastMoney, boardManagerScript.state.cents, ref vMoney, .1f);
        tmpMoney.text = "$" + (lastMoney / 100).ToString("N2");
    }
}
