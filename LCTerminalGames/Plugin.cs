using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LCTerminalGames
{
    public class ModInfo
    {
        public const string Name = "Terminal Games";
        public const string Guid = "jacktym.terminalgames";
        public const string Version = "1.0.0";
    }

    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmonyInstance = new Harmony(ModInfo.Guid);
        public static Terminal terminalInstance;
        public static Plugin pluginInstance;

        async Task<byte[]> DownloadDataAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetByteArrayAsync(url);
            }
        }

        private void Awake()
        {
            pluginInstance = this;
            Logger.LogInfo($"Plugin {ModInfo.Guid} is loaded!");
            TerminalGames.games.Add(new TerminalGame(
                "Snake",
                "Retro nokia game of Snake",
                0.2f,
                Snake.RunGame,
                Snake.StartGame,
                Snake.HandleKey
            ));
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Log(string log)
        {
            Logger.LogInfo(log);
        }
    }
}