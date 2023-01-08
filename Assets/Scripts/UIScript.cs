using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    static Dictionary<ProgressionPhase, string> TUTORIAL_STRINGS = new Dictionary<ProgressionPhase, string>() {
        { ProgressionPhase.Start, "Drag apples next to the Market to sell them." },
        { ProgressionPhase.TutorialFlinger, "Click the button in the lower right and drag a Flinger onto the highlighted spot." },
        { ProgressionPhase.TutorialBlocker, "Place a Blocker in the highlighted spot, then use another Flinger to reach the new Market." },
        { ProgressionPhase.SecondTree, "Sell apples and pears." },
        { ProgressionPhase.SecondTreeMoney, "Sell fruit until you can afford a Fuser." },
        { ProgressionPhase.TutorialFuser, "Fuse an apple and pear into a quince, then sell it." },
        { ProgressionPhase.FuserMoney, "Earn money to buy a Lab." },
        { ProgressionPhase.TutorialResearch, "Each fruit can be researched for a unique effect. Use a Lab to research the quince." },
        { ProgressionPhase.TutorialResearchAgain, "Use the Lab to research any other fruit." },
    };

    public GameObject prefabFruitCost;

    public BoardManagerScript boardManagerScript;
    public TextMeshProUGUI tmpMoney, tmpResearchProgress, tmpResearchName, tmpResearchMass, tmpTutorial, tmpTimescale;
    public CanvasGroup canvasGroupMoney, canvasGroupResearch, canvasGroupFruit, canvasGroupGadgetDrawerButton, canvasGroupTickRate, canvasGroupSlowDown, canvasGroupSpeedUp;
    public RectTransform rectTransformFruitPanel;
    public Image imageTickDisc, imageResearchFruitGradient;

    float lastMoney, vMoney;
    float vAlphaMoney, vAlphaResearch, vAlphaFruit, vAlphaGadgetDrawerButton;
    Dictionary<int, FruitCostRowScript> massToFruitRowScripts;
    float lastTimeScale;
    Research lastResearch;

    void Start() {
        massToFruitRowScripts = new Dictionary<int, FruitCostRowScript>();
    }

    void Update() {
        State state = boardManagerScript.state;
        bool showMoney = state.totalCentsEarned > 0;
        canvasGroupMoney.alpha = Mathf.SmoothDamp(canvasGroupMoney.alpha, showMoney ? 1 : 0, ref vAlphaMoney, .2f, float.MaxValue, Time.unscaledDeltaTime);
        bool showResearch = state.progression.research != null;
        canvasGroupResearch.alpha = Mathf.SmoothDamp(canvasGroupResearch.alpha, showResearch ? 1 : 0, ref vAlphaResearch, .2f, float.MaxValue, Time.unscaledDeltaTime);
        bool showFruit = state.storedFruit.Count > 0 && state.progression.gadgetCosts.ContainsKey(EntitySubtype.Storage);
        canvasGroupFruit.alpha = Mathf.SmoothDamp(canvasGroupFruit.alpha, showFruit ? 1 : 0, ref vAlphaFruit, .2f, float.MaxValue, Time.unscaledDeltaTime);
        bool showGadgetDrawerButton = state.progression.phase >= ProgressionPhase.TutorialFlinger;
        canvasGroupGadgetDrawerButton.alpha = Mathf.SmoothDamp(canvasGroupGadgetDrawerButton.alpha, showGadgetDrawerButton ? 1 : 0, ref vAlphaGadgetDrawerButton, .2f, float.MaxValue, Time.unscaledDeltaTime);
        lastMoney = Mathf.SmoothDamp(lastMoney, state.cents, ref vMoney, .1f);
        tmpMoney.text = string.Format("${0}<size=40%> max ${1}", (lastMoney / 100).ToString("N2"), state.progression.maxCents / 100);
        // Update research.
        if (state.progression.research != lastResearch) {
            lastResearch = state.progression.research;
            if (lastResearch != null) {
                tmpResearchName.text = Util.GetFruitNameFromMass(lastResearch.mass);
                tmpResearchMass.text = lastResearch.mass.ToString();
                imageResearchFruitGradient.color = Util.GetFruitColorFromMass(lastResearch.mass);
            }
        }
        if (lastResearch != null) {
            tmpResearchProgress.text = string.Format("{0}%<size=40%> {1}/{2}", Util.GetRoundedProgressPercent((float)lastResearch.progress / lastResearch.goal), lastResearch.progress, lastResearch.goal);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tmpResearchProgress.transform.parent as RectTransform);
        }
        // Update fruits.
        bool fruitsChanged = false;
        List<int> displayedMasses = new List<int>(massToFruitRowScripts.Keys);
        foreach (int displayedMass in displayedMasses) {
            if (state.storedFruit.ContainsKey(displayedMass)) {
                massToFruitRowScripts[displayedMass].tmpQuantity.text = state.storedFruit[displayedMass].ToString();
            } else {
                Destroy(massToFruitRowScripts[displayedMass].gameObject);
                massToFruitRowScripts.Remove(displayedMass);
                fruitsChanged = true;
            }
        }
        foreach (int storedMass in state.storedFruit.Keys) {
            if (!massToFruitRowScripts.ContainsKey(storedMass)) {
                FruitCostRowScript fruitCostRowScript = Instantiate(prefabFruitCost, rectTransformFruitPanel).GetComponent<FruitCostRowScript>();
                fruitCostRowScript.Init(storedMass, state.storedFruit[storedMass]);
                massToFruitRowScripts[storedMass] = fruitCostRowScript;
                fruitsChanged = true;
            }
        }
        if (fruitsChanged) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformFruitPanel);
        }
        // Update tutorial.
        tmpTutorial.text = TUTORIAL_STRINGS.ContainsKey(state.progression.phase) ? TUTORIAL_STRINGS[state.progression.phase] : "";
        // Update buttons.
        imageTickDisc.material.SetFloat("_Revealed", Time.time % 1);
        if (Time.timeScale != lastTimeScale) {
            bool showTimeControls = state.progression.timeScaleMinIndex != state.progression.timeScaleMaxIndex;
            bool showSlowDown = showTimeControls && boardManagerScript.timescaleIndex > state.progression.timeScaleMinIndex;
            bool showSpeedUp = showTimeControls && boardManagerScript.timescaleIndex < state.progression.timeScaleMaxIndex;
            canvasGroupSlowDown.alpha = showSlowDown ? 1 : 0;
            canvasGroupSpeedUp.alpha = showSpeedUp ? 1 : 0;
            tmpTimescale.text = Time.timeScale.ToString();
            canvasGroupTickRate.alpha = Time.timeScale == 1 ? 0 : 1;
            LayoutRebuilder.ForceRebuildLayoutImmediate(canvasGroupTickRate.transform as RectTransform);
            lastTimeScale = Time.timeScale;
        }
    }
}
