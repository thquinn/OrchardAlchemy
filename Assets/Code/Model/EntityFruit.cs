using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityFruit : Entity {
        public int mass, reactivity;

        public EntityFruit(State board, Vector2Int coor, int mass, int reactivity) : base(board, coor, EntityType.Fruit) {
            this.mass = mass;
            this.reactivity = reactivity;
        }
    }
}
