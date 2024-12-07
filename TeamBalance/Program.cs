using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace PlayerController
{
    [MinimumApiVersion(247)]
    public class PlayerController : BasePlugin
    {
        public override string ModuleAuthor => "ZIRA";
        public override string ModuleName => "[Discord] Player Controller";
        public override string ModuleVersion => "v3.0";
        public override void Load(bool hotReload)
        {
            Logger.LogInformation($"Плагин загружен");
            AddCommandListener("jointeam", (player, c) =>
            {
                Console.WriteLine($"Захватил смену команды от игрока {player.PlayerName}");
                SwitchTeams();
                return HookResult.Handled;
            });
        }
        [GameEventHandler]
        public HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            SwitchTeams();
            return HookResult.Continue;
        }
        public HookResult OnEventSwitchTeams(EventSwitchTeam @event, GameEventInfo info)
        {
            SwitchTeams();
            return HookResult.Continue;
        }
        public static void SwitchTeams()
        {
            var players = GetOnlinePlayers();

            foreach (var player in players)
            {
                if (player.SteamID != 76561198162988388)
                {
                    if (player.Team != CounterStrikeSharp.API.Modules.Utils.CsTeam.Terrorist)
                    {
                        player.SwitchTeam(CounterStrikeSharp.API.Modules.Utils.CsTeam.Terrorist);
                    }
                }
                if (player.SteamID == 76561198162988388)
                {
                    player.SwitchTeam(CounterStrikeSharp.API.Modules.Utils.CsTeam.CounterTerrorist);
                }
            }
        }
        public static List<CCSPlayerController> GetOnlinePlayers(bool getBots = false)
        {
            var players = Utilities.GetPlayers();

            List<CCSPlayerController> validPlayers = new List<CCSPlayerController>();

            foreach (var p in players)
            {
                if (!p.IsValid) continue;
                if (p.AuthorizedSteamID == null && !getBots) continue;
                //if (p.IsBot && !getBots) continue;
                if (p.Connected != PlayerConnectedState.PlayerConnected) continue;
                validPlayers.Add(p);
            }

            return validPlayers;
        }
    }
}