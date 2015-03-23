using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLobby : MonoBehaviour {

	NetworkView netView;
    private GlobalMenu globalMenu;
	private readonly int LEFT_MOUSE_BUTTON = 0;

	//private int[] selectedHeroes = new int[] {-1, -1, -1, -1};
	private Text[] playerNameHeroes = new Text[4];

    private RectTransform[] teamHolders;
    private Button[] joinTeamButtons;
    private List<Text>[] teamPlayerNames = new List<Text>[] {new List<Text>(), new List<Text>()}; 

	private float activateTime;
	private bool buttonsActive = false;
    private GameObject uiObject;

	private bool singleplayer = true;

    private bool initialized = false;

	// Use this for initialization
	void Awake () {
        globalMenu = GetComponent<GlobalMenu>();
		netView = GetComponent<NetworkView>();

        uiObject = GameObject.Find("Game Lobby");
		for(int i = 0; i < 4; i++)
		{
			playerNameHeroes[i] = uiObject.transform.FindChild ("Hero"+(i)).GetComponent<Text>();
			playerNameHeroes[i].text = "Available";
		}

        Transform teams = uiObject.transform.FindChild("Teams");
        teamHolders = new RectTransform[2];
        joinTeamButtons = new Button[2];

        for (int i = 0; i < 2; i++)
        {
            teamHolders[i] = teams.FindChild("Team"+i).GetComponent<RectTransform>();
            joinTeamButtons[i] = teamHolders[i].FindChild("Join").GetComponent<Button>();
            int team = i;
            joinTeamButtons[i].onClick.AddListener(() => buttonJoinTeam(team)); 
        }


        uiObject.SetActive(false);
	}

	public void init(bool singleplayer)
	{
        if (initialized) return;

        if (GlobalMenu.playerName == null || GlobalMenu.playerName.Trim().Equals(""))
        {
            GlobalMenu.randomizePlayerName();
        }

		this.singleplayer = singleplayer;
        if (NetworkMaster.isOnline())
        {
            netView.RPC("onPlayerConnected", RPCMode.All, Network.player, GlobalMenu.playerName);
        }
        else
        {
            addPlayer(Network.player, GlobalMenu.playerName, 0);
        }
		//NetworkMaster.connect();
		activateTime = Time.time + 1;
        initialized = true;
	}

    public void onLeave()
    {
        uiObject.SetActive(false);
        buttonsActive = false;
        initialized = false;
    }

	
	// Update is called once per frame
	void Update () {

		if(!buttonsActive && Time.time >= activateTime)
		{
            uiObject.SetActive(true);
			buttonsActive = true;
		}

		if(Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
		
		{
			RaycastHit hit;
			if( Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				string tag = hit.collider.tag;
				if(tag == "Unit")
				{
					MenuUnit unit = hit.transform.GetComponent<MenuUnit>();
					if(unit != null)
					{
						if(!singleplayer) netView.RPC ("requestSelectHero", RPCMode.Server, unit.id, int.Parse(Network.player.ToString()));
						else selectHeroSingleplayer(unit.id);
						unit.onSelect();
					}
				}
			}	
		}
	}

	public void buttonStart()
	{
		if(Network.isServer)
		{
			netView.RPC("startGameMultiplayer", RPCMode.All, Random.seed);
		}
		else if(singleplayer)
		{
			startGame();
		}
	}

	public void buttonLeave()
	{
        NetworkMaster.disconnect();
        globalMenu.goToMenu();
	}

	public void buttonHero(int index)
	{
		netView.RPC ("requestSelectHero", RPCMode.Server, index, int.Parse(Network.player.ToString()));
	}

    public void buttonJoinTeam(int team)
    {
        if (NetworkMaster.isOnline())
        {
            netView.RPC("requestJoinTeam", RPCMode.Server, NetworkMaster.getMe().getID(), team);
        }
        else
        {
            approveJoinTeam(NetworkMaster.getMe().getID(), team);
        }
    }

    [RPC]
    void requestJoinTeam(int playerID, int team)
    {
        netView.RPC("approveJoinTeam", RPCMode.All, playerID, team);
    }

    [RPC]
    void approveJoinTeam(int playerID, int team)
    {
        NetworkMaster.findPlayer(playerID).setTeam(team);
        updateTeamDisplay();

        if (playerID == NetworkMaster.getMe().getID())
        {
            for (int i = 0; i < joinTeamButtons.Length; i++)
                joinTeamButtons[i].interactable = i != team;
        }
    }

	void selectHeroSingleplayer(int index)
	{
        if (index >= GameMaster.heroNames.Length) return;
		for(int i = 0; i < playerNameHeroes.Length; i++)
		{
			if(i != index)
			{
                playerNameHeroes[i].text = "<color=white>Available</color>";
			}
			else
			{
                playerNameHeroes[i].text = "<color=#fcfc11ff>Selected</color>";
			}
		}
        NetworkMaster.getMe().setHero(index);

        updateTeamDisplay();
	}

	[RPC]
	void requestSelectHero(int index, int playerID)
	{
        if (index >= GameMaster.heroNames.Length) return;
		if(!NetworkMaster.isServer()) netView.RPC("requestSelectHero", RPCMode.Server, index, playerID);
        else netView.RPC("selectHero", RPCMode.All, index, playerID);
	}

	[RPC]
	void selectHero(int index, int playerID)
	{
        OnlinePlayer player = NetworkMaster.findPlayer(playerID);
        if (player != null)
        {
            player.setHero(index);

            updateTeamDisplay();

            if (playerID == NetworkMaster.getMe().getID())
            {
                for (int i = 0; i < playerNameHeroes.Length; i++)
                {
                    if (i != index)
                    {
                        playerNameHeroes[i].text = "<color=white>Available</color>";
                    }
                    else
                    {
                        playerNameHeroes[i].text = "<color=#fcfc11ff>Selected</color>";
                    }
                }
            }
        }
        /*if(playerID == -1)
		{
			playerNameHeroes[index].text = "<color=white>Available</color>";
		}
		else
		{
            string colorcode = "<color=#1111d6ff>";
            if (playerID == NetworkMaster.getMyPlayerID())
            {
                colorcode = "<color=#f6f622ff>";
            }
            playerNameHeroes[index].text = colorcode + NetworkMaster.getPlayerName(int.Parse(playerID.ToString())) + "</color>";
		}*/
	}

	[RPC]
	void startGameMultiplayer(int worldSeed)
	{
		World.seed = worldSeed;
		for(int i = 0; i < 4; i++)
		{
			//if(selectedHeroes[i] == -1) selectedHeroes[i] = 100000 + i;
			//GameMaster.playerToUnitID.Add(selectedHeroes[i], i);
		}
		Application.LoadLevel("world");
	}

	void startGame()
	{
		World.seed = Random.seed;
		for(int i = 0; i < 4; i++)
		{
			//if(selectedHeroes[i] != -1)
			//	GameMaster.playerToUnitID.Add(selectedHeroes[i], i);
		}
		Application.LoadLevel ("world");
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
        globalMenu.addChatMessage(NetworkMaster.getPlayerName(int.Parse(player.ToString())) + " disconnected."); 
        NetworkMaster.removePlayer(player);
	}

    [RPC]
    void onPlayerConnected(NetworkPlayer player, string name)
    {
        globalMenu.addChatMessage(name + " has connected.");
        int team = 0;


        //Add all connected players to the newly connected player
        if (Network.isServer)
        {
            if (player != Network.player)
            {
                List<OnlinePlayer> players = NetworkMaster.getAllPlayers();
                foreach (OnlinePlayer op in players)
                {
                    netView.RPC("addPlayer", player, op.getNetworkPlayer(), op.getName(), op.getTeam());
                }

                //Everyone adds the new player
                netView.RPC("addPlayer", RPCMode.All, player, name, team);
            }
            else
            {
                //Servers adds itself
                addPlayer(player, name, team);
            }
        }
    }
    [RPC]
    void addPlayer(NetworkPlayer player, string name, int team)
    {
        NetworkMaster.addPlayer(player, name, team);
        updateTeamDisplay();
        if (NetworkMaster.getMe() != null && player == NetworkMaster.getMe().getNetworkPlayer())
        {
            for (int i = 0; i < joinTeamButtons.Length; i++)
                joinTeamButtons[i].interactable = i != team;
        }
    }

    void updateTeamDisplay()
    {
        for (int i = 0; i < 2; i++)
        {
            List<OnlinePlayer> players = NetworkMaster.getTeamPlayers(i);
            List<Text> teamNames = teamPlayerNames[i];

            for (int j = 0; j < teamNames.Count; j++)
            {
                if (j >= players.Count)
                {
                    Destroy(teamNames[j].gameObject);
                    teamNames.RemoveAt(j);
                    j--;
                    continue;
                }

                teamNames[j].text = players[j].getName();
                int hero = players[j].getHero();
                if (hero >= 0 && hero < GameMaster.heroNames.Length)
                    teamNames[j].text += " (" + GameMaster.heroNames[hero] + ")";
            }

            while (teamNames.Count < players.Count)
            {
                GameObject prefab = Resources.Load<GameObject>("GUI/LobbyPlayerText");
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(teamHolders[i]);

                Text text = go.GetComponent<Text>();
                text.rectTransform.anchoredPosition = new Vector2(0, (teamNames.Count + 1) * -25);
                text.rectTransform.localScale = new Vector3(1, 1, 1);

                text.text = players[teamNames.Count].getName();

                int hero = players[teamNames.Count].getHero();
                if (hero >= 0 && hero < GameMaster.heroNames.Length)
                    text.text += " ("+ GameMaster.heroNames[hero] + ")";

                teamNames.Add(text);
            }
        }

        teamHolders[1].anchoredPosition = new Vector2(0, -(40 + 25 * teamPlayerNames[0].Count)); 
    }
	
	void OnServerInitialized()
	{
		Debug.Log ("Server Initialized!");
	}
	
	void OnMasterServerEvent(MasterServerEvent e)
	{
		if(e == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log ("Server Registered");
		}
		if(e == MasterServerEvent.RegistrationFailedGameName)
		{
			Debug.LogWarning("Registration failed: Game name");
		}
		if(e == MasterServerEvent.RegistrationFailedGameType)
		{
			Debug.LogWarning("Registration failed: Game type name");
		}
		if(e == MasterServerEvent.RegistrationFailedNoServer)
		{
			Debug.LogWarning("Registration failed: No server");
		}
	}
}
