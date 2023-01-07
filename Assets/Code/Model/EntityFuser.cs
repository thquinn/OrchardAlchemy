using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityFuser : EntityGadget {
        List<Entity> presentAtStart;

        public EntityFuser(State board, Vector2Int coor) : base(board, coor, "Fuser") {
            subtype = EntitySubtype.Fuser;
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
        public override void TickSpawn() {
            EntityType[] directionTypes = Util.ALL_DIRECTIONS.Select(d => state.GetTypeAtCoor(coor + d)).ToArray();
            for (int i = 0; i < 4; i++) {
                if (directionTypes[i] == EntityType.Fruit &&
                    directionTypes[(i + 1) % 4] == EntityType.Fruit &&
                    directionTypes[(i + 2) % 4] == EntityType.None) {
                    EntityFruit source1 = state.entities[coor + Util.ALL_DIRECTIONS[i]] as EntityFruit;
                    EntityFruit source2 = state.entities[coor + Util.ALL_DIRECTIONS[(i + 1) % 4]] as EntityFruit;
                    if (source1.reactivity > 0 && source2.reactivity > 0 && presentAtStart.Contains(source1) && presentAtStart.Contains(source2)) {
                        state.RemoveEntity(source1);
                        state.RemoveEntity(source2);
                        state.SpawnFruit(coor + Util.ALL_DIRECTIONS[(i + 2) % 4], source1.mass + source2.mass, Mathf.Min(source1.reactivity, source2.reactivity) - 1);
                        return;
                    }
                }
            }
        }
    }
}
