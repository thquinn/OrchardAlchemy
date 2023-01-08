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

        public static Vector2Int[] ALL_DIRECTIONS = new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        static Dictionary<int, FruitInfo> FRUIT_MASS_TO_INFO = new Dictionary<int, FruitInfo>() {
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
            { 8, new FruitInfo("Crabapple", HexToColor("#B00000"), 20,
                                "You discovered the Storage gadget! It moves adjacent fruit into your inventory, which you can use to buy gadgets.",
                                (state) => {
                                    state.progression.UnlockPurchase(EntitySubtype.Storage);
                                }) },
            { 9, new FruitInfo("Quince", HexToColor("#FFEF00"), 1,//20,
                                "You discovered Conditional Flingers! Click Flingers to set conditions on them.",
                                (state) => {
                                    state.progression.researchFlags.Add(ResearchFlags.ConditionalFlingers);
                                }) },
            { 10, new FruitInfo("Loquat", HexToColor("#EC983C"), 40,
                                "You discovered temporal secrets! You can make time pass faster.",
                                (state) => {
                                    state.progression.timeScaleMaxIndex++;
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
