using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalScript : MonoBehaviour
{
    static Dictionary<ProgressionPhase, ModalInfo> MODAL_INFOS = new Dictionary<ProgressionPhase, ModalInfo>() {
        { ProgressionPhase.TutorialFuser, new ModalInfo(){
            imageNames = new string[]{ "tutorial_fuser_1", "tutorial_fuser_2" },
            explanation = "Fusers combine fruit! If there are two fruit next to it, the combined fruit will be placed counterclockwise from them."
        } },
    };

    public GameObject prefabModalImage;
    public Sprite[] tutorialSprites;

    public BoardManagerScript boardManagerScript;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransformImages;
    public TextMeshProUGUI tmpExplanation;

    ProgressionPhase cachedProgressionPhase;

    void Update() {
        if (canvasGroup.interactable) {
            return;
        }
        if (boardManagerScript.state.progression.phase != cachedProgressionPhase) {
            cachedProgressionPhase = boardManagerScript.state.progression.phase;
            if (MODAL_INFOS.ContainsKey(cachedProgressionPhase)) {
                Invoke("Open", 1);
            }
        }
    }
    void Open() {
        ModalInfo info = MODAL_INFOS[cachedProgressionPhase];
        foreach (Transform child in rectTransformImages) {
            Destroy(child.gameObject);
        }
        foreach (string imageName in info.imageNames) {
            GameObject image = Instantiate(prefabModalImage, rectTransformImages);
            image.GetComponentInChildren<Image>().sprite = tutorialSprites.First(s => s.name == imageName);
        }
        tmpExplanation.text = info.explanation;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void Dismiss() {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}

public struct ModalInfo {
    public string[] imageNames;
    public string explanation;
}