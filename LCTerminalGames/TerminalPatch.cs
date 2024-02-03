using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;

namespace LCTerminalGames
{
    public class TerminalGame
    {
        public string name;
        public string description;
        public float tickSpeed;
        public Action<Action<string>> runGame;
        public Action startGame;
        public Action<KeyCode> handleKey;

        public TerminalGame(string name, string description, float tickSpeed, Action<Action<string>> runGame, Action startGame, Action<KeyCode> handleKey)
        {
            this.name = name;
            this.description = description;
            this.tickSpeed = tickSpeed;
            this.runGame = runGame;
            this.startGame = startGame;
            this.handleKey = handleKey;
        }
    }
    [HarmonyPatch(typeof(Terminal), "Start", MethodType.Normal)]
    public class TerminalGames
    {
        public static List<TerminalGame> games = new List<TerminalGame>();
        internal static void Postfix(Terminal __instance)
        {
            Plugin.terminalInstance = __instance;

            string gameText = "";
            foreach (TerminalGame game in games)
            {
                gameText += $"{game.name}\n{game.description}\n\n";
                AddKeyword(__instance, game.name.ToLower(), game.name, "Loading Game...");
            }

            AddKeyword(__instance, "games", "tg_Games", gameText);
        }

        private static void AddKeyword(Terminal __instance, string keyword, string name, string displayText)
        {
            TerminalKeyword termKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            termKeyword.word = keyword;
            
            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.name = name;
            node.displayText = displayText;
            node.clearPreviousText = true;
            node.terminalEvent = "";

            termKeyword.specialKeywordResult = node;
            __instance.terminalNodes.allKeywords = __instance.terminalNodes.allKeywords.AddToArray(termKeyword);
        }
    }

    [HarmonyPatch(typeof(TMP_InputField), "KeyPressed", MethodType.Normal)]
    internal class TerminalSnake
    {
        internal static bool Prefix(TMP_InputField __instance, Event evt)
        {
            foreach (TerminalGame game in TerminalGames.games)
            {
                if (Plugin.terminalInstance.currentNode.name == game.name + "_started")
                {
                    game.handleKey(evt.keyCode);
                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Terminal), "LoadNewNode", MethodType.Normal)]
    internal class TerminalLoadNewNodePatch
    {
        public static KeyCode keyCode = KeyCode.None;
        internal static bool Prefix(Terminal __instance, TerminalNode node)
        {
            Plugin.terminalInstance = __instance;

            if (node.name == "HelpCommands" && !node.displayText.Contains("GAMES"))
            {
                node.displayText = node.displayText.Replace("[numberOfItemsOnRoute]",
                    ">GAMES\nPlay games while on the ship!\n[numberOfItemsOnRoute]");
            }
            else
            {
                foreach (TerminalGame game in TerminalGames.games)
                {
                    if (node.name == game.name)
                    {
                        Plugin.pluginInstance.Log($"Starting Game: {game.name}");
                        node.name += "_started";
                        node.displayText = "";
                        Plugin.terminalInstance.LoadNewNode(node);
                        game.startGame();
                        __instance.StartCoroutine(GameTicks(game.runGame, game.tickSpeed, node.name));
                    }
                }
            }

            return true;
        }

        private static IEnumerator GameTicks(Action<Action<string>> runGame, float gameTicks, string gameName)
        {
            while (Plugin.terminalInstance.currentNode.name == gameName)
            {
                runGame(SetText);
                yield return new WaitForSeconds(gameTicks);
            }
        }

        private static void SetText(string text)
        {
            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.name = Plugin.terminalInstance.currentNode.name;
            node.displayText = text;
            node.clearPreviousText = true;
            node.terminalEvent = "";
            
            Plugin.pluginInstance.Log(node.name);
                
            Plugin.terminalInstance.LoadNewNode(node);
        }
    }
}
