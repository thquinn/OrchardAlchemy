using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityMarket : EntityFixture {
        List<Entity> lastTurnFruit;

        public EntityMarket(State board, Vector2Int coor) : base(board, coor, "Market") {
            lastTurnFruit = new List<Entity>();
        }

        public override void TickConsume() {
            List<Entity> eligibleFruit = null;
            if (lastTurnFruit.Count > 0) {
                eligibleFruit = lastTurnFruit;
                lastTurnFruit = new List<Entity>();
            }
            foreach (Vector2Int direction in Util.ALL_DIRECTIONS) {
                Vector2Int fruitCoor = coor + direction;
                if (state.GetTypeAtCoor(fruitCoor) == EntityType.Fruit) {
                    lastTurnFruit.Add(state.entities[fruitCoor]);
                }
            }
            // Sell.
            if (eligibleFruit != null) {
                var fruitToSell = eligibleFruit.Where(f => lastTurnFruit.Contains(f)).Cast<EntityFruit>();
                foreach (EntityFruit fruit in fruitToSell) {
                    state.GetMoney(GetFruitPrice(fruit));
                    state.ConsumeEntity(fruit);
                }
            }
        }

        int GetFruitPrice(EntityFruit fruit) {
            return Mathf.FloorToInt(Mathf.Pow(fruit.mass / 4, 1.1f) * 100);
        }
    }
}
