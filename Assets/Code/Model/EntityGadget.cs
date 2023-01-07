using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public abstract class EntityGadget : Entity {
        public string name;

        public EntityGadget(State board, Vector2Int coor, string name) : base(board, coor, EntityType.Gadget) {
            this.name = name;
        }
    }
}
