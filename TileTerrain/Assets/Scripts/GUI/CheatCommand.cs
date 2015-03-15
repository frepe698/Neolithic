using UnityEngine;
using System.Collections;
using System;

public class CheatCommand {
    private const string helpString = "Commands:\n!spawn (string aiUnitName)\n!give (string itemName, int amount = 1)\n!warp (float xpos, float ypos)"; 
    private static readonly string[] commands = 
    {
        "spawn",
        "give",
        "warp",
        "help",
        "addxp"
    };

    public const int SPAWN = 0;
    public const int GIVE = 1;
    public const int WARP = 2;
    public const int HELP = 3;
    public const int ADDXP = 4;

    public static void sendCommandFromString(string str)
    {
        string[] substrings = str.Split(' ');

        int commandID = Array.IndexOf<string>(commands, substrings[0]);
        
        if (commandID >= 0)
        {
            if (commandID == HELP)
            {
                GameMaster.getGameController().recieveChatMessage(-1, helpString);
            }
            else
            {
                string paramstring = str.Substring(substrings[0].Length);
                object[] parameters = getParametersFromString(commandID, paramstring);
                if (parameters == null)
                {
                    GameMaster.getGameController().recieveChatMessage(-1, "Invalid parameters!");
                }
                else if (paramsAreValid(commandID, parameters))
                {
                    sendCommand(commandID, paramstring);
                }
            }
        }
        else
        {
            GameMaster.getGameController().recieveChatMessage(-1, "Invalid command: " + substrings[0] + ", type !help to show all commands.");
        }
        
    }
    public static object[] getParametersFromString(int commandID, string parameters)
    {
        string[] substrings = parameters.Split(' ');
        
        switch (commandID)
        {
            case (SPAWN):
                {
                    if (substrings.Length > 1)  return new object[] { substrings[1]};
                }
                break;
            case (GIVE):
                {
                    if (substrings.Length > 2)
                    {
                        int amount;
                        if (int.TryParse(substrings[2], out amount)) return new object[] { substrings[1], amount};
                    }
                    else if (substrings.Length > 1)
                    {
                        return new object[] { substrings[1] };
                    }
                }
                break;
            case (WARP):
                {
                    if (substrings.Length > 2)
                    {
                        float x, y;
                        if (float.TryParse(substrings[1], out x) && float.TryParse(substrings[2], out y))
                        {
                            return new object[] { x, y };
                        }
                    }
                }
                break;
            case (ADDXP):
                {
                    if (substrings.Length > 2)
                    {
                        int amount;
                        if (int.TryParse(substrings[2], out amount))
                        {
                            return new object[]{substrings[1], amount};
                        }
                    }
                }
                break;
        }

        return null;
    }

    public static bool paramsAreValid(int commandID, params object[] parameters)
    {
        switch (commandID)
        {
            case (SPAWN):
                {
                    if (parameters.Length != 1)
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Invalid parameters: string aiUnitName");
                        return false;
                    }
                    if (DataHolder.Instance.getAIUnitData((string)parameters[0]) != null) return true;
                    else
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Couldn't find unitdata for " + (string)parameters[0]);
                        return false;
                    }
                }
            case (GIVE):
                {
                    if (parameters.Length > 2)
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Invalid parameters: string itemName, int amount = 1");
                        return false;
                    }
                    if (DataHolder.Instance.getItemData((string)parameters[0]) == null)
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Couldn't find itemdata for " + (string)parameters[0]);
                        return false;
                    }
                    int i;
                    if (parameters.Length > 1 && !(int.TryParse(parameters[1].ToString(), out i)))
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Parameter 2 should be an int");
                        return false;
                    }
                    return true;

                }
            case (WARP):
                {
                    if (parameters.Length == 2)
                    {
                        float x, y;
                        if (!float.TryParse(parameters[0].ToString(), out x) || !float.TryParse(parameters[1].ToString(), out y))
                        {
                            GameMaster.getGameController().recieveChatMessage(-1, "Parameter 1 and 2 should be floats");
                            return false;
                        }
                    }
                    else
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Invalid parameters: float x, float y");
                        return false;
                    }
                    return true;
                }
            case (ADDXP):
                {
                    if (parameters.Length > 2)
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Invalid parameters: string skillname, int amount");
                        return false;
                    }
                    if (DataHolder.Instance.getSkillData((string)parameters[0]) == null)
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Couldn't find skilldata for " + (string)parameters[0]);
                        return false;
                    }
                    int i;
                    if (parameters.Length > 1 && !(int.TryParse(parameters[1].ToString(), out i)))
                    {
                        GameMaster.getGameController().recieveChatMessage(-1, "Parameter 2 should be an int");
                        return false;
                    }
                    return true;
                }
        }
        return false;
    }

    public static void sendCommand(int commandID, string parameters)
    {
        GameMaster.getGameController().requestCheatCommand(GameMaster.getPlayerUnitID(), commandID, parameters);
    }

    public static bool isValid(string command)
    {
        foreach(string c in commands)
        {
            if(command.Equals(c)) return true;
        }

        return false;
    }
}
