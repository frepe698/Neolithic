using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerController : GameController {

    public override void requestGameStart()
    {
        gameMaster.getNetView().RPC("approveGameStart", RPCMode.All);
    }

    [RPC]
    public override void setPlayerLoaded(int playerID)
    {
        NetworkMaster.findPlayer(playerID).setInGame(true);

        //If all players are in game start the game
        foreach (OnlinePlayer player in NetworkMaster.getAllPlayers())
        {
            if (!player.isInGame()) return;
        }
        requestGameStart();
    }

    public override void requestLaneSpawning()
    {
        gameMaster.getNetView().RPC("approveLaneSpawning", RPCMode.All);
    }

    public override void requestAILevelIncrease()
    {
        gameMaster.getNetView().RPC("approveAILevelIncrease", RPCMode.All);
    }

    public override void requestHeroStartRespawn(int unitID)
    {
        gameMaster.getNetView().RPC("approveHeroStartRespawn", RPCMode.All, unitID);
    }

    public override void requestRespawnHero(int unitID)
    {
        gameMaster.getNetView().RPC("approveRespawnHero", RPCMode.All, unitID);
    }

    public override void requestDamageBase(int team, int damage, int unitID)
    {
        gameMaster.getNetView().RPC("approveDamageBase", RPCMode.All, team, damage, unitID);
    }

    public override void requestAddFavour(int team, int favour)
    {
        gameMaster.getNetView().RPC("approveAddFavour", RPCMode.All, team, favour);
    }

    [RPC]
    public override void requestAIUnitSpawn(int unitID, string name, float x, float y)
    {
        gameMaster.getNetView().RPC("approveAIUnitSpawn", RPCMode.All, unitID, name, x, y);
    }

    
    public override void requestSpawnerRespawn(int spawnerID)
    {
        gameMaster.getNetView().RPC("approveSpawnerRespawn", RPCMode.All, spawnerID);
    }
    public override void requestDaySpawnerRespawn(int spawnerID)
    {
        gameMaster.getNetView().RPC("approveDaySpawnerRespawn", RPCMode.All, spawnerID);
    }
    public override void requestNightSpawnerRespawn(int spawnerID)
    {
        gameMaster.getNetView().RPC("approveNightSpawnerRespawn", RPCMode.All, spawnerID);
    }


    public override void requestSpawnerRemoveAll(int spawnerID)
    {
        gameMaster.getNetView().RPC("approveSpawnerRemoveAll", RPCMode.All, spawnerID);
    }
    public override void requestDaySpawnerRemoveAll(int spawnerID)
    {
        gameMaster.getNetView().RPC("approveDaySpawnerRemoveAll", RPCMode.All, spawnerID);
    }
    public override void requestNightSpawnerRemoveAll(int spawnerID)
    {
        gameMaster.getNetView().RPC("approveNightSpawnerRemoveAll", RPCMode.All, spawnerID);
    }

	[RPC]
	public override void requestMoveCommand(int unitID, float x, float y)
	{
		Unit unit = GameMaster.getUnit(unitID);
		if(unit != null && unit.canOverrideCurrentCommand()) gameMaster.getNetView().RPC("approveMoveCommand", RPCMode.All, unitID, x, y, unit.getPosition());
	}

	[RPC]
	public override void requestGatherCommand(int unitID, float goalX, float goalY)
	{
		Unit unit = GameMaster.getHero(unitID);
		ResourceObject res = World.tileMap.getTile ((int)goalX, (int)goalY).getResourceObject();
        if (res != null && unit != null && unit.canStartCommand(new GatherCommand(unit, res)))
		{
			gameMaster.getNetView ().RPC ("approveGatherCommand", RPCMode.All, unitID, goalX, goalY, GameMaster.getHero(unitID).getPosition());
		}
	}

    [RPC]
    public override void requestActionCommand(int unitID, float goalX, float goalY)
    {
        Unit unit = GameMaster.getHero(unitID);
        WarpObject wrp = World.tileMap.getTile((int)goalX, (int)goalY).getTileObject() as WarpObject;
        if (wrp != null && unit != null && unit.canStartCommand(new ActionCommand(unit, wrp)))
        {
            gameMaster.getNetView().RPC("approveActionCommand", RPCMode.All, unitID, goalX, goalY, GameMaster.getHero(unitID).getPosition());
        }
    }

	[RPC]
	public override void requestAttackCommandUnit(int unitID, int targetID)
	{
		Unit unit = GameMaster.getUnit(unitID);
		Unit target = GameMaster.getUnit (targetID);
		if(target != null && unit != null && unit.canStartCommand(new AbilityCommand(unit, target, unit.getBasicAttack())))
		{
			gameMaster.getNetView().RPC ("approveAttackCommandUnit", RPCMode.All, unitID, targetID, GameMaster.getUnit(unitID).getPosition());
		}
	}

	[RPC]
	public override void requestAttackCommandPos(int unitID, Vector3 target)
	{
		Unit unit = GameMaster.getUnit(unitID);
        if (unit != null && unit.canStartCommand(new AbilityCommand(unit, target, unit.getBasicAttack())))
		{
			gameMaster.getNetView().RPC ("approveAttackCommandPos", RPCMode.All, unitID, target, GameMaster.getUnit(unitID).getPosition());
		}
	}

    [RPC]
    public override void requestAbilityCommandID(int unitID, int targetID, int ability)
    {
        Unit unit = GameMaster.getUnit(unitID);
        Unit target = GameMaster.getUnit(targetID);
        if (unit != null && target != null && unit.hasAbility(ability) && unit.canStartCommand(new AbilityCommand(unit, target, unit.getAbility(ability))))
        {
            gameMaster.getNetView().RPC("approveAbilityCommandID", RPCMode.All, unitID, targetID, unit.getPosition(), ability);
        }
    }

    [RPC]
    public override void requestAbilityCommandVec(int unitID, Vector3 target, int ability)
    {


        Unit unit = GameMaster.getUnit(unitID);
        if (unit != null && unit.hasAbility(ability) && unit.canStartCommand(new AbilityCommand(unit, target, unit.getAbility(ability))))
        {
            gameMaster.getNetView().RPC("approveAbilityCommandVec", RPCMode.All, unitID, target, unit.getPosition(), ability);
        }
    }

	[RPC]
	public override void requestLootCommand(int unitID, int tileX, int tileY, int lootID)
	{
		Unit unit = GameMaster.getUnit(unitID);
		if(unit != null) gameMaster.getNetView ().RPC ("approveLootCommand", RPCMode.All, unitID, tileX, tileY, lootID, unit.getPosition());
	}
	

	public override void update()
	{
		updateInput();
		gameMaster.updateAI();
	}

	protected override IEnumerator lagMove(int unitID, float x, float y, float lag)
	{
		yield return new WaitForSeconds(lag);
		gameMaster.getNetView().RPC("approveMoveCommand", RPCMode.All, unitID, x, y, GameMaster.getHero(unitID).getPosition());
	}

	public override void requestGather(int unitID, Vector2i tile)
	{
		ResourceObject resObject = World.tileMap.getTile(tile).getResourceObject();
		if(resObject != null)
		{
			int damage = GameMaster.getHero(unitID).getDamage(resObject.getDamageType());
			if(resObject.getHealth() <= damage)
			{
				gameMaster.getNetView().RPC ("gatherResource", RPCMode.All, tile.x, tile.y, unitID);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
                if(resObject.getDamageType() == 1)
                {
                    gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, Skill.WoodChopping, damage);
                }
                else
                {
                    gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, Skill.Mining, damage);
                }
			}
			else
			{
				gameMaster.getNetView().RPC ("hitResource", RPCMode.All, tile.x, tile.y, damage);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
                if (resObject.getDamageType() == 1)
                {
                    gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, Skill.WoodChopping, damage);
                }
                else
                {
                    gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, Skill.Mining, damage);
                }

			}
		}
	}

    public override void requestWarp(int unitID, Vector2i tile)
    {
        WarpObject warpObject = World.tileMap.getTile(tile).getTileObject() as WarpObject;
        if (warpObject != null)
        {
            Unit unit = GameMaster.getUnit(unitID);
            if (unit != null) gameMaster.getNetView().RPC("warpUnit", RPCMode.All, unitID, warpObject.getDestination().x, warpObject.getDestination().y, warpObject.getDestRotation());
        }
    }

	public override void requestAttack(int unitID, int targetID)
	{
		Unit target = GameMaster.getUnit(targetID);
		if(target != null)
		{
			int damage = GameMaster.getUnit(unitID).getDamage(0);
			if(target.getHealth() <= damage)
			{
				gameMaster.getNetView().RPC ("killUnit", RPCMode.All, targetID, unitID);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
                gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, Skill.Melee, damage);
			}
			else
			{
				gameMaster.getNetView().RPC ("hitUnit", RPCMode.All, targetID, unitID, damage);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
                gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, Skill.Melee, damage);
			}
		}
	}

	/*public override void requestFireProjectile(int unitID, Vector3 target)
	{
		Unit unit = GameMaster.getUnit(unitID);
		if(unit != null)
		{
			int damage = unit.getDamage(0);
			string name = unit.getProjectileName();
			gameMaster.getNetView().RPC ("approveFireProjectile", RPCMode.All, unitID, target, name, damage);
		}
	}*/

    public override void requestFireProjectile(int unitID, Vector3 target, string dataName, string projectileName)
    {
        Unit unit = GameMaster.getUnit(unitID);
        if (unit != null)
        {
            
            gameMaster.getNetView().RPC("approveFireProjectile", RPCMode.All, unitID, target, dataName, projectileName );
        }
    }
	

	public override void requestLoot(int unitID, Vector2i tile, int lootID)
	{
		if(World.tileMap.getTile(tile).getLootableObject(lootID) != null)
		{
			gameMaster.getNetView ().RPC ("lootObject", RPCMode.All, unitID, tile.x, tile.y, lootID);
		}
	}

	
	public override void requestResourceLootDrop(string resourceName, Vector2i tile, int unitID)
	{
		gameMaster.getNetView().RPC ("dropResourceLoot", RPCMode.All, resourceName, Random.seed, tile.x, tile.y, unitID);
	}

	public override void requestUnitLootDrop(string unitName, Vector2i tile)
	{
		gameMaster.getNetView ().RPC ("dropUnitLoot", RPCMode.All, unitName, Random.seed, tile.x, tile.y);
	}

	[RPC]
	public override void requestItemChange(int unitID, int itemIndex)
	{
		gameMaster.getNetView().RPC("approveItemChange", RPCMode.All, unitID, itemIndex);
	}

	[RPC]
	public override void requestItemConsume(int unitID, int itemIndex)
	{
		gameMaster.getNetView().RPC ("approveItemConsume", RPCMode.All, unitID, itemIndex);
	}

	[RPC]
	public override void requestItemCraft(int unitID, string name)
	{
        Hero hero = GameMaster.getHero(unitID);
        RecipeData data = DataHolder.Instance.getRecipeData(name);

        //Check if the hero's skill level is high enough and it has all the ingredients
        if (hero.getSkillManager().getSkill((int)data.skill).getLevel() >= data.requiredSkillLevel &&
            hero.getInventory().hasIngredients(data.ingredients))
		{
			gameMaster.getNetView().RPC ("approveItemCraft", RPCMode.All, unitID, name);
		}
	}

    [RPC]
    public override void requestItemDrop(int unitID, int itemIndex)
    {
        Hero hero = GameMaster.getHero(unitID);
        Item item = hero.getInventory().getItem(itemIndex);
        if (item != null && item.canBeDropped())
        {
            gameMaster.getNetView().RPC("approveItemDrop", RPCMode.All, unitID, itemIndex, item.getName(), hero.getPosition());
        }
    }

	public override void requestProjectileHit(int damage, int unitID, int targetID)
	{
		Unit target = GameMaster.getUnit(targetID);
		if(target != null)
		{
			if(target.getHealth() <= damage)
			{
				gameMaster.getNetView().RPC ("killUnit", RPCMode.All, targetID, unitID);
				//gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
                gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, Skill.Ranged, damage);
			}
			else
			{
				gameMaster.getNetView().RPC ("hitUnit", RPCMode.All, targetID, unitID, damage);
				//gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
                gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, Skill.Ranged, damage);
			}
		}

	}

    public override void requestHit(int damage, int unitID, int targetID, int skill = -1)
    {
        Unit target = GameMaster.getUnit(targetID);
        if (target != null)
        {
            if (damage < 0)
            {
                gameMaster.getNetView().RPC("changeHealth", RPCMode.All, targetID, -damage);
                if (skill >= 0) gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, skill, -damage);
            }
            else
            {
                if (target.getHealth() <= damage)
                {
                    gameMaster.getNetView().RPC("killUnit", RPCMode.All, targetID, unitID);
                }
                else
                {
                    gameMaster.getNetView().RPC("hitUnit", RPCMode.All, targetID, unitID, damage);
                }
                if (target.isHostile())
                {
                    if (skill >= 0) gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, skill, damage);
                }
                else
                {
                    gameMaster.getNetView().RPC("giveExperience", RPCMode.All, unitID, (int)Skills.Hunting, damage*2);
                }
            }
        }

    }

    public override void requestChangeEnergy(int targetID, int energy)
    {
        gameMaster.getNetView().RPC("changeEnergy", RPCMode.All, targetID, energy);
    }
    public override void requestChangeHealth(int targetID, int health)
    {
         gameMaster.getNetView().RPC("changeHealth", RPCMode.All, targetID, health);
    }

    public override void syncVitals()
    {
        foreach(Hero hero in GameMaster.getHeroes())
        {
            gameMaster.getNetView().RPC("syncHeroVitals", RPCMode.Others, hero.getID(), 
                (int)hero.getHeroStats().getHealth().getCurValue(), 
                (int)hero.getHeroStats().getEnergy().getCurValue());
        }
    }

    [RPC]
    public override void requestLearnAbility(string ability, int unitID, int index)
    {
        gameMaster.getNetView().RPC("approveLearnAbility", RPCMode.All, ability, unitID, index);
    }

    public override void requestAddBuff(int unitID, string name, params object[] parameters)
    {
        gameMaster.getNetView().RPC("approveAddBuff", RPCMode.All, unitID, name, parameters);
    }

    public override void requestApplyEffect(int unitID, int targetID, string effectName)
    {
        gameMaster.getNetView().RPC("approveApplyEffect", RPCMode.All, unitID, targetID, effectName);
    }

    [RPC]
    public override void requestCheatCommand(int unitID, int commandID, string parameters)
    {
        gameMaster.getNetView().RPC("approveCheatCommand", RPCMode.All, unitID, commandID, parameters);
    }

    public override void sendChatMessage(string msg)
    {
        if (msg.StartsWith("/all"))
        {
            gameMaster.getNetView().RPC("recieveChatMessage", RPCMode.All, NetworkMaster.getMyPlayerID(), "(All) " + msg.Substring(4));
        }
        else
        {
            List<OnlinePlayer> team = NetworkMaster.getTeamPlayers(NetworkMaster.getMe().getTeam());
            foreach (OnlinePlayer p in team)
            {
                if (p.getID() == NetworkMaster.getMyPlayerID()) recieveChatMessage(p.getID(), msg);
                else gameMaster.getNetView().RPC("recieveChatMessage", p.getNetworkPlayer(), NetworkMaster.getMyPlayerID(), msg);
            }
        }
    }
    public override void requestRemoveUnit(int unitID)
    {
        gameMaster.getNetView().RPC("approveRemoveUnit", RPCMode.All, unitID);
    }

}
