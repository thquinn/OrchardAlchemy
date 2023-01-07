using Assets.Code.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManagerScript : MonoBehaviour
{
    public GameObject prefabEntity;

    public State state;
    Dictionary<Entity, EntityScript> entityScripts;

    void Start() {
        state = new State();
        entityScripts = new Dictionary<Entity, EntityScript>();
        EntityCheck();
    }

    void Update() {
        if (Mathf.FloorToInt(Time.time - Time.deltaTime) < Mathf.FloorToInt(Time.time)) {
            Tick();
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
}
