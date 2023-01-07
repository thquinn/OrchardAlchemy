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

        // GADGET COSTS
        static Dictionary<EntitySubtype, GadgetCost[]> GADGET_COSTS = new Dictionary<EntitySubtype, GadgetCost[]>() {
            { EntitySubtype.Blocker, new GadgetCost[] {
                new GadgetCost(5, 5 * 100, null),
            } },
            { EntitySubtype.Flinger, new GadgetCost[] {
                new GadgetCost(5, 10 * 100, null),
            } },
            { EntitySubtype.Fuser, new GadgetCost[] {
                new GadgetCost(1, 50 * 100, null),
            } },
        };

        // TREES
        static Vector3Int[] TREE_APPLE = new Vector3Int[] {
            new Vector3Int(4, 1, 1),
        };
        static Vector3Int[] TREE_PEAR = new Vector3Int[] {
            new Vector3Int(5, 1, 1),
        };

        // COORDINATES
        static Vector2Int COOR_FIRST_TREE = new Vector2Int(-5, 0);
        static Vector2Int COOR_TUTORIAL_FLINGER_MARKET = new Vector2Int(5, 0);
        static Vector2Int COOR_TUTORIAL_BLOCKER_MARKET = new Vector2Int(0, 8);
        static Vector2Int COOR_TUTORIAL_BLOCKER_POSITION = new Vector2Int(1, 0);
        static Vector2Int COOR_SECOND_TREE = new Vector2Int(5, 0);

        State state;
        public ProgressionPhase phase;
        public ulong maxCents, maxFruit;
        public int walletSizeIndex, fruitWalletSizeIndex;
        public Dictionary<EntitySubtype, GadgetCost> gadgetCosts;
        public Dictionary<EntitySubtype, int> gadgetCostIndex;
        public Vector2Int[] highlightCoors;

        public Progression(State state) {
            this.state = state;
            maxCents = WALLET_SIZES[0];
            maxFruit = FRUIT_WALLET_SIZES[0];
            gadgetCosts = new Dictionary<EntitySubtype, GadgetCost>();

            state.SpawnTree(COOR_FIRST_TREE, TREE_APPLE, new Vector2Int[] { Vector2Int.right });
            state.SpawnEntity(new EntityMarket(state, COOR_TUTORIAL_FLINGER_MARKET));
        }

        public void Tick() {
            if (phase == ProgressionPhase.Start && state.totalCentsEarned >= 300) {
                phase = ProgressionPhase.TutorialFlinger;
                state.StoreGadgetType(EntitySubtype.Flinger);
                highlightCoors = new Vector2Int[] { COOR_FIRST_TREE + Vector2Int.right * 2 };
            }
            if (phase == ProgressionPhase.TutorialFlinger && state.storedGadgets[EntitySubtype.Flinger] == 0 && state.totalCentsEarned >= 600) {
                phase = ProgressionPhase.TutorialBlocker;
                state.RemoveEntityAtCoor(COOR_TUTORIAL_FLINGER_MARKET);
                state.SpawnEntity(new EntityMarket(state, COOR_TUTORIAL_BLOCKER_MARKET));
                state.StoreGadgetType(EntitySubtype.Blocker);
                state.StoreGadgetType(EntitySubtype.Flinger);
                highlightCoors = new Vector2Int[] { COOR_TUTORIAL_BLOCKER_POSITION };
            }
            if (phase == ProgressionPhase.TutorialBlocker && state.totalCentsEarned >= 1000) {
                state.SpawnTree(COOR_SECOND_TREE, TREE_PEAR, new Vector2Int[] { Vector2Int.left });
                phase = ProgressionPhase.SecondTree;
                UnlockPurchase(EntitySubtype.Flinger);
                UnlockPurchase(EntitySubtype.Blocker);
                highlightCoors = null;
            }
            if (phase == ProgressionPhase.SecondTree && state.fruitProcessedCounts.ContainsKey(4) && state.fruitProcessedCounts.ContainsKey(5)) {
                phase = ProgressionPhase.SecondTreeMoney;
                UnlockPurchase(EntitySubtype.Fuser);
            }
            if (phase == ProgressionPhase.SecondTreeMoney && state.cents >= GADGET_COSTS[EntitySubtype.Fuser][0].cents) {
                phase = ProgressionPhase.TutorialFuser;
                state.RemoveEntityAtCoor(COOR_TUTORIAL_BLOCKER_MARKET);
            }
        }
        void UnlockPurchase(EntitySubtype gadgetType) {
            gadgetCosts[gadgetType] = GADGET_COSTS[gadgetType][0];
        }
    }

    public enum ProgressionPhase : int {
        Start = 0,
        TutorialFlinger = 1,
        TutorialBlocker = 2,
        SecondTree = 3,
        SecondTreeMoney = 4,
        TutorialFuser = 5,
    }

    public class GadgetCost {
        public int stockAmount; // How many of this gadget can be bought at this price before it goes up.
        public ulong cents;
        public Vector2Int[] massesAndAmounts;

        public GadgetCost(int stockAmount, ulong cents, params Vector2Int[] massesAndAmounts) {
            this.stockAmount = stockAmount;
            this.cents = cents;
            this.massesAndAmounts = massesAndAmounts ?? new Vector2Int[0];
        }
    }
}
