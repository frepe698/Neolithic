using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using Edit;

public class ArmorData : EquipmentData {

    public readonly int armor;
    public readonly int speedPenalty;

    public readonly int armorType = 0;

    public ArmorData()
    { 
    }

    public ArmorData(ArmorEdit edit)
        : base(edit)
    {
        armor = edit.armor;
        speedPenalty = edit.speedPenalty;
        armorType = (int)edit.armorType;
    }

    public override string getTooltipStatsString()
    {
        return "Armor: " + armor +
            "\nSpeed Penalty: " + speedPenalty;
    }
	
}
