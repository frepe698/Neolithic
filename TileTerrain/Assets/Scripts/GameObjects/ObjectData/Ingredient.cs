using UnityEngine;
using System.Collections;
using Edit;

public class Ingredient {

	public readonly string name;
	public readonly int amount;

    public Ingredient()
    { 
    }

    public Ingredient(IngredientEdit edit)
    {
        name = edit.name;
        amount = edit.amount;
    }
}
