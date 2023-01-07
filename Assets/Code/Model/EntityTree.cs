using System.Linq;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityTree : Entity {
        public Vector3Int[] fruitTypesAndWeights;
        public Vector2Int[] directions;

        public EntityTree(State board,
                          Vector2Int coor,
                          Vector3Int[] fruitTypesAndWeights,
                          Vector2Int[] directions = null
                         ) : base(board, coor, EntityType.Tree) {
            this.fruitTypesAndWeights = fruitTypesAndWeights;
            this.directions = directions ?? Util.ALL_DIRECTIONS;
        }

        public override void Tick() {
            base.Tick();
            if (ticksAlive % 10 != 1) {
                return;
            }
            Vector3Int fruitType = new Vector3Int();
            int fruitTypeSelector = Random.Range(0, fruitTypesAndWeights.Sum(ftw => ftw.z));
            foreach (Vector3Int fruitTypeAndWeight in fruitTypesAndWeights) {
                fruitTypeSelector -= fruitTypeAndWeight.z;
                if (fruitTypeSelector <= 0) {
                    fruitType = fruitTypeAndWeight;
                    break;
                }
            }
            Debug.Assert(fruitType.x != 0);
            Vector2Int direction = directions[Random.Range(0, directions.Length)];
            state.SpawnFruit(coor + direction, fruitType.x, fruitType.y);
        }
    }
}
