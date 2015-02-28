using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class MaterialRecipeData : RecipeData
{

    public MaterialRecipeData()
    {
    }

    public MaterialRecipeData(MaterialRecipeEdit edit)
        : base(edit)
    {

    }

    public override Item getCraftedItem()
    {
        return new MaterialItem(product);
    }
}
