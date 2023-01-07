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
    };

    public GameObject prefabFruitCost;

    public BoardManagerScript boardManagerScript;
    public TextMeshProUGUI tmpMoney, tmpTutorial;
    public CanvasGroup canvasGroupMoney, canvasGroupFruit;
    public RectTransform rectTransformFruitPanel;

    float lastMoney, vMoney;
    float vAlphaMoney, vAlphaFruit;
    Dictionary<int, FruitCostRowScript> massToFruitRowScripts;

    void Start() {
        massToFruitRowScripts = new Dictionary<int, FruitCostRowScript>();
    }

    void Update() {
        State state = boardManagerScript.state;
        bool showMoney = state.totalCentsEarned > 0;
        canvasGroupMoney.alpha = Mathf.SmoothDamp(canvasGroupMoney.alpha, showMoney ? 1 : 0, ref vAlphaMoney, .2f, float.MaxValue, Time.unscaledDeltaTime);
        bool showFruit = showMoney && state.storedFruit.Count > 0;
        canvasGroupFruit.alpha = Mathf.SmoothDamp(canvasGroupFruit.alpha, showFruit ? 1 : 0, ref vAlphaFruit, .2f, float.MaxValue, Time.unscaledDeltaTime);
        lastMoney = Mathf.SmoothDamp(lastMoney, state.cents, ref vMoney, .1f);
        tmpMoney.text = string.Format("${0}<size=50%> / ${1}", (lastMoney / 100).ToString("N2"), state.progression.maxCents / 100);
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
    }
}
