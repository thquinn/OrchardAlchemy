using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Model {
    public abstract class Entity {
        public State state;
        public Vector2Int coor;
        public EntityType type;
        public EntitySubtype subtype;
        public int ticksAlive;

        public Entity(State board, Vector2Int coor, EntityType type) {
            this.state = board;
            this.coor = coor;
            this.type = type;
        }

        public virtual void Tick() {
            ticksAlive++;
        }
    }

    public enum EntityType {
        None, Fixture, Fruit, Gadget, Tree
    }
    public enum EntitySubtype {
        None, Blocker
    }
}
