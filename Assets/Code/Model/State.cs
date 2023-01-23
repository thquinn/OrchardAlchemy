using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class State {
        public Dictionary<Vector2Int, Entity> entities;
        public ulong cents, totalCentsEarned;
        public Dictionary<int, int> fruitProcessedCounts;
        public Dictionary<int, ulong> storedFruit;
        public Dictionary<EntitySubtype, int> storedGadgets;
        public Progression progression;
        List<PendingThrow> pendingThrows;
        float flingVolume;

        public State() {
            entities = new Dictionary<Vector2Int, Entity>();
            fruitProcessedCounts = new Dictionary<int, int>();
            storedFruit = new Dictionary<int, ulong>();
            storedGadgets = new Dictionary<EntitySubtype, int>();
            progression = new Progression(this);
            pendingThrows = new List<PendingThrow>();
        }

        public EntityType GetTypeAtCoor(Vector2Int coor) {
            return entities.ContainsKey(coor) ? entities[coor].type : EntityType.None;
        }
        public EntitySubtype GetSubtypeAtCoor(Vector2Int coor) {
            return entities.ContainsKey(coor) ? entities[coor].subtype : EntitySubtype.None;
        }

        public void ThrowEntity(Entity entity, Vector2Int direction, int distance) {
            pendingThrows.Add(new PendingThrow(entity, direction, distance));
        }

        public void SpawnEntity(Entity entity) {
            entity.coor = GetNearestOpenCoor(entity.coor);
            entities[entity.coor] = entity;
            entity.state = this;
        }
        public void SpawnTree(Vector2Int coor, Vector3Int[] fruitTypesAndWeights, Vector2Int[] directions) {
            EntityTree tree = new EntityTree(this, coor, fruitTypesAndWeights, directions);
            SpawnEntity(tree);
        }
        public void SpawnFruit(Vector2Int coor, int mass, int reactivity) {
            if (mass == 6 && progression.researchFlags.Contains(ResearchFlags.SuperLemon)) {
                reactivity++;
            }
            EntityFruit fruit = new EntityFruit(this, coor, mass, reactivity);
            SpawnEntity(fruit);
        }
        public void SpawnHalfVictory() {
            var pedestalKVP = entities.FirstOrDefault(kvp => kvp.Value.subtype == EntitySubtype.Pedestal);
            if (pedestalKVP.Value == null) {
                SpawnEntity(new EntityPedestal(this, Vector2Int.zero));
            } else {
                RemoveEntityAtCoor(pedestalKVP.Key);
                SpawnEntity(new EntityVictoryStatue(this, pedestalKVP.Key));
                progression.cameraTakeover = true;
                progression.cameraTargetPosition = new Vector3(pedestalKVP.Key.x, pedestalKVP.Key.y, 0);
            }
        }
        static Vector2Int[] SPAWN_SEARCH_DIAGONALS = new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(1, 1) };
        Vector2Int GetNearestOpenCoor(Vector2Int coor) {
            if (!entities.ContainsKey(coor)) {
                return coor;
            }
            int numSteps = 1;
            while (true) {
                coor += Vector2Int.right;
                foreach (Vector2Int direction in SPAWN_SEARCH_DIAGONALS) {
                    for (int i = 0; i < numSteps; i++) {
                        if (!entities.ContainsKey(coor)) {
                            return coor;
                        }
                        coor += direction;
                    }
                }
                numSteps++;
            }
        }

        public void RemoveEntity(Entity entity) {
            entity.state = null;
            entities.Remove(entity.coor);
        }
        public void RemoveEntityAtCoor(Vector2Int coor) {
            if (entities.ContainsKey(coor)) {
                RemoveEntity(entities[coor]);
            }
        }

        public void GetMoney(ulong gained, bool clamp = true) {
            totalCentsEarned += gained;
            if (clamp && cents + gained > progression.maxCents) {
                gained = progression.maxCents - cents;
            }
            cents += gained;
        }
        public void StoreEntity(Entity entity) {
            if (entity.type == EntityType.Fruit) {
                EntityFruit fruit = entity as EntityFruit;
                int mass = fruit.mass;
                if (!storedFruit.ContainsKey(mass)) {
                    storedFruit[mass] = 1;
                } else {
                    storedFruit[mass]++;
                }
            } else if (entity.type == EntityType.Gadget) {
                StoreGadgetType(entity.subtype);
            } else {
                throw new Exception("Trying to store unstorable entity of type: " + entity.type);
            }
            if (entity.state != null) {
                RemoveEntity(entity);
            }
        }
        public void StoreGadgetType(EntitySubtype subtype) {
            if (!storedGadgets.ContainsKey(subtype)) {
                storedGadgets[subtype] = 1;
            } else {
                storedGadgets[subtype]++;
            }
        }
        public int GetStoredGadgetCount(EntitySubtype subtype) {
            return storedGadgets.ContainsKey(subtype) ? storedGadgets[subtype] : 0;
        }
        public void UnstoreEntity(Entity entity) {
            if (entity.type == EntityType.Fruit) {
                int mass = (entity as EntityFruit).mass;
                storedFruit[mass]--;
                if (storedFruit[mass] == 0) {
                    storedFruit.Remove(mass);
                }
            } else if (entity.type == EntityType.Gadget) {
                storedGadgets[entity.subtype]--;
            }
        }
        public void CleanUpFruits() {
            List<Vector2Int> coors = new List<Vector2Int>(entities.Keys);
            foreach (Vector2Int coor in coors) {
                if (entities[coor].type == EntityType.Fruit) {
                    StoreEntity(entities[coor]);
                }
            }
        }
        public void CleanUpGadgets() {
            List<Vector2Int> coors = new List<Vector2Int>(entities.Keys);
            foreach (Vector2Int coor in coors) {
                if (entities[coor].type == EntityType.Gadget) {
                    StoreEntity(entities[coor]);
                }
            }
        }
        public void CountProcessedFruit(EntityFruit fruit) {
            if (!fruitProcessedCounts.ContainsKey(fruit.mass)) {
                fruitProcessedCounts[fruit.mass] = 1;
            } else {
                fruitProcessedCounts[fruit.mass]++;
            }
        }

        public bool CheckAndPayCost(GadgetCost cost) {
            if (cost.cents > cents) {
                return false;
            }
            foreach (Vector2Int massAndAmount in cost.massesAndAmounts) {
                if (!storedFruit.ContainsKey(massAndAmount.x)) {
                    return false;
                }
                if (storedFruit[massAndAmount.x] < (ulong) massAndAmount.y) {
                    return false;
                }
            }
            // Pay!
            cents -= cost.cents;
            foreach (Vector2Int massAndAmount in cost.massesAndAmounts) {
                storedFruit[massAndAmount.x] -= (ulong) massAndAmount.y;
            }
            return true;
        }

        public void Tick() {
            List<Entity> entitiesThisTurn = new List<Entity>(entities.Values);
            foreach (Entity entity in entitiesThisTurn) {
                entity.TickStart();
            }
            foreach (Entity entity in entitiesThisTurn) {
                entity.TickConsume();
            }
            foreach (Entity entity in entitiesThisTurn) {
                entity.TickThrow();
            }
            flingVolume = 0;
            PerformPendingThrows();
            SFXScript.instance.SFXFling(flingVolume * Mathf.Lerp(.8f, 1, UnityEngine.Random.value));
            foreach (Entity entity in entitiesThisTurn) {
                entity.TickSpawn();
            }
            progression.Tick();
        }
        void PerformPendingThrows() {
            // Sort throws, giving priority to right -> up -> left -> down.
            pendingThrows = pendingThrows.OrderBy(pt => OrderDirections(pt.direction)).ToList();
            // Index by entity, removing attempts to throw the same entity multiple times.
            Dictionary<Entity, PendingThrow> throwByEntity = new Dictionary<Entity, PendingThrow>();
            foreach (PendingThrow pendingThrow in pendingThrows) {
                if (!throwByEntity.ContainsKey(pendingThrow.entity)) {
                    throwByEntity[pendingThrow.entity] = pendingThrow;
                }
            }
            // Roll back throws that pass through blockers.
            // "Roll back" in this case means to reduce the distance until there are no conflicts.
            Dictionary<Vector2Int, List<PendingThrow>> throwByCoor = new Dictionary<Vector2Int, List<PendingThrow>>();
            foreach (PendingThrow pendingThrow in throwByEntity.Values) {
                Vector2Int coor = pendingThrow.entity.coor;
                for (int i = 0; i < pendingThrow.distance; i++) {
                    coor += pendingThrow.direction;
                    if (GetSubtypeAtCoor(coor) == EntitySubtype.Blocker) {
                        pendingThrow.distance = i;
                        break;
                    }
                    if (!throwByCoor.ContainsKey(coor)) {
                        throwByCoor[coor] = new List<PendingThrow>();
                    }
                    throwByCoor[coor].Add(pendingThrow);
                }
            }
            // Roll back throws that pass through each other in opposite directions.
            foreach (var kvp in throwByCoor) {
                // Ignore tiles that only one throw is passing through.
                if (kvp.Value.Count == 1) {
                    continue;
                }
                // Horizontal collisions.
                var rightThrows = kvp.Value.Where(pt => pt.direction == Vector2Int.right);
                var leftThrows = kvp.Value.Where(pt => pt.direction == Vector2Int.left);
                if (rightThrows.Any() && leftThrows.Any()) {
                    int maxRightX = rightThrows.Max(t => t.entity.coor.x);
                    int minLeftX = leftThrows.Min(t => t.entity.coor.x);
                    int rightLandingX = (maxRightX + minLeftX) / 2;
                    int leftLandingX = rightLandingX + 1;
                    foreach (PendingThrow rightThrow in rightThrows) {
                        rightThrow.distance = Mathf.Min(rightThrow.distance, rightLandingX - rightThrow.entity.coor.x);
                    }
                    foreach (PendingThrow leftThrow in leftThrows) {
                        leftThrow.distance = Mathf.Min(leftThrow.distance, leftThrow.entity.coor.x - leftLandingX);
                    }
                }
                // Vertical collisions.
                var upThrows = kvp.Value.Where(pt => pt.direction == Vector2Int.up);
                var downThrows = kvp.Value.Where(pt => pt.direction == Vector2Int.down);
                if (upThrows.Any() && downThrows.Any()) {
                    int maxUpY = upThrows.Max(t => t.entity.coor.y);
                    int minDownY = downThrows.Min(t => t.entity.coor.y);
                    int upLandingY = (maxUpY + minDownY) / 2;
                    int downLandingY = upLandingY + 1;
                    foreach (PendingThrow upThrow in upThrows) {
                        upThrow.distance = Mathf.Min(upThrow.distance, upLandingY - upThrow.entity.coor.y);
                    }
                    foreach (PendingThrow downThrow in downThrows) {
                        downThrow.distance = Mathf.Min(downThrow.distance, downThrow.entity.coor.y - downLandingY);
                    }
                }
            }
            // Execute the throws, rolling back if an entity is encountered.
            // Prioritize by directions, then by distance (shortest throw gets priority).
            var sortedThrows = throwByEntity.Values.OrderBy(pt => OrderDirections(pt.direction)).ThenBy(pt => pt.distance);
            foreach (PendingThrow pendingThrow in sortedThrows) {
                Entity entity = pendingThrow.entity;
                Vector2Int destination = entity.coor + pendingThrow.direction * pendingThrow.distance;
                for (int i = 0; entities.ContainsKey(destination) && i < pendingThrow.distance; i++) {
                    destination -= pendingThrow.direction;
                }
                if (destination != entity.coor) {
                    flingVolume += (1 - flingVolume) * Util.GetVolume(entity.coor);
                    entities.Remove(entity.coor);
                    entities[destination] = entity;
                    entity.coor = destination;
                }
            }
            // Clear.
            pendingThrows.Clear();
        }
        static int OrderDirections(Vector2Int direction) {
            if (direction == Vector2Int.right) return 1;
            if (direction == Vector2Int.up) return 2;
            if (direction == Vector2Int.left) return 3;
            if (direction == Vector2Int.down) return 4;
            throw new Exception("Pending throw had a non-unit direction: " + direction);
        }
    }

    public class PendingThrow {
        public Entity entity;
        public Vector2Int direction;
        public int distance;

        public PendingThrow(Entity entity, Vector2Int direction, int distance) {
            this.entity = entity;
            this.direction = direction;
            this.distance = distance;
        }
    }
}
