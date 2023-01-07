using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManagerScript : MonoBehaviour {
    public GameObject prefabEntity, prefabGadgetRow;

    public RectTransform rectTransformGadgetPanel;

    Camera cam;
    public State state;
    Dictionary<Entity, EntityScript> entityScripts;
    Entity draggedEntity;
    EntityScript draggedEntityScript;

    void Start() {
        cam = Camera.main;
        state = new State();
        entityScripts = new Dictionary<Entity, EntityScript>();
        EntityCheck();

        AddGadgetRow(new EntityFlinger(null, Vector2Int.zero));
    }

    void Update() {
        if (Mathf.FloorToInt(Time.time - Time.deltaTime) < Mathf.FloorToInt(Time.time)) {
            Tick();
        }
        UpdateDraggedEntity();
        // DEBUG
        if (Input.GetKeyDown(KeyCode.F2)) {
            Time.timeScale += 1;
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
            } else {
                return;
            }
            
        }
        if (!Input.GetMouseButton(0)) {
            draggedEntity.coor = coor;
            if (!state.SpawnEntity(draggedEntity)) {
                state.StoreEntity(draggedEntity);
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
        return type == EntityType.Gadget || type == EntityType.Fruit;
    }

    void AddGadgetRow(EntityGadget gadget) {
        Instantiate(prefabGadgetRow, rectTransformGadgetPanel).GetComponent<GadgetRowScript>().Init(this, gadget);
    }
    public void DragNewGadget(EntityGadget gadget) {
        if (gadget is EntityFlinger) {
            gadget = new EntityFlinger(state, Vector2Int.zero);
        } else {
            throw new System.Exception("Unknown gadget type: " + gadget.name);
        }
        draggedEntity = gadget;
        draggedEntityScript = Instantiate(prefabEntity, transform).GetComponent<EntityScript>();
        draggedEntityScript.Init(gadget);
    }
}
