using UnityEngine;
using System.Collections;

public class OfflineController : GameController {



	[RPC]
	public override void requestMoveCommand(int unitID, float x, float y)
	{
		Unit unit = GameMaster.getUnit(unitID);
		if(unit != null) approveMoveCommand(unitID, x, y, unit.getPosition());
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
	public override void requestAttackCommand(int unitID, int targetID)
	{
		Unit unit = GameMaster.getUnit(unitID);
		Unit target = GameMaster.getUnit (targetID);
        if (target != null && unit != null && unit.canStartCommand(new AttackCommand(unit, target)))
		{
			approveAttackCommand(unitID, targetID, GameMaster.getUnit(unitID).getPosition());
		}
	}

	[RPC]
	public override void requestRangedAttackCommand(int unitID, Vector3 target)
	{
		Unit unit = GameMaster.getUnit(unitID);
        if (unit != null && unit.canStartCommand(new RangedAttackCommand(unit, target)))
		{
			approveRangedAttackCommand(unitID, target, GameMaster.getUnit(unitID).getPosition());
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
		if(resObject != null)
		{
			int damage = GameMaster.getHero(unitID).getDamage(resObject.getDamageType());
			if(resObject.getHealth() <= damage)
			{
				gatherResource(tile.x, tile.y);
				changeEnergy(unitID, -5);
			}
			else
			{
				hitResource(tile.x, tile.y, damage);
				changeEnergy(unitID, -5);
			}
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
				killUnit(targetID, unitID);
				changeEnergy(unitID, -5);
			}
			else
			{
				hitUnit(targetID, unitID, damage);
				changeEnergy(unitID, -5);
			}
		}
		
	}

	public override void requestFireProjectile(int unitID, Vector3 target)
	{
		Unit unit = GameMaster.getUnit(unitID);
		if(unit != null)
		{
			float damage = unit.getDamage(0);
			string name = unit.getProjectileName();
			approveFireProjectile(unitID, target, name, damage);
		}
	}

	public override void requestLoot(int unitID, Vector2i tile, int lootID)
	{
		lootObject(unitID, tile.x, tile.y, lootID);
	}

	public override void requestResourceLootDrop(string resourceName, Vector2i tile)
	{
		dropResourceLoot(resourceName,Random.seed,tile.x,tile.y);
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
	public override void requestItemCraft(int unitID, string name)
	{
		if(GameMaster.getHero(unitID).getInventory().hasIngredients(DataHolder.Instance.getRecipeData(name).ingredients))
		{
			approveItemCraft(unitID, name);
		}
	}

	public override void requestProjectileHit(float damage, int unitID, int targetID)
	{
		Unit target = GameMaster.getUnit(targetID);
		if(target == null) return;
		target.playSound("hitBow01");
		if(target.getHealth() <= damage)
		{
			killUnit(targetID, unitID);
			changeEnergy(unitID, -5);
		}
		else
		{
			hitUnit(targetID, unitID, damage);
			changeEnergy(unitID, -5);
		}
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
	

}
