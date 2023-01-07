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

    public EventTrigger dragTrigger;
    public Transform transformCostSection;
    public TextMeshProUGUI tmpName, tmpMoneyCost;
    public Image gadgetIcon;

    public void Init(BoardManagerScript boardManagerScript, EntityGadget gadget) {
        tmpName.text = gadget.name;
        gadgetIcon.sprite = gadgetIcons.First(i => i.name == "icon_gadget_" + gadget.name.ToLower());
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((eventData) => boardManagerScript.DragNewGadget(gadget));
        dragTrigger.triggers.Add(entry);
    }
}
