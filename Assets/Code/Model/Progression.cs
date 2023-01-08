using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class Progression {
        public static float[] TIMESCALES = new float[] { .25f, .5f, 1, 1.5f, 2, 2.5f, 3 };

        // GADGET COSTS
        static Dictionary<EntitySubtype, GadgetCost[]> GADGET_COSTS = new Dictionary<EntitySubtype, GadgetCost[]>() {
            { EntitySubtype.Blocker, new GadgetCost[] {
                new GadgetCost(999, 5 * 100, null),
            } },
            { EntitySubtype.Flinger, new GadgetCost[] {
                new GadgetCost(10, 10 * 100, null),
                new GadgetCost(20, 10 * 100, new Vector2Int[] { new Vector2Int(9, 1) }),
                new GadgetCost(999, 20 * 100, new Vector2Int[] { new Vector2Int(9, 2) }),
            } },
            { EntitySubtype.Fuser, new GadgetCost[] {
                new GadgetCost(2, 30 * 100, null),
                new GadgetCost(4, 40 * 100, null),
                new GadgetCost(6, 50 * 100, null),
                new GadgetCost(999, 50 * 100, new Vector2Int[] { new Vector2Int(3, 1) }),
            } },
            { EntitySubtype.Lab, new GadgetCost[] {
                new GadgetCost(1, 50 * 100, null),
                new GadgetCost(1, 100 * 100, null),
                new GadgetCost(999, 200 * 100, new Vector2Int[] { new Vector2Int(7, 1) }),
            } },
            { EntitySubtype.Storage, new GadgetCost[] {
                new GadgetCost(5, 20 * 100, null),
                new GadgetCost(5, 40 * 100, null),
                new GadgetCost(5, 80 * 100, null),
                new GadgetCost(999, 160 * 100, new Vector2Int[] { new Vector2Int(8, 1) }),
            } },
        };

        // TREES
        static Vector3Int[] TREE_MINT = new Vector3Int[] {
            new Vector3Int(1, 2, 1),
        };
        static Vector3Int[] TREE_APPLE = new Vector3Int[] {
            new Vector3Int(4, 1, 1),
        };
        static Vector3Int[] TREE_PEAR = new Vector3Int[] {
            new Vector3Int(5, 1, 1),
        };
        static Vector3Int[] TREE_MINT_APPLE = new Vector3Int[] {
            new Vector3Int(1, 1, 1),
            new Vector3Int(4, 1, 1),
        };
        static Vector3Int[] TREE_MINT_PEAR = new Vector3Int[] {
            new Vector3Int(1, 1, 1),
            new Vector3Int(5, 1, 1),
        };
        static Vector3Int[] TREE_APPLE_PEAR = new Vector3Int[] {
            new Vector3Int(4, 1, 1),
            new Vector3Int(5, 1, 1),
        };
        static Vector3Int[][] ALL_TREES = new Vector3Int[][] { TREE_MINT, TREE_APPLE, TREE_PEAR, TREE_MINT_APPLE, TREE_MINT_PEAR, TREE_APPLE_PEAR };

        // COORDINATES
        static Vector2Int COOR_FIRST_TREE = new Vector2Int(-5, 0);
        static Vector2Int COOR_TUTORIAL_FLINGER_MARKET = new Vector2Int(5, 0);
        static Vector2Int COOR_TUTORIAL_BLOCKER_MARKET = new Vector2Int(0, 8);
        static Vector2Int COOR_TUTORIAL_BLOCKER_POSITION = new Vector2Int(1, 0);
        static Vector2Int COOR_SECOND_TREE = new Vector2Int(5, 0);
        static Vector2Int COOR_TUTORIAL_FUSER_MARKET = new Vector2Int(-5, 8);
        static Vector2Int COOR_THIRD_TREE = new Vector2Int(5, 5);
        static Vector2Int COOR_FOURTH_TREE = new Vector2Int(1, -5);

        State state;
        public ProgressionPhase phase;
        public ulong maxCents, maxFruit;
        public int walletSizeIndex, fruitWalletSizeIndex;
        public Dictionary<EntitySubtype, GadgetCost> gadgetCosts;
        public Dictionary<EntitySubtype, int> gadgetCostIndex;
        public Vector2Int[] highlightCoors;
        public bool cameraTakeover;
        public Vector3 cameraTargetPosition;
        public int cameraTargetSize;
        public Research research;
        public HashSet<int> fruitsResearched;
        public HashSet<ResearchFlags> researchFlags;
        public int timeScaleMinIndex, timeScaleMaxIndex;
        public bool researchStartBonus;

        public Progression(State state) {
            this.state = state;
            maxCents = 100 * 100;
            maxFruit = 10;
            gadgetCosts = new Dictionary<EntitySubtype, GadgetCost>();
            gadgetCostIndex = new Dictionary<EntitySubtype, int>();
            fruitsResearched = new HashSet<int>();
            researchFlags = new HashSet<ResearchFlags>();
            timeScaleMinIndex = Array.IndexOf(TIMESCALES, 1);
            timeScaleMaxIndex = timeScaleMinIndex;

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
                state.CleanUpFruits();
                state.SpawnEntity(new EntityMarket(state, COOR_TUTORIAL_BLOCKER_MARKET));
                state.StoreGadgetType(EntitySubtype.Blocker);
                state.StoreGadgetType(EntitySubtype.Flinger);
                highlightCoors = new Vector2Int[] { COOR_TUTORIAL_BLOCKER_POSITION };
                cameraTakeover = true;
                cameraTargetPosition = new Vector3(0, 3, 0);
                cameraTargetSize = 7;
            }
            if (phase == ProgressionPhase.TutorialBlocker && state.totalCentsEarned >= 1000) {
                state.CleanUpFruits();
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
                state.CleanUpFruits();
                state.CleanUpGadgets();
                state.SpawnEntity(new EntityMarket(state, COOR_TUTORIAL_FUSER_MARKET));
                cameraTakeover = true;
                cameraTargetPosition = new Vector3(0, 4, 0);
                cameraTargetSize = 7;
            }
            if (phase == ProgressionPhase.TutorialFuser && state.fruitProcessedCounts.ContainsKey(9)) {
                phase = ProgressionPhase.FuserMoney;
                state.SpawnTree(COOR_THIRD_TREE, TREE_APPLE, null);
                UnlockPurchase(EntitySubtype.Lab);
            }
            if (phase == ProgressionPhase.FuserMoney && state.cents >= GADGET_COSTS[EntitySubtype.Lab][0].cents) {
                phase = ProgressionPhase.TutorialResearch;
            }
            if (phase == ProgressionPhase.TutorialResearch && fruitsResearched.Contains(9)) {
                phase = ProgressionPhase.TutorialResearchAgain;
            }
            if (phase == ProgressionPhase.TutorialResearchAgain && fruitsResearched.Count >= 2) {
                phase = ProgressionPhase.TutorialOver;
                state.SpawnTree(COOR_FOURTH_TREE, TREE_APPLE_PEAR, null);
            }
            if (phase == ProgressionPhase.TutorialOver) {
                EndlessSpawning();
            }
        }
        public void UnlockPurchase(EntitySubtype gadgetType) {
            gadgetCosts[gadgetType] = GADGET_COSTS[gadgetType][0];
            gadgetCostIndex[gadgetType] = 0;
        }

        public void IncrementCost(EntitySubtype gadgetType) {
            GadgetCost cost = gadgetCosts[gadgetType];
            cost.stockAmount--;
            if (cost.stockAmount <= 0 && gadgetCostIndex[gadgetType] < GADGET_COSTS[gadgetType].Length - 1) {
                gadgetCostIndex[gadgetType]++;
                gadgetCosts[gadgetType] = GADGET_COSTS[gadgetType][gadgetCostIndex[gadgetType]];
            }
        }

        public bool CanResearch(EntityFruit fruit) {
            if (research != null && researchFlags.Contains(ResearchFlags.ApproximateResearch) && Mathf.Abs(research.mass - fruit.mass) <= 1) {
                return true;
            }
            if (fruitsResearched.Contains(fruit.mass)) {
                return false;
            }
            if (Util.GetFruitResearchGoalFromMass(fruit.mass) == -1) {
                return false;
            }
            return research == null || research.mass == fruit.mass;
        }
        public void IncrementResearch(EntityFruit fruit) {
            if (research == null) {
                research = new Research(fruit.mass, Util.GetFruitResearchGoalFromMass(fruit.mass));
                if (researchStartBonus) {
                    research.progress = Mathf.CeilToInt(research.goal / 2f);
                    researchStartBonus = false;
                }
            }
            research.progress++;
            if (research.progress >= research.goal) {
                fruitsResearched.Add(research.mass);
                Util.InvokeResearchActionByMass(research.mass, state);
                research = null;
            }
        }

        static ulong EPOCH_CENTS = 100 * 100;
        ulong lastTotalCentsEarned;
        int totalEpochs;
        void EndlessSpawning() {
            if (lastTotalCentsEarned == 0) {
                lastTotalCentsEarned = state.totalCentsEarned;
                return;
            }
            ulong lastEpoch = lastTotalCentsEarned / EPOCH_CENTS;
            lastTotalCentsEarned = state.totalCentsEarned;
            ulong thisEpoch = lastTotalCentsEarned / EPOCH_CENTS;
            while (lastEpoch < thisEpoch) {
                thisEpoch++;
                SpawnEpoch();
            }
        }
        public void SpawnEpoch() {
            totalEpochs++;
            float radius = 8 + Mathf.Sqrt(totalEpochs) * 5;
            float theta = UnityEngine.Random.value * Mathf.PI * 2;
            Vector2Int coor = new Vector2Int(
                Mathf.RoundToInt(radius * Mathf.Cos(theta)),
                Mathf.RoundToInt(radius * Mathf.Sin(theta))
            );
            if (totalEpochs % 7 == 1) {
                state.SpawnEntity(new EntityMarket(state, coor));
            } else {
                Vector3Int[] tree = ALL_TREES[UnityEngine.Random.Range(0, ALL_TREES.Length)];
                Vector2Int[] directions = (Vector2Int[])Util.ALL_DIRECTIONS.Clone();
                directions.Shuffle();
                directions = directions.Take(UnityEngine.Random.Range(1, 5)).ToArray();
                state.SpawnTree(coor, tree, directions);
            }
        }
    }

    public enum ProgressionPhase : int {
        Start = 0,
        TutorialFlinger = 1,
        TutorialBlocker = 2,
        SecondTree = 3,
        SecondTreeMoney = 4,
        TutorialFuser = 5,
        FuserMoney = 6,
        TutorialResearch = 7,
        TutorialResearchAgain = 8,
        TutorialOver = 9,
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

    public class Research {
        public int mass, progress, goal;

        public Research(int mass, int goal) {
            this.mass = mass;
            this.progress = 0;
            this.goal = goal;
        }
    }
    public enum ResearchFlags {
        ApproximateResearch,
        ConditionalFlingers,
        PrimeBonus,
        SuperLemon,
    }
}
