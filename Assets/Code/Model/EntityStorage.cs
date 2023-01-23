using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityStorage : EntityGadget {
        public EntityStorage(State board, Vector2Int coor) : base(board, coor, "Storage") {
            subtype = EntitySubtype.Storage;
        }

        public override void TickConsume() {
            foreach (Vector2Int direction in Util.ALL_DIRECTIONS) {
                Vector2Int fruitCoor = coor + direction;
                if (state.GetTypeAtCoor(fruitCoor) == EntityType.Fruit) {
                    EntityFruit fruit = state.entities[fruitCoor] as EntityFruit;
                    state.StoreEntity(fruit);
                }
            }
        }
    }
}
