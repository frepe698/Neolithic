using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class ConsumableRecipeData : RecipeData {

    public ConsumableRecipeData()
    { 
    }

    public ConsumableRecipeData(ConsumableRecipeEdit edit)
        : base(edit)
    {

    }

    public override Item getCraftedItem()
    {
        return new ConsumableItem(product);
    }
}
