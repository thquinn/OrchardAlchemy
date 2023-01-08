using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityLab : EntityGadget {
        List<Entity> presentAtStart;

        public EntityLab(State board, Vector2Int coor) : base(board, coor, "Lab") {
            subtype = EntitySubtype.Lab;
            presentAtStart = new List<Entity>();
        }

        public override void TickStart() {
            base.TickStart();
            presentAtStart.Clear();
            foreach (Vector2Int direction in Util.ALL_DIRECTIONS) {
                Vector2Int fruitCoor = coor + direction;
                if (state.GetTypeAtCoor(fruitCoor) == EntityType.Fruit) {
                    presentAtStart.Add(state.entities[fruitCoor]);
                }
            }
        }
        public override void TickConsume() {
            foreach (Vector2Int direction in Util.ALL_DIRECTIONS) {
                Vector2Int fruitCoor = coor + direction;
                if (state.GetTypeAtCoor(fruitCoor) == EntityType.Fruit) {
                    EntityFruit fruit = state.entities[fruitCoor] as EntityFruit;
                    if (presentAtStart.Contains(fruit) && state.progression.CanResearch(fruit)) {
                        state.progression.IncrementResearch(fruit);
                        state.RemoveEntity(fruit);
                        state.CountProcessedFruit(fruit);
                    }
                }
            }
        }
    }
}
