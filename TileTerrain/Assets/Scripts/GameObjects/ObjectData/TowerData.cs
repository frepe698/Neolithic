using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class TowerData : BuildingData {

    public readonly int damage;
    public readonly float attackspeed;
    [XmlElement(IsNullable = false)]
    public readonly string attackSound;
    public readonly int lineofsight;

    [XmlArray("abilities")]
    public readonly string[] abilities;

    public readonly string basicattack;

    public TowerData()
    { 
    }

    public TowerData(TowerEdit edit)
        : base(edit)
    {
        damage = edit.damage;
        attackspeed = edit.attackspeed;
        if (edit.attackSound != null) attackSound = edit.attackSound;
        lineofsight = edit.lineofsight;

        if (edit.abilities != null) abilities = edit.abilities.Trim().Split('\n');

        basicattack = edit.basicattack;
    }

    public override Building getBuilding(Vector2i position, float yRotation, int id)
    {
        return new Tower(this, position, yRotation, id);
    }
}
