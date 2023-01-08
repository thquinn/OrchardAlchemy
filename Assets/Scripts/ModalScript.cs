using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalScript : MonoBehaviour
{
    static Dictionary<ProgressionPhase, ModalInfo> PROGRESSION_MODAL_INFOS = new Dictionary<ProgressionPhase, ModalInfo>() {
        { ProgressionPhase.TutorialFuser, new ModalInfo(){
            imageNames = new string[]{ "tutorial_fuser_1", "tutorial_fuser_2" },
            explanation = "Fusers combine fruit! If there are two fruit next to it, the combined fruit will be placed counterclockwise from them."
        } },
        { ProgressionPhase.FuserMoney, new ModalInfo(){
            imageNames = new string[]{ "tutorial_reactivity_1", "tutorial_reactivity_2" },
            explanation = "The pips under a fruit's number show its <color=red>reactivity:</color> the number of times it can be fused. Both fruits need it!"
        } },
    };

    public GameObject prefabModalImage;
    public Sprite[] tutorialSprites;

    public BoardManagerScript boardManagerScript;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransformImages;
    public TextMeshProUGUI tmpExplanation, tmpFruitName, tmpFruitMass;
    public Image imageFruitGradient;
    public GameObject goFruit;

    ProgressionPhase cachedProgressionPhase;
    HashSet<int> cachedFruitsResearched;

    void Start() {
        cachedFruitsResearched = new HashSet<int>(boardManagerScript.state.progression.fruitsResearched);
    }

    void Update() {
        if (canvasGroup.interactable) {
            return;
        }
        if (boardManagerScript.state.progression.phase != cachedProgressionPhase) {
            cachedProgressionPhase = boardManagerScript.state.progression.phase;
            if (PROGRESSION_MODAL_INFOS.ContainsKey(cachedProgressionPhase)) {
                Invoke("Open", 1);
            }
        }
        if (boardManagerScript.state.progression.fruitsResearched.Count > cachedFruitsResearched.Count) {
            int mass = boardManagerScript.state.progression.fruitsResearched.First(f => !cachedFruitsResearched.Contains(f));
            cachedFruitsResearched = new HashSet<int>(boardManagerScript.state.progression.fruitsResearched);
            OpenResearch(mass);
        }
    }
    void Open() {
        foreach (Transform child in rectTransformImages) {
            Destroy(child.gameObject);
        }
        goFruit.SetActive(false);
        ModalInfo info = PROGRESSION_MODAL_INFOS[cachedProgressionPhase];
        foreach (string imageName in info.imageNames) {
            GameObject image = Instantiate(prefabModalImage, rectTransformImages);
            image.transform.GetChild(0).GetComponent<Image>().sprite = tutorialSprites.First(s => s.name == imageName);
        }
        tmpExplanation.text = info.explanation;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
    void OpenResearch(int mass) {
        foreach (Transform child in rectTransformImages) {
            Destroy(child.gameObject);
        }
        goFruit.SetActive(true);
        tmpFruitName.text = Util.GetFruitNameFromMass(mass);
        tmpFruitMass.text = mass.ToString();
        Color c = Util.GetFruitColorFromMass(mass);
        c.a = imageFruitGradient.color.a;
        imageFruitGradient.color = c;
        tmpExplanation.text = Util.GetFruitResearchDescriptionFromMass(mass);
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