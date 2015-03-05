using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class EquipmentRecipeData : RecipeData
{

    public EquipmentRecipeData()
    {
    }

    public EquipmentRecipeData(EquipmentRecipeEdit edit)
        : base(edit)
    {

    }

    public override Item getCraftedItem()
    {
        return new EquipmentItem(product);
    }
}
