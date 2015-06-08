using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class BuildingData : ObjectData {

    public readonly int health;
    public readonly int healthperlevel;
    public readonly float lifegen;
    public readonly float warmth;

    public readonly float size;

    public readonly float buildTime;

    [XmlArray("craftingRecipeNames")]
    public readonly string[] craftingRecipeNames;

    private RecipeData[] craftingRecipes;

    public BuildingData()
    { 
    }

    public BuildingData(BuildingEdit edit)
        : base(edit)
    {
        this.health = edit.health;
        this.healthperlevel = edit.healthperlevel;
        this.lifegen = edit.lifegen;
        this.warmth = edit.warmth;
        this.size = edit.size;
        this.buildTime = edit.buildTime;

        if (edit.craftingRecipeNames != null) craftingRecipeNames = edit.craftingRecipeNames.Trim().Split('\n');
    }

    public void initRecipes()
    {
        if(craftingRecipeNames == null || craftingRecipeNames.Length == 0) 
            return;

        craftingRecipes = new RecipeData[craftingRecipeNames.Length];

        for (int i = 0; i < craftingRecipes.Length; i++)
        {
            string s = craftingRecipeNames[i];
            RecipeData data = DataHolder.Instance.getRecipeData(s);
            if (data == null)
            {
                Debug.LogError("Couldn't find recipe data @" + s);
                break;
            }
            craftingRecipes[i] = data;

        }
    }

    public RecipeData[] getCraftingRecipes()
    {
        return craftingRecipes;
    }

    public virtual Building getBuilding(Vector2i position, float yRotation, int id, int team)
    {
        return new Building(this, position, yRotation, id, team);
    }
}
