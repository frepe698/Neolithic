using UnityEngine;
using System.Collections;

public class OfflineController : GameController {

    public override void requestGameStart()
    {
        approveGameStart();
    }

    [RPC]
    public override void setPlayerLoaded(int playerID)
    {
        requestGameStart();
    }

    public override void requestLaneSpawning()
    {
        approveLaneSpawning();
    }

    public override void requestAILevelIncrease()
    {
        approveAILevelIncrease();
    }

    public override void requestHeroStartRespawn(int unitID)
    {
        approveHeroStartRespawn(unitID);
    }

    public override void requestRespawnHero(int unitID)
    {
        approveRespawnHero(unitID);
    }
    public override void requestDamageBase(int team, int damage, int unitID)
    {
        approveDamageBase(team, damage, unitID);
    }
    public override void requestAddFavour(int team, int favour)
    {
        approveAddFavour(team, favour);
    }
    [RPC]
    public override void requestAIUnitSpawn(int unitID, string name, float x, float y)
    {
        approveAIUnitSpawn(unitID, name, x, y);
    }

    public override void requestSpawnerRespawn(int spawnerID)
    {
        approveSpawnerRespawn(spawnerID);
    }
    public override void requestDaySpawnerRespawn(int spawnerID)
    {
        approveDaySpawnerRespawn(spawnerID);
    }
    public override void requestNightSpawnerRespawn(int spawnerID)
    {
        approveNightSpawnerRespawn(spawnerID);
    }

    public override void requestSpawnerRemoveAll(int spawnerID)
    {
        approveSpawnerRemoveAll(spawnerID);
    }
    public override void requestDaySpawnerRemoveAll(int spawnerID)
    {
        approveDaySpawnerRemoveAll(spawnerID);
    }
    public override void requestNightSpawnerRemoveAll(int spawnerID)
    {
        approveNightSpawnerRemoveAll(spawnerID);
    }

    public override void requestKillActor(int targetID, int killerID)
    {
        killActor(targetID, killerID);
    }

	[RPC]
	public override void requestMoveCommand(int unitID, float x, float y)
	{
		Unit unit = GameMaster.getActor(unitID) as Unit;
		if(unit != null && unit.canOverrideCurrentCommand()) approveMoveCommand(unitID, x, y, unit.getPosition());
	}

	[RPC]
	public override void requestGatherCommand(int unitID, float goalX, float goalY)
	{
		Unit unit = GameMaster.getHero(unitID);
		ResourceObject res = World.tileMap.getTile ((int)goalX, (int)goalY).getResourceObject();
		if(res != null && unit != null && unit.canStartCommand(new GatherCommand(unit, res)))
		{
			approveGatherCommand(unitID, goalX, goalY, GameMaster.getHero(unitID).getPosition());
		}
	}

    [RPC]
    public override void requestActionCommand(int unitID, float goalX, float goalY)
    {
        Unit unit = GameMaster.getHero(unitID);
        WarpObject wrp = World.tileMap.getTile((int)goalX, (int)goalY).getTileObject() as WarpObject; 
        if (wrp != null && unit != null && unit.canStartCommand(new ActionCommand(unit, wrp)))
        {
            approveActionCommand(unitID, goalX, goalY, GameMaster.getHero(unitID).getPosition());
        }
    }

	[RPC]
	public override void requestAttackCommandUnit(int unitID, int targetID)
	{
		Actor actor = GameMaster.getActor(unitID);
		Actor target = GameMaster.getActor (targetID);
        if (target != null && actor != null && actor.canStartCommand(new AbilityCommand(actor, target, actor.getBasicAttack())))
		{
			approveAttackCommandUnit(unitID, targetID, GameMaster.getActor(unitID).getPosition());
		}
	}

	[RPC]
	public override void requestAttackCommandPos(int unitID, Vector3 target)
	{
		Actor actor = GameMaster.getActor(unitID);
        if (actor != null && actor.canStartCommand(new AbilityCommand(actor, target, actor.getBasicAttack())))
		{
			approveAttackCommandPos(unitID, target, GameMaster.getActor(unitID).getPosition());
		}
	}

    [RPC]
    public override void requestAbilityCommandID(int unitID, int targetID, int ability)
    {
        Actor actor = GameMaster.getActor(unitID);
        Actor target = GameMaster.getActor(targetID);
        if (actor != null && target != null && actor.hasAbility(ability) && actor.canStartCommand(new AbilityCommand(actor, target, actor.getAbility(ability))))
        {
            approveAbilityCommandID(unitID, targetID, actor.getPosition(), ability);
        }
    }

    [RPC]
    public override void requestAbilityCommandVec(int unitID, Vector3 target, int ability)
    {
        Actor actor = GameMaster.getActor(unitID);
        if(actor != null && actor.hasAbility(ability) && actor.canStartCommand(new AbilityCommand(actor,target, actor.getAbility(ability))))
        {
            approveAbilityCommandVec(unitID, target, actor.getPosition(), ability);
        }
    }

	[RPC]
	public override void requestLootCommand(int unitID, int tileX, int tileY, int lootID)
	{
		Unit unit = GameMaster.getHero(unitID);
		if(unit != null) approveLootCommand(unitID, tileX, tileY, lootID, unit.getPosition());
	}

	protected override IEnumerator lagMove(int unitID, float x, float y, float lag)
	{
		yield return new WaitForSeconds(lag);
		approveMoveCommand(unitID, x, y, GameMaster.getHero(unitID).getPosition());
	}

	public override void update()
	{
		updateInput();
		gameMaster.updateAI();
	}

	public override void requestGather(int unitID, Vector2i tile)
	{
		ResourceObject resObject = World.tileMap.getTile(tile).getResourceObject();
        if (resObject != null)
        {
            if (resObject.canBeHarvested())
            {
                harvestResource(tile.x, tile.y, unitID);
                changeEnergy(unitID, -5);
            }
            else
            {
                int damage = GameMaster.getHero(unitID).getDamage(resObject.getDamageType());
                if (resObject.getHealth() <= damage)
                {
                    gatherResource(tile.x, tile.y, unitID);
                    changeEnergy(unitID, -5);
                    if (resObject.getDamageType() == 1)
                    {
                        giveExperience(unitID, Skill.WoodChopping, damage);
                    }
                    else
                    {
                        giveExperience(unitID, Skill.Mining, damage);
                    }
                }
                else
                {
                    hitResource(tile.x, tile.y, damage);
                    changeEnergy(unitID, -5);
                    if (resObject.getDamageType() == 1)
                    {
                        giveExperience(unitID, Skill.WoodChopping, damage);
                    }
                    else
                    {
                        giveExperience(unitID, Skill.Mining, damage);
                    }

                }
            }
        }

	}

    public override void requestWarp(int unitID, Vector2i tile)
    {
        WarpObject warpObject = World.tileMap.getTile(tile).getTileObject() as WarpObject;
        if (warpObject != null)
        {
            Unit unit = GameMaster.getActor(unitID) as Unit;
            if(unit != null)
            {
                unit.warp(warpObject.getDestination());
                unit.setRotation(new Vector3(0, warpObject.getDestRotation(), 0));
            }
        }
    }

	public override void requestAttack(int unitID, int targetID)
	{
		Actor target = GameMaster.getActor(targetID);
		if(target != null)
		{
			int damage = GameMaster.getActor(unitID).getDamage(0);
			if(target.getHealth() <= damage)
			{
				killActor(targetID, unitID);
				changeEnergy(unitID, -5);
                giveExperience(unitID, Skill.Melee, damage);
			}
			else
			{
				hitActor(targetID, unitID, damage);
				changeEnergy(unitID, -5);
                giveExperience(unitID, Skill.Melee, damage);
			}
		}
		
	}

    /*
	public override void requestFireProjectile(int unitID, Vector3 target)
	{
		Unit unit = GameMaster.getUnit(unitID);
		if(unit != null)
		{
			int damage = unit.getDamage(0);
			string name = unit.getProjectileName();
			approveFireProjectile(unitID, target, name, damage);
		}
	}
    */
    public override void requestFireProjectile(int unitID, Vector3 target, string dataName, string projectileName)
    {
        Actor actor = GameMaster.getActor(unitID);
        if (actor != null)
        {
            approveFireProjectile(unitID, target, dataName, projectileName);
        }
    }

	public override void requestLoot(int unitID, Vector2i tile, int lootID)
	{
		lootObject(unitID, tile.x, tile.y, lootID);
	}

	public override void requestResourceLootDrop(string resourceName, Vector2i tile, int unitID)
	{
		dropResourceLoot(resourceName,Random.seed,tile.x,tile.y, unitID);
	}

	public override void requestUnitLootDrop(string unitName, Vector2i tile)
	{
		dropUnitLoot(unitName,Random.seed,tile.x,tile.y);
	}

	[RPC]
	public override void requestItemChange(int unitID, int itemIndex)
	{
		approveItemChange(unitID, itemIndex);
	}

	[RPC]
	public override void requestItemConsume(int unitID, int itemIndex)
	{
		approveItemConsume(unitID, itemIndex);
	}

    [RPC]
    public override void requestBuildBuilding(int unitID, string recipeName, int x, int y)
    {
        BuildingRecipeData recipe = DataHolder.Instance.getBuildingRecipeData(recipeName);
        BuildingData data = DataHolder.Instance.getBuildingData(recipe.product);
        if (!World.tileMap.getTile(x, y).isBuildable(data is MonumentData)) return;
        Hero hero = GameMaster.getHero(unitID);
        
        //Check if the hero's skill level is high enough and it has all the ingredients
        if (hero.getSkillManager().getSkill((int)recipe.skill).getLevel() >= recipe.requiredSkillLevel &&
            hero.getInventory().hasIngredients(recipe.ingredients))
        {
            approveBuildBuilding(unitID, recipeName, x, y);
        }
    }

    [RPC]
    public override void requestBuildingCommand(int unitID, int buildingID)
    {
        approveBuildingCommand(unitID, buildingID);
    }

    [RPC]
    public override void requestCraftingCommand(int unitID, string recipeName)
    {
        Hero hero = GameMaster.getHero(unitID);
        RecipeData data = DataHolder.Instance.getRecipeData(recipeName);

        if (hero != null && data != null)
        {
            if (hero.canStartCommand(new CraftingCommand(hero, data)))
            {
                approveCraftingCommand(unitID, recipeName);
            }
        }
    }

	[RPC]
	public override void requestItemCraft(int unitID, string name)
	{
        Hero hero = GameMaster.getHero(unitID);
        RecipeData data = DataHolder.Instance.getRecipeData(name);

        //Check if the hero's skill level is high enough and it has all the ingredients
		if(hero.getSkillManager().getSkill((int)data.skill).getLevel() >= data.requiredSkillLevel && 
            hero.getInventory().hasIngredients(data.ingredients))
		{
			approveItemCraft(unitID, name);
		}
	}

    [RPC]
    public override void requestItemDrop(int unitID, int itemIndex)
    {
        Hero hero = GameMaster.getHero(unitID);
        Item item = hero.getInventory().getItem(itemIndex);
        if (item != null && item.canBeDropped())
        {
            approveItemDrop(unitID, itemIndex, item.getName(), hero.getPosition());
        }
    }

	public override void requestProjectileHit(int damage, int unitID, int targetID)
	{
		Actor target = GameMaster.getActor(targetID);
		if(target == null) return;
		target.playSound("hitBow01");
		if(target.getHealth() <= damage)
		{
			killActor(targetID, unitID);
			//changeEnergy(unitID, -5);
		}
		else
		{
			hitActor(targetID, unitID, damage);
			//changeEnergy(unitID, -5);
		}
	}

    public override void requestHit(int damage, int unitID, int targetID, int skill = -1)
    {
        Actor target = GameMaster.getActor(targetID);
        if (target == null || !target.isAlive()) return;
        if (damage < 0)
        {
            changeHealth(targetID, -damage);
            if (skill >= 0) giveExperience(unitID, skill, -damage);
        }
        else
        {
            target.playSound("hitBow01");
            if (target.getHealth() <= damage)
            {
                killActor(targetID, unitID);
            }
            else
            {
                hitActor(targetID, unitID, damage);
            }
            if (target.isHostile())
            {
                if (skill >= 0) giveExperience(unitID, skill, damage);
            }
            else
            {
                giveExperience(unitID, (int)Skills.Hunting, damage * 2);
            }
        }
        
    }

    public override void requestChangeEnergy(int targetID, int energy)
    {
        changeEnergy(targetID, energy);
    }
    public override void requestChangeHealth(int targetID, int health)
    {
        changeHealth(targetID, health);
    }

    public override void syncVitals()
    {
        //do nada
    }

    [RPC]
    public override void requestLearnAbility(string ability, int unitID, int index)
    {
        approveLearnAbility(ability, unitID, index);
    }

    public override void requestAddBuff(int unitID, string name, params object[] parameters)
    {
        approveAddBuff(unitID, name, parameters);
    }

    public override void requestApplyEffect(int unitID, int targetID, string effectName)
    {
        Actor target = GameMaster.getActor(targetID);
        if (target == null || !target.isAlive())
            return;

        Debug.Log("approve apply effect");
        approveApplyEffect(unitID, targetID, effectName);
    }

    [RPC]
    public override void requestCheatCommand(int unitID, int commandID, string parameters)
    {
        approveCheatCommand(unitID, commandID, parameters);
    }

    public override void sendChatMessage(string msg)
    {
        recieveChatMessage(GameMaster.getPlayerUnitID(), msg);
    }

    public override void requestRemoveActor(int unitID)
    {
        approveRemoveActor(unitID);
    }
	

}
