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
            directions.Shuffle();
            Vector2Int spawnDirection = Vector2Int.zero;
            foreach (Vector2Int direction in directions) {
                if (state.GetTypeAtCoor(coor + direction) == EntityType.None) {
                    spawnDirection = direction;
                    break;
                }
            }
            if (spawnDirection == Vector2Int.zero) {
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
            state.SpawnFruit(coor + spawnDirection, fruitType.x, fruitType.y);
            spawnTimer = SPAWN_RATE;
        }
    }
}
