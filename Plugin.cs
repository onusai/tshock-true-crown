using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;
using System.Text.Json;
using Terraria.GameContent.Creative;
using System.Diagnostics;
using IL.Terraria.DataStructures;
using Terraria.Net;
using IL.Terraria.Net;

namespace TrueCrown
{
    [ApiVersion(2, 1)]
    public class TrueCrown : TerrariaPlugin
    {

        public override string Author => "Onusai";
        public override string Description => "Forces players to consume a teleportation potion every X seconds";
        public override string Name => "TrueCrown";
        public override Version Version => new Version(1, 0, 0, 0);

        public class ConfigData
        {
            public bool Enabled { get; set; } = true;
            public int Interval { get; set; } = 120;
            public bool WarnPlayersBeforeTeleport { get; set; } = true;
            public Dictionary<int, int> GiveItems { get; set; } = new Dictionary<int, int>()
            {
                {4423, 8}
            };
        }

        ConfigData config;

        int TicksCountdown = 0;
        bool paused = false;

        public TrueCrown(Main game) : base(game) { }

        public override void Initialize()
        {
            config = PluginConfig.Load("TrueCrown");

            ServerApi.Hooks.GameInitialize.Register(this, OnGameLoad);
        }

        void OnGameLoad(EventArgs e)
        {
            if (!config.Enabled) return;
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);

            RegisterCommand("ttc", "tshock.admin", OnTTC, "Toggle True Crown (pause/resume random teleportation)");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnGameLoad);
                if (config.Enabled)
                {
                    ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                }
            }
            base.Dispose(disposing);
        }

        void RegisterCommand(string name, string perm, CommandDelegate handler, string helptext)
        {
            TShockAPI.Commands.ChatCommands.Add(new Command(perm, handler, name)
            { HelpText = helptext });
        }

        void OnGameUpdate(EventArgs args)
        {
            if (paused) return;

            TicksCountdown--;

            if (config.WarnPlayersBeforeTeleport && TicksCountdown == 300)
                TShock.Utils.Broadcast("[TrueCrown] Teleporting in 5s", Color.MediumPurple);

            if (TicksCountdown > 0) return;
            TicksCountdown = config.Interval * 60;

            foreach (TSPlayer player in TShock.Players)
            {
                if (player == null || player.Dead) continue;
                player.TPlayer.TeleportationPotion();

                foreach (var entry in config.GiveItems)
                {
                    player.GiveItem(entry.Key, entry.Value);
                }
            }
        }

        void OnTTC(CommandArgs args)
        {
            paused = !paused;

            if (paused)
            {
                TShock.Utils.Broadcast("[TrueCrown] Paused", Color.MediumPurple);
            }
            else
            {
                TShock.Utils.Broadcast("[TrueCrown] Resumed", Color.MediumPurple);
                TicksCountdown = config.Interval * 60;
            }
        }

        public static class PluginConfig
        {
            public static string filePath;
            public static ConfigData Load(string Name)
            {
                filePath = String.Format("{0}/{1}.json", TShock.SavePath, Name);

                if (!File.Exists(filePath))
                {
                    var data = new ConfigData();
                    Save(data);
                    return data;
                }

                var jsonString = File.ReadAllText(filePath);
                var myObject = JsonSerializer.Deserialize<ConfigData>(jsonString);

                return myObject;
            }

            public static void Save(ConfigData myObject)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(myObject, options);

                File.WriteAllText(filePath, jsonString);
            }
        }

    }
}
