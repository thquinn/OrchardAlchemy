using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public GameObject prefabFruitCost;

    public BoardManagerScript boardManagerScript;
    public TextMeshProUGUI tmpMoney;
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
        List<int> displayedMasses = new List<int>(massToFruitRowScripts.Keys);
        foreach (int displayedMass in displayedMasses) {
            if (state.storedFruit.ContainsKey(displayedMass)) {
                massToFruitRowScripts[displayedMass].tmpQuantity.text = state.storedFruit[displayedMass].ToString();
            } else {
                Destroy(massToFruitRowScripts[displayedMass].gameObject);
                massToFruitRowScripts.Remove(displayedMass);
            }
        }
        foreach (int storedMass in state.storedFruit.Keys) {
            if (!massToFruitRowScripts.ContainsKey(storedMass)) {
                FruitCostRowScript fruitCostRowScript = Instantiate(prefabFruitCost, rectTransformFruitPanel).GetComponent<FruitCostRowScript>();
                fruitCostRowScript.Init(storedMass, state.storedFruit[storedMass]);
                massToFruitRowScripts[storedMass] = fruitCostRowScript;
            }
        }
    }
}
