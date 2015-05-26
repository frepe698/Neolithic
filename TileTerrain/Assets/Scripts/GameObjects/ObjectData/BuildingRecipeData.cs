using UnityEngine;
using System.Collections;
using Edit;

public class BuildingRecipeData : RecipeData {

    public BuildingRecipeData() { }

    public BuildingRecipeData(BuildingRecipeEdit edit)
        : base(edit)
    {
    }

    public override Item getCraftedItem()
    {
        return null;
    }
}
