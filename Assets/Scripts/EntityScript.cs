using Assets.Code;
using Assets.Code.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EntityScript : MonoBehaviour
{
    public GameObject prefabReactivityNotch;
    public Sprite[] icons;

    public SpriteRenderer rendererTop, rendererSide, rendererShadow, rendererWoodGrain, rendererGradient, rendererIcon;
    public TextMeshPro tmpName, tmpMass;
    public GameObject directionArrows, reactivityAnchor;
    public SpriteRenderer rendererArrowRight, rendererArrowUp, rendererArrowLeft, rendererArrowDown;

    EntityScriptHandler handler;

    public void Init(Entity entity) {
        transform.localPosition = new Vector3(entity.coor.x, entity.coor.y, 0);
        rendererTop.color = Util.ShiftColorToEntityType(rendererTop.color, entity.type);
        rendererSide.color = Util.ShiftColorToEntityType(rendererSide.color, entity.type);
        tmpName.color = Util.ShiftColorToEntityType(tmpName.color, entity.type);
        rendererIcon.color = Util.ShiftColorToEntityType(rendererIcon.color, entity.type);
        if (entity.type == EntityType.Tree) {
            handler = new EntityTreeHandler(this, entity as EntityTree);
        } else if (entity.type == EntityType.Fruit) {
            handler = new EntityFruitHandler(this, entity as EntityFruit);
        } else if (entity.type == EntityType.Gadget) {
            handler = new EntityGadgetHandler(this, entity as EntityGadget);
        } else if (entity.type == EntityType.Fixture) {
            handler = new EntityFixtureHandler(this, entity as EntityFixture);
        }
    }

    void Update() {
        handler.Update();
    }
    public void DestroyReactivityNotch() {
        Destroy(reactivityAnchor.transform.GetChild(0).gameObject);
    }
    public void AddReactivityNotch() {
        Instantiate(prefabReactivityNotch, reactivityAnchor.transform);
    }
}

public abstract class EntityScriptHandler {
    protected EntityScript script;

    public EntityScriptHandler(EntityScript script) {
        this.script = script;
    }
    public virtual void Update() { }
}

public class EntityTreeHandler : EntityScriptHandler {
    EntityTree tree;

    public EntityTreeHandler(EntityScript script, EntityTree tree) : base(script) {
        this.tree = tree;
        script.rendererWoodGrain.gameObject.SetActive(true);
        script.tmpName.text = tree.fruitTypesAndWeights.Length == 1 ? Util.GetFruitNameFromMass(tree.fruitTypesAndWeights[0].x) + " Tree" : "Mixed Tree";
        script.directionArrows.SetActive(true);
        if (Array.Exists(tree.directions, d => d == Vector2Int.right)) {
            script.rendererArrowRight.gameObject.SetActive(true);
        }
        if (Array.Exists(tree.directions, d => d == Vector2Int.up)) {
            script.rendererArrowUp.gameObject.SetActive(true);
        }
        if (Array.Exists(tree.directions, d => d == Vector2Int.left)) {
            script.rendererArrowLeft.gameObject.SetActive(true);
        }
        if (Array.Exists(tree.directions, d => d == Vector2Int.down)) {
            script.rendererArrowDown.gameObject.SetActive(true);
        }
    }
}

public class EntityFruitHandler : EntityScriptHandler {
    EntityFruit fruit;
    Vector3 velocity;

    public EntityFruitHandler(EntityScript script, EntityFruit fruit) : base(script) {
        this.fruit = fruit;
        script.rendererGradient.gameObject.SetActive(true);
        Color gradientColor = Util.GetFruitColorFromMass(fruit.mass);
        gradientColor.a = .25f;
        script.rendererGradient.color = gradientColor;
        script.tmpName.text = Util.GetFruitNameFromMass(fruit.mass);
        script.tmpMass.gameObject.SetActive(true);
        script.tmpMass.text = fruit.mass.ToString();
        script.tmpMass.color = Util.ShiftColorToEntityType(script.tmpMass.color, EntityType.Fruit);
    }

    public override void Update() {
        if (fruit.state == null) {
            return;
        }
        Vector3 newPosition = Vector3.SmoothDamp(script.transform.localPosition, new Vector3(fruit.coor.x, fruit.coor.y), ref velocity, .1f);
        newPosition.z = new Vector2(velocity.x, velocity.y).sqrMagnitude * -.01f;
        script.transform.localPosition = newPosition;
        // Reactivity.
        if (fruit.reactivity != script.reactivityAnchor.transform.childCount) {
            while (fruit.reactivity < script.reactivityAnchor.transform.childCount) {
                script.DestroyReactivityNotch();
            }
            while (fruit.reactivity > script.reactivityAnchor.transform.childCount) {
                script.AddReactivityNotch();
            }
            float midIndex = fruit.reactivity / 2f - .5f;
            for (int i = 0; i < fruit.reactivity; i++) {
                script.reactivityAnchor.transform.GetChild(i).localPosition = new Vector3(-.1f * (i - midIndex), 0, 0);
            }
        }
    }
}

public class EntityGadgetHandler : EntityScriptHandler {
    EntityGadget gadget;

    public EntityGadgetHandler(EntityScript script, EntityGadget gadget) : base(script) {
        this.gadget = gadget;
        script.tmpName.text = gadget.name;
        script.rendererIcon.gameObject.SetActive(true);
        script.rendererIcon.sprite = script.icons.First(i => i.name == "icon_gadget_" + gadget.name.ToLower());
    }
    public override void Update() {
        if (gadget.subtype == EntitySubtype.Flinger) {
            bool hasCondition = (gadget as EntityFlinger).condition != null;
            script.tmpName.text = hasCondition ? "Conditional Flinger" : "Flinger";
        }
    }
}

public class EntityFixtureHandler : EntityScriptHandler {
    EntityFixture fixture;

    public EntityFixtureHandler(EntityScript script, EntityFixture fixture) : base(script) {
        this.fixture = fixture;
        script.tmpName.text = fixture.name;
        script.rendererIcon.gameObject.SetActive(true);
        script.rendererIcon.sprite = script.icons.First(i => i.name == "icon_fixture_" + fixture.name.ToLower());
    }
}