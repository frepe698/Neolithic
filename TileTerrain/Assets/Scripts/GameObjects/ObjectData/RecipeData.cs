using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public abstract class RecipeData : ObjectData
{
    public readonly bool isBasicRecipe;
    public readonly string product;
    public readonly float creationTime = 1;

    [XmlArray("ingredients"), XmlArrayItem("Ingredient")]
    public readonly Ingredient[] ingredients;

    public readonly string description;

    public readonly Skills skill = Skills.Crafting;
    public readonly int expAmount;

    public readonly int requiredSkillLevel;

    public RecipeData()
    { 
    }

    public RecipeData(RecipeEdit edit)
        : base(edit)
    {
        isBasicRecipe = edit.isBasicRecipe;
        product = edit.product;
        creationTime = edit.creationTime;
        ingredients = edit.getIngredients();
        description = edit.description;

        skill = edit.skill;
        expAmount = edit.expAmount;
        requiredSkillLevel = edit.requiredSkillLevel;
    }

    public abstract Item getCraftedItem();
}
