using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityFlinger : EntityGadget {
        static int FLING_DISTANCE = 8;

        public FlingerCondition condition;

        public EntityFlinger(State board, Vector2Int coor) : base(board, coor, "Flinger") {
            subtype = EntitySubtype.Flinger;
        }

        public override void TickThrow() {
            foreach (Vector2Int direction in Util.ALL_DIRECTIONS) {
                Vector2Int sourceCoor = coor + direction;
                if (state.GetTypeAtCoor(sourceCoor) == EntityType.Fruit) {
                    Entity entity = state.entities[sourceCoor];
                    if (condition != null && !condition.Satisfies(entity as EntityFruit)) {
                        continue;
                    }
                    state.ThrowEntity(entity, -direction, FLING_DISTANCE);
                }
            }
        }
    }

    public class FlingerCondition {
        public FlingerConditionType type;
        public FlingerConditionOperation operation;
        public int value;

        public FlingerCondition(FlingerConditionType type, FlingerConditionOperation operation, int value) {
            this.type = type;
            this.operation = operation;
            this.value = value;
        }
        
        public bool Satisfies(EntityFruit fruit) {
            if (type == FlingerConditionType.Mass) {
                if (operation == FlingerConditionOperation.Equals) {
                    return fruit.mass == value;
                }
                if (operation == FlingerConditionOperation.LessThan) {
                    return fruit.mass < value;
                }
                if (operation == FlingerConditionOperation.GreaterThan) {
                    return fruit.mass > value;
                }
            } else if (type == FlingerConditionType.Reactivity) {
                if (operation == FlingerConditionOperation.Equals) {
                    return fruit.reactivity == value;
                }
                if (operation == FlingerConditionOperation.LessThan) {
                    return fruit.reactivity < value;
                }
                if (operation == FlingerConditionOperation.GreaterThan) {
                    return fruit.reactivity > value;
                }
            }
            throw new Exception("Unexpected flinger condition type checked: " + type);
        }
    }
    public enum FlingerConditionType {
        None, Mass, Reactivity
    }
    public enum FlingerConditionOperation {
        Equals, LessThan, GreaterThan
    }
}
