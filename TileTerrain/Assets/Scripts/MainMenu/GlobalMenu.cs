using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlobalMenu : MonoBehaviour {

    public static string playerName = "";

    public Transform canvas;
    //Chat
    private GameObject chatObject;
    private GameObject chatInputObject;
    private InputField chatInputField;
    private Text chatOutputText;
    private float chatOutputHeight = 0;
    private Scrollbar chatScrollbar;

    private NetworkView netView;

    private MainMenu mainMenu;
    private GameLobby gameLobby;


	void Awake ()
    {
        mainMenu = GetComponent<MainMenu>();
        gameLobby = GetComponent<GameLobby>();
        gameLobby.enabled = false;
        netView = GetComponent<NetworkView>();
        initUI();
	}

    void Start()
    {
		GetComponent<AudioSource>().Play ();

    }

    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            toggleChatInput();
        }

        chatInputField.text = chatInputField.text.Replace("\n", "").Replace("\r", "");

        /*if (chatting)
        {
            if (justOpenedChat)
            {
                chatInputField.ActivateInputField();
                chatInputField.Select();
                justOpenedChat = false;
            }
            chatInputField.text = chatInputField.text.Replace("\n", "").Replace("\r", "");
            chatObject.SetActive(true);
            chatOutputOpen = true;
            chatInputObject.SetActive(true);
        }
        else
        {
            chatInputField.DeactivateInputField();
            chatInputObject.SetActive(false);

            if (Time.time >= chatCloseTime)
            {
                chatObject.SetActive(false);
                chatOutputOpen = false;
            }
        }

        if (chatOutputOpen)
        {
            
        }*/
        float newHeight = chatOutputText.rectTransform.sizeDelta.y;
        if (newHeight > chatOutputHeight)
        {
            chatScrollbar.value = 0;
            chatOutputHeight = newHeight;
        }
    }

    void initUI()
    {
        canvas = GameObject.Find("MainCanvas").transform;
        //Chat
        chatObject = canvas.FindChild("Chat").gameObject;
        chatInputObject = chatObject.transform.FindChild("ChatInput").gameObject;
        chatInputField = chatInputObject.GetComponent<InputField>();
        chatOutputText = chatObject.transform.FindChild("ChatOutputScrollmask").FindChild("ChatOutput").GetComponent<Text>();
        chatScrollbar = chatObject.transform.FindChild("Scrollbar").GetComponent<Scrollbar>();
    }

    public void goToLobby(bool singleplayer)
    {
        gameLobby.enabled = true;
        gameLobby.init(singleplayer);
        GetComponent<Animator>().SetTrigger("gotoLobby");
        mainMenu.enabled = false;
    }

    public void goToMenu()
    {
        mainMenu.enabled = true;
        gameLobby.onLeave();
        gameLobby.enabled = false;
        GetComponent<Animator>().SetTrigger("gotoMainMenu");
    }

    public void addChatMessage(string msg)
    {
        chatOutputText.text += msg + "\n";
    }

    public void toggleChatInput()
    {
        //if (chatting)
        //{
            //send message
            if (chatInputField.text.StartsWith("!setname"))
            {
                changeName(chatInputField.text.Substring("!setname".Length).Trim());
            }
            else if (chatInputField.text.Equals("!leave"))
            {
                goToMenu();
            }
            else if (!chatInputField.text.Trim().Equals(""))
            {
                sendChatMessage(chatInputField.text);
            }

            chatInputField.text = "";
            chatInputField.ActivateInputField();
            chatInputField.Select();

        //}
        //else
        //{
        //   chatting = true;
        //    chatObject.SetActive(true);
        //    chatInputObject.SetActive(true);
        //    justOpenedChat = true;
        //}

    }

    private void changeName(string name)
    {
        playerName = name;
        if (Network.isClient || Network.isServer)
        {
            netView.RPC("recieveNameChange", RPCMode.All, NetworkMaster.getPlayerID(), name);
        }
        else
        {
            addChatMessage("You changed name to " + name + ".");
        }
    }

    private void sendChatMessage(string msg)
    {
        if (Network.isClient || Network.isServer)
        {
            netView.RPC("recieveChatMessage", RPCMode.All, NetworkMaster.getPlayerID(), msg);
        }
        else
        {
            addChatMessage(playerName + ": " + msg);
        }
    }

    [RPC]
    public void recieveNameChange(int player, string name)
    {
        addChatMessage(NetworkMaster.getPlayerName(player) + " changed name to " + name + ".");
        NetworkMaster.changePlayerName(player, name);
    }

    [RPC]
    public void recieveChatMessage(int player, string msg)
    {
        addChatMessage(NetworkMaster.getPlayerName(player) + ": " + msg);
    }

    public static void randomizePlayerName()
    {
        string[] randomNames = new string[]
        {
            "Sven", "Olaf", "Uthred", "Ragnar", "Rolle", "Tor", "Oden", "Loke", "Best Vrodle EU", "Best Vrodl NA", "Best Vrodl Afrika", "Trolle", "Floki", 
        };

        playerName = randomNames[Random.Range(0, randomNames.Length)];
    }

    

}
