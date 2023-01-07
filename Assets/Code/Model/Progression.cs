using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class Progression {
        static ulong[] WALLET_SIZES = new ulong[] { 100*100, 500*100, 2500*100, 10000*100 };
        static ulong[] FRUIT_WALLET_SIZES = new ulong[] { 10, 50, 250 };

        // TREES
        public static Vector3Int[] TREE_APPLE = new Vector3Int[] {
            new Vector3Int(4, 1, 1),
        };

        State state;
        public ulong maxCents, maxFruit;
        public int walletSizeIndex, fruitWalletSizeIndex;

        public Progression(State state) {
            this.state = state;
            maxCents = WALLET_SIZES[0];
            maxFruit = FRUIT_WALLET_SIZES[0];

            state.SpawnTree(new Vector2Int(-5, 0), TREE_APPLE, new Vector2Int[] { Vector2Int.right });
            state.SpawnEntity(new EntityMarket(state, new Vector2Int(5, 0)));
        }

        public void Tick() {

        }
    }

    public class GadgetCost {

    }
}
