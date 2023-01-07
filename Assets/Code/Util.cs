using Assets.Code.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            } else {
                throw new System.Exception("Unknown gadget subtype: " + subtype);
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

        public static Vector2Int[] ALL_DIRECTIONS = new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        static Dictionary<int, FruitInfo> FRUIT_MASS_TO_INFO = new Dictionary<int, FruitInfo>() {
            { 4, new FruitInfo("Apple", Color.red) },
            { 5, new FruitInfo("Pear", HexToColor("#D1E231")) },
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

        public FruitInfo(string name, Color color) {
            this.name = name;
            this.color = color;
        }
    }
}
