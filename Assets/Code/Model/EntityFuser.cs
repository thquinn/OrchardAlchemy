using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityFuser : EntityGadget {
        int spawnMass, spawnReactivity;
        Vector2Int spawnCoor;

        public EntityFuser(State board, Vector2Int coor) : base(board, coor, "Fuser") {
            subtype = EntitySubtype.Fuser;
        }

        public override void TickConsume() {
            EntityType[] directionTypes = Util.ALL_DIRECTIONS.Select(d => state.GetTypeAtCoor(coor + d)).ToArray();
            for (int i = 0; i < 4; i++) {
                if (directionTypes[i] == EntityType.Fruit &&
                    directionTypes[(i + 1) % 4] == EntityType.Fruit &&
                    directionTypes[(i + 2) % 4] == EntityType.None) {
                    EntityFruit source1 = state.entities[coor + Util.ALL_DIRECTIONS[i]] as EntityFruit;
                    EntityFruit source2 = state.entities[coor + Util.ALL_DIRECTIONS[(i + 1) % 4]] as EntityFruit;
                    if (source1.reactivity > 0 && source2.reactivity > 0) {
                        state.RemoveEntity(source1);
                        state.RemoveEntity(source2);
                        spawnMass = source1.mass + source2.mass;
                        spawnReactivity = Mathf.Min(source1.reactivity, source2.reactivity) - 1;
                        spawnCoor = coor + Util.ALL_DIRECTIONS[(i + 2) % 4];
                        return;
                    }
                }
            }
        }
        public override void TickSpawn() {
            if (spawnMass > 0) {
                state.SpawnFruit(spawnCoor, spawnMass, spawnReactivity);
                spawnMass = 0;
            }
        }
    }
}
