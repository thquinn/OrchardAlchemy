﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class State {
        public Dictionary<Vector2Int, Entity> entities;
        List<PendingThrow> pendingThrows;
        Dictionary<Vector2Int, Entity> pendingSpawns;

        public State() {
            entities = new Dictionary<Vector2Int, Entity>();
            pendingSpawns = new Dictionary<Vector2Int, Entity>();
            pendingThrows = new List<PendingThrow>();
            SpawnTree();
            EntityFlinger flinger = new EntityFlinger(this, new Vector2Int(2, 0));
            SpawnEntity(flinger);
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
            if (!entities.ContainsKey(entity.coor) && !pendingSpawns.ContainsKey(entity.coor)) {
                pendingSpawns[entity.coor] = entity;
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
            foreach (Entity entity in entities.Values) {
                entity.Tick();
            }
            foreach (var kvp in pendingSpawns) {
                entities[kvp.Key] = kvp.Value;
            }
            pendingSpawns.Clear();
            PerformPendingThrows();
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
