using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityBlocker : EntityGadget {
        public EntityBlocker(State board, Vector2Int coor) : base(board, coor, "Blocker") {
            subtype = EntitySubtype.Blocker;
        }
    }
}
