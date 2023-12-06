using Middleman_Game;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Program
{
    static void Main()
    {
        GameInfo gameInfo = GameInfo.Instance; // Get Singleton

        GameController.initializeGameParameters(gameInfo);

        StateMachine.startStateMachine(gameInfo);
    }
}