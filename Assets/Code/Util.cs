using Assets.Code.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code {
    public static class Util {
        public static EntityGadget GetGadgetInstanceFromSubtype(EntitySubtype subtype) {
            if (subtype == EntitySubtype.Blocker) {
                return new EntityBlocker(null, Vector2Int.zero);
            } else if (subtype == EntitySubtype.Flinger) {
                return new EntityFlinger(null, Vector2Int.zero);
            } else if (subtype == EntitySubtype.Fuser) {
                return new EntityFuser(null, Vector2Int.zero);
            } else if (subtype == EntitySubtype.Lab) {
                return new EntityLab(null, Vector2Int.zero);
            } else if (subtype == EntitySubtype.Storage) {
                return new EntityStorage(null, Vector2Int.zero);
            } else {
                throw new System.Exception("Unknown gadget subtype: " + subtype);
            }
        }

        public static int GetFruitResearchGoalFromMass(int mass) {
            if (FRUIT_MASS_TO_INFO.ContainsKey(mass)) {
                return FRUIT_MASS_TO_INFO[mass].researchGoal;
            }
            return -1;
        }
        public static string GetFruitResearchDescriptionFromMass(int mass) {
            if (FRUIT_MASS_TO_INFO.ContainsKey(mass)) {
                return FRUIT_MASS_TO_INFO[mass].researchDescription;
            }
            return "Unknown research...";
        }
        public static void InvokeResearchActionByMass(int mass, State state) {
            if (FRUIT_MASS_TO_INFO.ContainsKey(mass)) {
                FRUIT_MASS_TO_INFO[mass].researchAction.Invoke(state);
            }
        }
        public static string GetFruitNameFromMass(int mass) {
            if (FRUIT_MASS_TO_INFO.ContainsKey(mass)) {
                return FRUIT_MASS_TO_INFO[mass].name;
            }
            return "???";
        }
        public static Color GetFruitColorFromMass(int mass) {
            if (FRUIT_MASS_TO_INFO.ContainsKey(mass)) {
                Color c = FRUIT_MASS_TO_INFO[mass].color;
                if (c != Color.clear) {
                    return c;
                }
            }
            float h = (mass * .11f) % 1;
            float saturationT = (mass * .081f) % 1;
            float s = Mathf.Lerp(.25f, 1, saturationT);
            float valueT = (mass * .071f) % 1;
            float v = Mathf.Lerp(1, .33f, valueT);
            return Color.HSVToRGB(h, s, v);
        }
        public static Color ShiftColorToEntityType(Color c, EntityType type) {
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            if (type == EntityType.Tree) {
                h = 48f / 360;
            } else if (type == EntityType.Fruit) {
                h = 90f / 360;
            } else if (type == EntityType.Fixture) {
                h = 190f / 360;
            } else {
                s = 0;
            }
            return Color.HSVToRGB(h, s, v);
        }
        public static Color HexToColor(string hex) {
            Color color;
            return ColorUtility.TryParseHtmlString(hex, out color) ? color : Color.clear;
        }

        public static int GetRoundedProgressPercent(float percent) {
            int output = Mathf.CeilToInt(percent * 100);
            if (output == 100 && percent < 1) {
                output--;
            }
            return output;
        }
        static Dictionary<int, bool> PRIME_MEMO = new Dictionary<int, bool>();
        public static bool IsPrimeMemoized(int number) {
            if (PRIME_MEMO.ContainsKey(number)) {
                return PRIME_MEMO[number];
            }
            if (number < 2) {
                PRIME_MEMO[number] = false;
                return false;
            }
            if (number % 2 == 0) {
                PRIME_MEMO[number] = number == 2;
                return number == 2;
            }
            int root = (int) System.Math.Sqrt(number);
            for (int i = 3; i <= root; i += 2) {
                if (number % i == 0) {
                    PRIME_MEMO[number] = false;
                    return false;
                }
            }
            PRIME_MEMO[number] = true;
            return true;
        }

        static Camera cam;
        public static float GetVolume(Vector2Int coor) {
            if (cam == null) {
                cam = Camera.main;
            }
            float xyDistance = new Vector2(coor.x - cam.transform.position.x, coor.y - cam.transform.position.y).magnitude;
            float xyCoverage = cam.orthographicSize * 3;
            float xyFactor = xyCoverage / (xyDistance + xyCoverage);
            float zFactor = 3 / cam.orthographicSize;
            return Mathf.Min(xyFactor * zFactor, 1);
        }

        public static Vector2Int[] ALL_DIRECTIONS = new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        /* These are the base fruits I plan to have in:
         * 1/2, 4/1, 5/1
         * these are the first-order derived fruits:
         * 2/1, 6/0, 8/0, 9/0, 10/0
         * third-order:
         * 3/0, 7/0
         * Super Lemon gets you:
         * 6/1
         * which in turn gets:
         * 11/0, 12/0
         */
        static Dictionary<int, FruitInfo> FRUIT_MASS_TO_INFO = new Dictionary<int, FruitInfo>() {
            { 1, new FruitInfo("Mint", HexToColor("#A2E4B8"), 11,
                               "You discovered the perfect reagent! You get a free Fuser.",
                               (state) => {
                                   state.StoreGadgetType(EntitySubtype.Fuser);
                               }) },
            { 2, new FruitInfo("Doublemint", HexToColor("#5FE88D"), 30,
                               "You discovered the first prime number! Prime-numbered fruits sell for 50% more.",
                               (state) => {
                                   state.progression.researchFlags.Add(ResearchFlags.PrimeBonus);
                               }) },
            { 3, new FruitInfo("Cacao", HexToColor("#411900"), 40,
                               "You learned how to fudge the numbers!",
                               (state) => {
                                   state.progression.researchFlags.Add(ResearchFlags.ApproximateResearch);
                               }) },
            { 4, new FruitInfo("Apple", Color.red, 25,
                               "You discovered a bigger wallet! Your max money is increased by $100.",
                               (state) => {
                                   state.progression.maxCents += 100 * 100;
                               }) },
            { 5, new FruitInfo("Pear", HexToColor("#D1E231"), 10,
                               "You discovered a <i>pair</i> of $50 bills... that's $100!",
                               (state) => {
                                   state.GetMoney(100 * 100, false);
                               }) },
            { 6, new FruitInfo("Lemon", HexToColor("#FDFF00"), 5,
                               "You discovered a new energy source! Your next research starts at 50%.",
                               (state) => {
                                   state.progression.researchStartBonus = true;
                               }) },
            { 7, new FruitInfo("Miracle Berry", HexToColor("#B00000"), 50,
                               "You discovered a new way to taste! Lemons get +1 reactivity.",
                               (state) => {
                                   state.progression.researchFlags.Add(ResearchFlags.SuperLemon);
                               }) },
            { 8, new FruitInfo("Crabapple", HexToColor("#B00000"), 25,
                               "You discovered the Storage gadget! It moves adjacent fruit into your inventory, which you can use to buy gadgets.",
                               (state) => {
                                   state.progression.UnlockPurchase(EntitySubtype.Storage);
                               }) },
            { 9, new FruitInfo("Quince", HexToColor("#FFEF00"), 20,
                               "You discovered Conditional Flingers! Click Flingers to set conditions on them.",
                               (state) => {
                                   state.progression.researchFlags.Add(ResearchFlags.ConditionalFlingers);
                               }) },
            { 10, new FruitInfo("Loquat", HexToColor("#EC983C"), 40,
                                "You discovered temporal secrets! You can make time pass faster.",
                                (state) => {
                                    state.progression.timeScaleMaxIndex = Mathf.Min(state.progression.timeScaleMaxIndex + 1, Progression.TIMESCALES.Length - 1);
                                }) },
            { 11, new FruitInfo("Raspberry", HexToColor("#E30B5C"), 33,
                                "You discovered half of a victory...",
                                (state) => {
                                    state.SpawnHalfVictory();
                                }) },
            { 12, new FruitInfo("Lime", HexToColor("#32CD32"), 16,
                                "You discovered half of a victory...",
                                (state) => {
                                    state.SpawnHalfVictory();
                                }) },
        };
    }

    public static class ArrayExtensions {
        public static T[] Shuffle<T>(this T[] array) {
            int n = array.Length;
            for (int i = 0; i < n; i++) {
                int r = i + Random.Range(0, n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
            return array;
        }
    }

    public struct FruitInfo {
        public string name;
        public Color color;
        public int researchGoal;
        public string researchDescription;
        public System.Action<State> researchAction;

        public FruitInfo(string name, Color color, int researchGoal, string researchDescription, System.Action<State> researchAction) {
            this.name = name;
            this.color = color;
            this.researchGoal = researchGoal;
            this.researchDescription = researchDescription;
            this.researchAction = researchAction;
        }
    }
}
