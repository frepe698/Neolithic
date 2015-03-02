using UnityEngine;
using System.Collections;
using System;

public class CheatCommand {

    private static readonly string[] commands = 
    {
        "spawn",
        "give",
        "warp",
    };

    public const int SPAWN = 0;
    public const int GIVE = 1;
    public const int WARP = 2;

    public static void sendCommandFromString(string str)
    {
        string[] substrings = str.Split(' ');

        int commandID = Array.IndexOf<string>(commands, substrings[0]);

        sendCommand(commandID, str.Substring(substrings[0].Length));
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
        }

        return null;
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
