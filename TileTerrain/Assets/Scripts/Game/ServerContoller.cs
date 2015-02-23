using UnityEngine;
using System.Collections;

public class ServerController : GameController {



	[RPC]
	public override void requestMoveCommand(int unitID, float x, float y)
	{
		Unit unit = GameMaster.getUnit(unitID);
		if(unit != null) gameMaster.getNetView().RPC("approveMoveCommand", RPCMode.All, unitID, x, y, unit.getPosition());
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
	public override void requestAttackCommand(int unitID, int targetID)
	{
		Unit unit = GameMaster.getUnit(unitID);
		Unit target = GameMaster.getUnit (targetID);
		if(target != null && unit != null && unit.canStartCommand(new AttackCommand(unit, target)))
		{
			gameMaster.getNetView().RPC ("approveAttackCommand", RPCMode.All, unitID, targetID, GameMaster.getUnit(unitID).getPosition());
		}
	}

	[RPC]
	public override void requestRangedAttackCommand(int unitID, Vector3 target)
	{
		Unit unit = GameMaster.getUnit(unitID);
        if (unit != null && unit.canStartCommand(new RangedAttackCommand(unit, target)))
		{
			gameMaster.getNetView().RPC ("approveRangedAttackCommand", RPCMode.All, unitID, target, GameMaster.getUnit(unitID).getPosition());
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
				gameMaster.getNetView().RPC ("gatherResource", RPCMode.All, tile.x, tile.y);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
			}
			else
			{
				gameMaster.getNetView().RPC ("hitResource", RPCMode.All, tile.x, tile.y, damage);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
			}
		}
	}

	public override void requestAttack(int unitID, int targetID)
	{
		Unit target = GameMaster.getUnit(targetID);
		if(target != null)
		{
			float damage = GameMaster.getUnit(unitID).getDamage(0);
			if(target.getHealth() <= damage)
			{
				gameMaster.getNetView().RPC ("killUnit", RPCMode.All, targetID, unitID);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
			}
			else
			{
				gameMaster.getNetView().RPC ("hitUnit", RPCMode.All, targetID, unitID, damage);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
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
			gameMaster.getNetView().RPC ("approveFireProjectile", RPCMode.All, unitID, target, name, damage);
		}
	}
	

	public override void requestLoot(int unitID, Vector2i tile, int lootID)
	{
		if(World.tileMap.getTile(tile).getLootableObject(lootID) != null)
		{
			gameMaster.getNetView ().RPC ("lootObject", RPCMode.All, unitID, tile.x, tile.y, lootID);
		}
	}

	
	public override void requestResourceLootDrop(string resourceName, Vector2i tile)
	{
		gameMaster.getNetView().RPC ("dropResourceLoot", RPCMode.All, resourceName, Random.seed, tile.x, tile.y);
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
		if(GameMaster.getHero(unitID).getInventory().hasIngredients(DataHolder.Instance.getRecipeData(name).ingredients))
		{
			gameMaster.getNetView().RPC ("approveItemCraft", RPCMode.All, unitID, name);
		}
	}

	public override void requestProjectileHit(float damage, int unitID, int targetID)
	{
		Unit target = GameMaster.getUnit(targetID);
		if(target != null)
		{
			if(target.getHealth() <= damage)
			{
				gameMaster.getNetView().RPC ("killUnit", RPCMode.All, targetID, unitID);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
			}
			else
			{
				gameMaster.getNetView().RPC ("hitUnit", RPCMode.All, targetID, unitID, damage);
				gameMaster.getNetView ().RPC ("changeEnergy", RPCMode.All, unitID, -5);
			}
		}

	}

}
