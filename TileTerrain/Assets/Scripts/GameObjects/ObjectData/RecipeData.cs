using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public abstract class RecipeData : ObjectData
{

    public readonly string product;

    [XmlArray("ingredients"), XmlArrayItem("Ingredient")]
    public readonly Ingredient[] ingredients;

    public readonly string description;

    public RecipeData()
    { 
    }

    public RecipeData(RecipeEdit edit)
        : base(edit)
    {
        product = edit.product;
        ingredients = edit.getIngredients();
        description = edit.description;
    }

    public abstract Item getCraftedItem();
}
