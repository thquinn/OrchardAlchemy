using Assets.Code;
using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManagerScript : MonoBehaviour {
    public GameObject prefabEntity;

    public StorageButtonScript storageButtonScript;

    public Camera cam;
    public State state;
    Dictionary<Entity, EntityScript> entityScripts;
    Entity draggedEntity;
    EntityScript draggedEntityScript;
    public int timescaleIndex;

    void Start() {
        cam = Camera.main;
        state = new State();
        entityScripts = new Dictionary<Entity, EntityScript>();
        EntityCheck();
        timescaleIndex = state.progression.timeScaleMinIndex;
    }

    void Update() {
        if (Mathf.FloorToInt(Time.time - Time.deltaTime) < Mathf.FloorToInt(Time.time)) {
            Tick();
        }
        UpdateDraggedEntity();
        // DEBUG
        if (Input.GetKeyDown(KeyCode.F1)) {
            state.GetMoney(10000000);
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            state.progression.timeScaleMaxIndex = Progression.TIMESCALES.Length - 1;
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            state.StoreGadgetType(EntitySubtype.Blocker);
            state.StoreGadgetType(EntitySubtype.Flinger);
            state.StoreGadgetType(EntitySubtype.Fuser);
            state.StoreGadgetType(EntitySubtype.Lab);
            state.StoreGadgetType(EntitySubtype.Storage);
        }
        if (Input.GetKeyDown(KeyCode.F4)) {
            state.progression.SpawnEpoch();
            EntityCheck();
        }
        if (Input.GetKeyDown(KeyCode.F6)) {
            state.progression.researchFlags.Add(ResearchFlags.ApproximateResearch);
            state.progression.researchFlags.Add(ResearchFlags.ConditionalFlingers);
            state.progression.researchFlags.Add(ResearchFlags.PrimeBonus);
            state.progression.researchFlags.Add(ResearchFlags.SuperLemon);
        }
    }
    void Tick() {
        state.Tick();
        EntityCheck();
    }
    void EntityCheck() {
        // Removing destroyed entities.
        List<Entity> toRemove = new List<Entity>();
        foreach (Entity entity in entityScripts.Keys) {
            if (entity.state == null) {
                toRemove.Add(entity);
            }
        }
        foreach (Entity entity in toRemove) {
            Destroy(entityScripts[entity].gameObject);
            entityScripts.Remove(entity);
        }
        // Spawning new entities.
        foreach (Entity entity in state.entities.Values) {
            if (!entityScripts.ContainsKey(entity)) {
                EntityScript entityScript = Instantiate(prefabEntity, transform).GetComponent<EntityScript>();
                entityScript.Init(entity);
                entityScripts[entity] = entityScript;
            }
        }
    }

    void UpdateDraggedEntity() {
        Vector3 worldMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int coor = new Vector2Int(Mathf.RoundToInt(worldMousePosition.x), Mathf.RoundToInt(worldMousePosition.y));
        if (draggedEntity == null) {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && IsDraggableType(state.GetTypeAtCoor(coor))) {
                draggedEntity = state.entities[coor];
                state.RemoveEntity(draggedEntity);
                draggedEntityScript = entityScripts[draggedEntity];
                entityScripts.Remove(draggedEntity);
                float sfxVolume = Util.GetVolume(coor);
                SFXScript.instance.SFXLift(sfxVolume);
            } else {
                return;
            }
        }
        // Drop.
        if (!Input.GetMouseButton(0) || !IsDraggableType(draggedEntity.type)) {
            bool storageHovered = storageButtonScript.hovered;
            draggedEntity.coor = coor;
            if (storageHovered) {
                state.StoreEntity(draggedEntity);
            } else {
                state.SpawnEntity(draggedEntity);
                float sfxVolume = Util.GetVolume(coor);
                SFXScript.instance.SFXPlace(sfxVolume);
            }
            EntityCheck();
            draggedEntity = null;
            Destroy(draggedEntityScript.gameObject);
            draggedEntityScript = null;
            return;
        }
        worldMousePosition.z = -10;
        draggedEntityScript.transform.localPosition = worldMousePosition;
    }
    bool IsDraggableType(EntityType type) {
        if (state.progression.phase == ProgressionPhase.TutorialFlinger || state.progression.phase == ProgressionPhase.TutorialBlocker) {
            return type == EntityType.Gadget;
        }
        return type == EntityType.Gadget || type == EntityType.Fruit;
    }

    public void DragNewGadget(EntityGadget gadget) {
        // Pay or remove from storage.
        if (state.GetStoredGadgetCount(gadget.subtype) > 0) {
            state.UnstoreEntity(gadget);
        } else {
            GadgetCost cost = state.progression.gadgetCosts.ContainsKey(gadget.subtype) ? state.progression.gadgetCosts[gadget.subtype] : null;
            if (cost == null || !state.CheckAndPayCost(cost)) {
                return;
            } else {
                state.progression.IncrementCost(gadget.subtype);
            }
        }
        // Create and drag.
        gadget = Util.GetGadgetInstanceFromSubtype(gadget.subtype);
        draggedEntity = gadget;
        draggedEntityScript = Instantiate(prefabEntity, transform).GetComponent<EntityScript>();
        draggedEntityScript.Init(gadget);
    }

    public void SlowDown() {
        timescaleIndex = Mathf.Max(state.progression.timeScaleMinIndex, timescaleIndex - 1);
        Time.timeScale = Progression.TIMESCALES[timescaleIndex];
    }
    public void SpeedUp() {
        timescaleIndex = Mathf.Min(timescaleIndex + 1, state.progression.timeScaleMaxIndex);
        Time.timeScale = Progression.TIMESCALES[timescaleIndex];
    }
    public void CleanUpFruits() {
        state.CleanUpFruits();
        EntityCheck();
    }
    public void RecenterCamera() {
        state.progression.cameraTakeover = true;
        state.progression.cameraTargetPosition = Vector3.zero;
        state.progression.cameraTargetSize = 5;
    }
}
