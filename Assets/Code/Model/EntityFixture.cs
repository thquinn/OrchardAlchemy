using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public abstract class EntityFixture : Entity {
        public string name;

        public EntityFixture(State board, Vector2Int coor, string name) : base(board, coor, EntityType.Fixture) {
            this.name = name;
        }
    }
}
