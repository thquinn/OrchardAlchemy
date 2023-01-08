using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GadgetDrawerScript : MonoBehaviour
{
    public GameObject prefabGadgetRow;

    public RectTransform rectTransformDrawer, rectTransformButton, rectTransformArrowIcon, rectTransformGadgetPanel;
    public BoardManagerScript boardManagerScript;

    bool open;
    HashSet<EntitySubtype> instantiatedGadgetTypes;
    Vector2 vDrawer, vButton;

    void Start() {
        instantiatedGadgetTypes = new HashSet<EntitySubtype>();
    }

    void Update() {
        bool pointerOnUI = EventSystem.current.IsPointerOverGameObject();
        if (open && Input.GetMouseButtonDown(0) && !pointerOnUI) {
            Toggle();
        }
        rectTransformDrawer.pivot = Vector2.SmoothDamp(rectTransformDrawer.pivot, open ? new Vector2(1, 0) : new Vector2(0, 0), ref vDrawer, .2f, float.MaxValue, Time.unscaledDeltaTime);
        rectTransformButton.anchoredPosition = Vector2.SmoothDamp(rectTransformButton.anchoredPosition, open ? new Vector2(0, 20) : new Vector2(-20, 20), ref vButton, .2f, float.MaxValue, Time.unscaledDeltaTime);
        AddGadgetRows();
    }
    public void Toggle() {
        open = !open;
        if (open) {
            EventSystem.current.SetSelectedGameObject(rectTransformDrawer.gameObject);
        }
        rectTransformArrowIcon.anchoredPosition = new Vector2(-rectTransformArrowIcon.anchoredPosition.x, 0);
        rectTransformArrowIcon.localScale = new Vector3(-rectTransformArrowIcon.localScale.x, 1, 1);
    }

    void AddGadgetRows() {
        State state = boardManagerScript.state;
        HashSet<EntitySubtype> typesToShow = new HashSet<EntitySubtype>(state.storedGadgets.Keys);
        typesToShow.UnionWith(state.progression.gadgetCosts.Keys);
        typesToShow.ExceptWith(instantiatedGadgetTypes);
        foreach (EntitySubtype type in typesToShow) {
            Instantiate(prefabGadgetRow, rectTransformGadgetPanel).GetComponent<GadgetRowScript>().Init(boardManagerScript, Util.GetGadgetInstanceFromSubtype(type));
            instantiatedGadgetTypes.Add(type);
        }
    }
}
