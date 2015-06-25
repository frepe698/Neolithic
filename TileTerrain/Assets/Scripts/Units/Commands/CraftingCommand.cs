using UnityEngine;
using System.Collections;

public class CraftingCommand : Command {

    private RecipeData recipeData;

    private float timer;

    public CraftingCommand(Hero hero, RecipeData recipeData)
        : base(hero)
    {
        this.recipeData = recipeData;
    }

    public override void start()
    {
        timer = 0.0f;
        actor.setMoving(false);
        actor.setAnimation(actor.getIdleAnim());
    }

    public override void update()
    {
        timer += Time.deltaTime;

        if (timer >= 2.0f)
        {
            GameMaster.getGameController().requestItemCraft(actor.getID(), recipeData.name);
            setCompleted();
        }
        else if(actor.getID() == GameMaster.getPlayerUnitID())
        {
            GameMaster.getGUIManager().setProgressBar("Crafting " + recipeData.product + "...", timer / 2.0f);
        }
    }

}
