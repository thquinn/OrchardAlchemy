using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GadgetDrawerScript : MonoBehaviour
{
    public RectTransform rectTransformDrawer, rectTransformButton, rectTransformArrowIcon;

    bool open;
    Vector2 vDrawer, vButton;

    void Update() {
        if (open && EventSystem.current.currentSelectedGameObject != rectTransformDrawer.gameObject) {
            Toggle();
        }
        rectTransformDrawer.pivot = Vector2.SmoothDamp(rectTransformDrawer.pivot, open ? new Vector2(1, 0) : new Vector2(0, 0), ref vDrawer, .2f, float.MaxValue, Time.unscaledDeltaTime);
        rectTransformButton.anchoredPosition = Vector2.SmoothDamp(rectTransformButton.anchoredPosition, open ? new Vector2(0, 20) : new Vector2(-20, 20), ref vButton, .2f, float.MaxValue, Time.unscaledDeltaTime);
    }

    public void Toggle() {
        open = !open;
        if (open) {
            EventSystem.current.SetSelectedGameObject(rectTransformDrawer.gameObject);
        }
        rectTransformArrowIcon.anchoredPosition = new Vector2(-rectTransformArrowIcon.anchoredPosition.x, 0);
        rectTransformArrowIcon.localScale = new Vector3(-rectTransformArrowIcon.localScale.x, 1, 1);
    }
}
