using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StorageButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public bool hovered;

    public void OnPointerEnter(PointerEventData eventData) {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        hovered = false;
    }
}
