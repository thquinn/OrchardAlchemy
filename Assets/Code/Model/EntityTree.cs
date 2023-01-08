using System.Linq;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityTree : Entity {
        static int SPAWN_RATE = 4;

        public Vector3Int[] fruitTypesAndWeights;
        public Vector2Int[] directions;

        int spawnTimer;

        public EntityTree(State board,
                          Vector2Int coor,
                          Vector3Int[] fruitTypesAndWeights,
                          Vector2Int[] directions = null
                         ) : base(board, coor, EntityType.Tree) {
            this.fruitTypesAndWeights = fruitTypesAndWeights;
            this.directions = directions ?? Util.ALL_DIRECTIONS.Clone() as Vector2Int[];
            spawnTimer = 1;
        }

        public override void TickSpawn() {
            spawnTimer = Mathf.Max(spawnTimer - 1, 0);
            if (spawnTimer > 0) {
                return;
            }
            Vector2Int spawnLocation = coor + directions[Random.Range(0, directions.Length)];
            if (state.GetTypeAtCoor(spawnLocation) != EntityType.None) {
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
            state.SpawnFruit(spawnLocation, fruitType.x, fruitType.y);
            spawnTimer = SPAWN_RATE;
        }
    }
}
