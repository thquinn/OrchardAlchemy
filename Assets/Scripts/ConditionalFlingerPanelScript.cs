using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConditionalFlingerPanelScript : MonoBehaviour
{
    public BoardManagerScript boardManagerScript;
    public CanvasGroup canvasGroup, canvasGroupOperations;
    public GameObject[] radioFillsType, radioFillsOperation;
    public TMP_InputField input;

    EntityFlinger flinger;

    void Start() {
        input.onEndEdit.AddListener(v => SetInputValue(v));
    }
    void Update() {
        State state = boardManagerScript.state;
        if (!state.progression.researchFlags.Contains(ResearchFlags.ConditionalFlingers)) {
            return;
        }
        // Check if clicked a flinger.
        Vector3 worldMousePosition = boardManagerScript.cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int coor = new Vector2Int(Mathf.RoundToInt(worldMousePosition.x), Mathf.RoundToInt(worldMousePosition.y));
        if (Input.GetMouseButtonDown(0)) {
            bool pointerOnUI = EventSystem.current.IsPointerOverGameObject();
            if (!pointerOnUI && state.GetSubtypeAtCoor(coor) == EntitySubtype.Flinger) {
                SetFlinger(state.entities[coor] as EntityFlinger);
            } else if (!pointerOnUI) {
                SetFlinger(null);
            }
        }
    }
    void SetFlinger(EntityFlinger newFlinger) {
        if (newFlinger == flinger) {
            return;
        }
        flinger = newFlinger;
        canvasGroup.alpha = flinger == null ? 0 : 1;
        canvasGroup.blocksRaycasts = flinger != null;
        canvasGroup.interactable = flinger != null;
        if (flinger != null) {
            SetControls();
        }
    }

    void SetConditionType(FlingerConditionType type) {
        if (type == FlingerConditionType.None) {
            flinger.condition = null;
        } else if (flinger.condition == null) {
            flinger.condition = new FlingerCondition(type, FlingerConditionOperation.Equals, 0);
        } else {
            flinger.condition.type = type;
        }
        SetControls();
    }
    void SetConditionOperation(FlingerConditionOperation operation) {
        flinger.condition.operation = operation;
        SetControls();
        input.ActivateInputField();
    }
    void SetInputValue(string v) {
        flinger.condition.value = int.Parse(v);
        SetControls();
    }
    void SetControls() {
        FlingerConditionType type = flinger.condition?.type ?? FlingerConditionType.None;
        int typeIndex = (int) type;
        for (int i = 0; i < radioFillsType.Length; i++) {
            radioFillsType[i].SetActive(i == typeIndex);
        }
        if (type == FlingerConditionType.None) {
            canvasGroupOperations.alpha = .1f;
            canvasGroupOperations.blocksRaycasts = false;
            canvasGroupOperations.interactable = false;
            foreach (GameObject radioFill in radioFillsOperation) {
                radioFill.SetActive(false);
            }
            input.text = "";
        } else {
            FlingerConditionOperation operation = flinger.condition.operation;
            int operationIndex = (int) operation;
            canvasGroupOperations.alpha = 1;
            canvasGroupOperations.blocksRaycasts = true;
            canvasGroupOperations.interactable = true;
            for (int i = 0; i < radioFillsOperation.Length; i++) {
                radioFillsOperation[i].SetActive(i == operationIndex);
            }
            input.text = flinger.condition.value.ToString();
        }
        FlingerConditionOperation indexOperation = flinger.condition?.operation ?? FlingerConditionOperation.Equals;
        Vector3 inputPosition = input.transform.parent.position;
        inputPosition.y = radioFillsOperation[(int)indexOperation].transform.position.y;
        input.transform.parent.position = inputPosition;
    }
    public void ClickNone() {
        SetConditionType(FlingerConditionType.None);
    }
    public void ClickMass() {
        SetConditionType(FlingerConditionType.Mass);
    }
    public void ClickReactivity() {
        SetConditionType(FlingerConditionType.Reactivity);
    }
    public void ClickEquals() {
        SetConditionOperation(FlingerConditionOperation.Equals);
    }
    public void ClickLessThan() {
        SetConditionOperation(FlingerConditionOperation.LessThan);
    }
    public void ClickGreaterThan() {
        SetConditionOperation(FlingerConditionOperation.GreaterThan);
    }
}
