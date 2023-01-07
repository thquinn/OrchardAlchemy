using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityFlinger : EntityGadget {
        public EntityFlinger(State board, Vector2Int coor) : base(board, coor, "Flinger") { }

        public override void Tick() {
            base.Tick();
            List<Vector2Int> skipDirs = new List<Vector2Int>();
            foreach (Vector2Int direction in Util.ALL_DIRECTIONS) {
                if (skipDirs.Contains(direction)) {
                    continue;
                }
                Vector2Int sourceCoor = coor + direction;
                Vector2Int destinationCoor = coor - direction;
                if (state.GetTypeAtCoor(sourceCoor) == EntityType.Fruit && state.GetTypeAtCoor(destinationCoor) == EntityType.None) {
                    Entity e = state.entities[sourceCoor];
                    state.entities.Remove(sourceCoor);
                    state.entities.Add(destinationCoor, e);
                    e.coor = destinationCoor;
                    skipDirs.Add(-direction);
                }
            }
        }
    }
}
