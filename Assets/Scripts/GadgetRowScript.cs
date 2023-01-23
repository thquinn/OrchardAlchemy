using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GadgetRowScript : MonoBehaviour
{
    public GameObject prefabFruitCostRow;
    public Sprite[] gadgetIcons;

    public RectTransform rectTransformCostSection;
    public EventTrigger dragTrigger;
    public Transform transformCostSection;
    public TextMeshProUGUI tmpHeader, tmpName, tmpMoneyCost;
    public Image gadgetIcon;

    GadgetCost cachedCost;
    int cachedStoredAmount;
    EntitySubtype gadgetType;
    State state;

    public void Init(BoardManagerScript boardManagerScript, EntityGadget gadget) {
        gadgetType = gadget.subtype;
        state = boardManagerScript.state;
        tmpName.text = gadget.name;
        gadgetIcon.sprite = gadgetIcons.First(i => i.name == "icon_gadget_" + gadget.name.ToLower());
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((eventData) => boardManagerScript.DragNewGadget(gadget));
        dragTrigger.triggers.Add(entry);
    }

    void Update() {
        GadgetCost cost = state.progression.gadgetCosts.ContainsKey(gadgetType) ? state.progression.gadgetCosts[gadgetType] : null;
        int storedAmount = state.GetStoredGadgetCount(gadgetType);
        if (cost == cachedCost && storedAmount == cachedStoredAmount) {
            return;
        }
        cachedCost = cost;
        cachedStoredAmount = storedAmount;
        for (int i = rectTransformCostSection.childCount - 1; i >= 2; i--) {
            Destroy(rectTransformCostSection.GetChild(i).gameObject);
        }
        if (cost == null || storedAmount > 0) {
            tmpHeader.text = "Inventory: " + cachedStoredAmount;
            tmpMoneyCost.gameObject.SetActive(false);
        } else {
            tmpHeader.text = "Cost";
            tmpMoneyCost.gameObject.SetActive(true);
            tmpMoneyCost.text = "$" + (cost.cents / 100f).ToString("N2");
            foreach (Vector2Int massAndAmount in cost.massesAndAmounts) {
                Instantiate(prefabFruitCostRow, rectTransformCostSection).GetComponent<FruitCostRowScript>().Init(massAndAmount.x, (ulong) massAndAmount.y);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformCostSection);
        }
    }
}
