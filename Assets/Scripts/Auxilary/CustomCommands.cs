using SickDev.CommandSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Auxilary
{
    internal class CustomCommands
    {
        static void Main(string[] args)
        {
            Configuration configuration = new Configuration(false, "MauSquared");
            CommandsManager manager = new CommandsManager(configuration);

            Command addCardCommand =new ActionCommand<string, bool>(GameManager.AddCardToEntity);
            Command addSideCardCommand = new ActionCommand<string, bool>(GameManager.AddSideCardToEntity);
            Command WinRoundCommand = new ActionCommand(GameManager.WinRound);
            Command DrawCardsCommand = new ActionCommand<int, bool>(GameManager.DrawCards);
            Command AddPlayerMoneyCommand = new ActionCommand<int>(GameManager.AddPlayerMoney);
            Command AddOpponentMoneyCommand = new ActionCommand<int>(GameManager.AddOpponentMoney);
            Command ResetShopCommand = new ActionCommand(GameManager.ResetShop);
            Command RevealOpHandCommand = new ActionCommand(GameManager.RevealOpHand);
            Command AddScoreCommand = new ActionCommand<int>(GameManager.AddScore);

            manager.Add(addCardCommand);
            manager.Add(addSideCardCommand);
            manager.Add(WinRoundCommand);
            manager.Add(DrawCardsCommand);
            manager.Add(AddPlayerMoneyCommand);
            manager.Add(AddOpponentMoneyCommand);
            manager.Add(ResetShopCommand);
            manager.Add(RevealOpHandCommand);
            manager.Add(AddScoreCommand);

            manager.LoadCommands();
        }
    }
}
