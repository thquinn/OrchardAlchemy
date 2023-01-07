using Assets.Code.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code {
    public static class Util {
        public static string GetFruitNameFromMass(int mass) {
            if (FRUIT_MASS_TO_INFO.ContainsKey(mass)) {
                return FRUIT_MASS_TO_INFO[mass].name;
            }
            return "Fruit " + mass;
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
            } else {
                s = 0;
            }
            return Color.HSVToRGB(h, s, v);
        }

        public static Vector2Int[] ALL_DIRECTIONS = new Vector2Int[] { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        static Dictionary<int, FruitInfo> FRUIT_MASS_TO_INFO = new Dictionary<int, FruitInfo>() {
            { 4, new FruitInfo("Apple", Color.red) },
        };
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
