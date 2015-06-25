using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientController : GameController {

    public override void requestGameStart()
    {
        //do nade
    }

    [RPC]
    public override void setPlayerLoaded(int playerID)
    {
        gameMaster.getNetView().RPC("setPlayerLoaded", RPCMode.Server, playerID);
    }

    public override void requestLaneSpawning()
    {
        //do nada;
    }

    public override void requestAILevelIncrease()
    {
        //do nada
    }

    public override void requestDamageBase(int team, int damage, int unitID)
    {
        //do nada
    }

    public override void requestAddFavour(int team, int favour)
    {
        //do nada
    }
    [RPC]
    public override void requestAIUnitSpawn(int unitID, string name, float x, float y)
    {
        //Do NADA
    }

    public override void requestHeroStartRespawn(int unitID)
    {
        //do nada
    }

    public override void requestRespawnHero(int unitID)
    {
        //do nada
    }

   

    public override void requestSpawnerRespawn(int spawnerID)
    {
        //Do nada
    }
    public override void requestDaySpawnerRespawn(int spawnerID)
    {
        //Do nada
    }
    public override void requestNightSpawnerRespawn(int spawnerID)
    {
        //Do nada
    }
    public override void requestSpawnerRemoveAll(int spawnerID)
    {
        //Do nada
    }
    public override void requestDaySpawnerRemoveAll(int spawnerID)
    {
        //Do nada
    }
    public override void requestNightSpawnerRemoveAll(int spawnerID)
    {
        //Do nada
    }

    public override void requestKillActor(int targetID, int killerID)    {        //Do nada    }

	[RPC]
	public override void requestMoveCommand(int unitID, float x, float y)
	{
		gameMaster.getNetView().RPC("requestMoveCommand", RPCMode.Server, unitID, x, y);
	}

	[RPC]
	public override void requestGatherCommand(int unitID, float goalX, float goalY)
	{
		gameMaster.getNetView().RPC ("requestGatherCommand", RPCMode.Server, unitID, goalX, goalY);
	}

    [RPC]
    public override void requestActionCommand(int unitID, float goalX, float goalY)
    {
        gameMaster.getNetView().RPC("requestActionCommand", RPCMode.Server, unitID, goalX, goalY);
    }

	[RPC]
	public override void requestAttackCommandUnit(int unitID, int targetID)
	{
		gameMaster.getNetView().RPC ("requestAttackCommandUnit", RPCMode.Server, unitID, targetID);
	}

	[RPC]
	public override void requestAttackCommandPos(int unitID, Vector3 target)
	{
		gameMaster.getNetView().RPC ("requestAttackCommandPos", RPCMode.Server, unitID, target);
	}

    [RPC]
    public override void requestAbilityCommandID(int unitID, int targetID, int ability)
    {
        gameMaster.getNetView().RPC("requestAbilityCommandID", RPCMode.Server, unitID, targetID, ability);
    }
    [RPC]
    public override void requestAbilityCommandVec(int unitID, Vector3 target, int ability)
    {
        gameMaster.getNetView().RPC("requestAbilityCommandVec", RPCMode.Server, unitID, target, ability);
    }



	[RPC]
	public override void requestLootCommand(int unitID, int tileX, int tileY, int lootID)
	{
		gameMaster.getNetView().RPC ("requestLootCommand", RPCMode.Server, unitID, tileX, tileY, lootID);
	}


	
	protected override IEnumerator lagMove(int unitID, float x, float y, float lag)
	{
		yield return new WaitForSeconds(lag);
		gameMaster.getNetView().RPC("requestMoveCommand", RPCMode.Server, unitID, x, y);
	}

	public override void requestGather(int unitID, Vector2i tile)
	{
		//do nada
	}

    public override void requestWarp(int unitID, Vector2i tile)
    {
        //do nada
    }

	public override void requestAttack(int unitID, int targetID)
	{
		//do nada
	}

	/*public override void requestFireProjectile(int unitID, Vector3 target)
	{
		//do nada
	}*/
    public override void requestFireProjectile(int unitID, Vector3 target, string dataName, string projectileName)
    {
        //do nada
    }

    [RPC]
    public override void requestLearnAbility(string ability, int unitID, int index)
    {
        gameMaster.getNetView().RPC("requestLearnAbility", RPCMode.Server, ability, unitID, index);
    }

    public override void requestAddBuff(int unitID, string name, params object[] parameters)
    {
        //Do nada
    }

    public override void requestApplyEffect(int unitID, int targetID, string effectName)
    {
        //Do nade
    }

	public override void requestResourceLootDrop(string resourceName, Vector2i tile, int unitID)
	{
		//do nada
	}

	public override void requestUnitLootDrop(string unitName, Vector2i tile)
	{
		//do nada
	}

	public override void requestLoot(int unitID, Vector2i tile, int lootID)
	{
		//do nade
	}

	public override void requestProjectileHit(int damage, int unitID, int targetID)
	{
		//do nada
	}

    public override void requestHit(int damage, int unitID, int targetID, int skill = -1)
    {
        //do nada
    }

    public override void requestChangeEnergy(int targetID, int energy)
    {
       //do nada
    }

    public override void requestChangeHealth(int targetID, int health)
    {
        //do nada
    }

    public override void syncVitals()
    {
        //do nada
    }

    [RPC]
    public override void requestCheatCommand(int unitID, int commandID, string parameters)
    {
        gameMaster.getNetView().RPC("requestCheatCommand", RPCMode.Server, unitID, commandID, parameters);
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

    public override void requestRemoveActor(int unitID)
    {
        //do nada
    }
	

	[RPC]
	public override void requestItemChange(int unitID, int itemIndex)
	{
		gameMaster.getNetView().RPC("requestItemChange", RPCMode.Server, unitID, itemIndex);
	}

	[RPC]
	public override void requestItemConsume(int unitID, int itemIndex)
	{
		gameMaster.getNetView().RPC ("requestItemConsume", RPCMode.Server, unitID, itemIndex);
	}

    [RPC]
    public override void requestBuildBuilding(int unitID, string recipeName, int x, int y)
    {
        gameMaster.getNetView().RPC("requestBuildBuilding", RPCMode.Server, unitID, recipeName, x, y);
    }

    [RPC]
    public override void requestBuildingCommand(int unitID, int buildingID)
    {
        gameMaster.getNetView().RPC("requestBuildingCommand", RPCMode.Server, unitID, buildingID);
    }

    [RPC]
    public override void requestCraftingCommand(int unitID, string recipeName)
    {
        gameMaster.getNetView().RPC("requestCraftingCommand", RPCMode.Server, unitID, recipeName);
    }

    [RPC]
    public override void requestItemCraft(int unitID, string name)
    {
        gameMaster.getNetView().RPC("requestItemCraft", RPCMode.Server, unitID, name);
    }

    [RPC]
    public override void requestItemDrop(int unitID, int itemIndex)
    {
        gameMaster.getNetView().RPC("requestItemDrop", RPCMode.Server, unitID, itemIndex);
    }

	public override void update()
	{
		updateInput();
	}
}
