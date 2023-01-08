using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityPedestal : EntityFixture {
        public EntityPedestal(State board, Vector2Int coor) : base(board, coor, "Pedestal") {
            subtype = EntitySubtype.Pedestal;
        }
    }
}
