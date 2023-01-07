using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class State {
        public Dictionary<Vector2Int, Entity> entities;
        Dictionary<Vector2Int, Entity> spawningEntities;

        public State() {
            entities = new Dictionary<Vector2Int, Entity>();
            spawningEntities = new Dictionary<Vector2Int, Entity>();
            SpawnTree();
            EntityFlinger flinger = new EntityFlinger(this, new Vector2Int(2, 0));
            SpawnEntity(flinger);
        }

        public EntityType GetTypeAtCoor(Vector2Int coor) {
            return entities.ContainsKey(coor) ? entities[coor].type : EntityType.None;
        }

        public void SpawnEntity(Entity entity) {
            if (!entities.ContainsKey(entity.coor) && !spawningEntities.ContainsKey(entity.coor)) {
                spawningEntities[entity.coor] = entity;
            }
        }
        public void SpawnTree() {
            Vector3Int[] fruitTypesAndWeights = new Vector3Int[] {
                new Vector3Int(4, 1, 1),
            };
            EntityTree tree = new EntityTree(this, new Vector2Int(0, 0), fruitTypesAndWeights, new Vector2Int[] { Vector2Int.right });
            SpawnEntity(tree);
        }
        public void SpawnFruit(Vector2Int coor, int mass, int reactivity) {
            EntityFruit fruit = new EntityFruit(this, coor, mass, reactivity);
            SpawnEntity(fruit);
        }

        public void Tick() {
            foreach (Entity entity in entities.Values.Where(e => e.type == EntityType.Gadget).ToList()) {
                entity.Tick();
            }
            foreach (Entity entity in entities.Values.Where(e => e.type == EntityType.Tree).ToList()) {
                entity.Tick();
            }
            foreach (var kvp in spawningEntities) {
                entities[kvp.Key] = kvp.Value;
            }
            spawningEntities.Clear();
        }
    }
}
