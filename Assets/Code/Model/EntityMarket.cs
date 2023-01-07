using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityMarket : EntityFixture {
        List<Entity> presentAtStart;

        public EntityMarket(State board, Vector2Int coor) : base(board, coor, "Market") {
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
                    if (presentAtStart.Contains(fruit)) {
                        state.GetMoney((ulong)GetFruitPrice(fruit));
                        state.RemoveEntity(fruit);
                        state.CountProcessedFruit(fruit);
                    }
                }
            }
        }

        int GetFruitPrice(EntityFruit fruit) {
            return Mathf.FloorToInt(Mathf.Pow(fruit.mass / 4f, 1.1f) * 100);
        }
    }
}
