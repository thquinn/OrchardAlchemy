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

        public Entity(State state, Vector2Int coor, EntityType type) {
            this.state = state;
            this.coor = coor;
            this.type = type;
        }

        public virtual void TickStart() {
            ticksAlive++;
        }
        public virtual void TickConsume() { }
        public virtual void TickThrow() { }
        public virtual void TickSpawn() { }
    }

    public enum EntityType {
        None, Fixture, Fruit, Gadget, Tree
    }
    public enum EntitySubtype {
        None,
        // Gadget subtypes.
        Blocker, Flinger, Fuser, Lab, Storage,
        // Fixture subtypes.
        Market, Pedestal
    }
}
