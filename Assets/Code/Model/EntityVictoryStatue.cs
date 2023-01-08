using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public class EntityVictoryStatue : EntityFixture {
        public EntityVictoryStatue(State board, Vector2Int coor) : base(board, coor, "Victory Statue") { }
    }
}
