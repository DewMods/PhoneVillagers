using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using DewMods.StardewValleyMods.Common.Enums;

namespace DewMods.StardewValleyMods.SkipFishingMinigame
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            Log($"MenuEvents_MenuChanged {e.NewMenu}");
            Log($"Game1.player.CurrentTool {Game1.player.CurrentTool}");

            if (e.NewMenu is BobberBar bobberBar && Game1.player.CurrentTool is FishingRod fishingRod)
            {
                var whichFish = this.Helper.Reflection.GetField<int>(bobberBar, "whichFish").GetValue();
                var fishSize = this.Helper.Reflection.GetField<int>(bobberBar, "fishSize").GetValue();
                var fishQuality = this.Helper.Reflection.GetField<int>(bobberBar, "fishQuality").GetValue();
                var difficulty = this.Helper.Reflection.GetField<float>(bobberBar, "difficulty").GetValue();
                var perfect = this.Helper.Reflection.GetField<bool>(bobberBar, "perfect").GetValue();
                var treasure = this.Helper.Reflection.GetField<bool>(bobberBar, "treasure").GetValue();

                Game1.player.fishCaught.TryGetValue(whichFish, out var whichFishCaughtTimes);
                Log($"whichFish       [{whichFish}]. Times caught [{whichFishCaughtTimes?[0] + 1}]. Largest [{whichFishCaughtTimes?[1]}].");
                Log($"fishSize        [{fishSize}]");
                Log($"fishQuality     [{fishQuality}]");
                Log($"difficulty      [{difficulty}]");
                Log($"perfect         [{perfect}]");
                Log($"treasure        [{treasure}]");

                // We always catch the treasure if one was spawned
                var treasureCaught = treasure;
                fishingRod.pullFishFromWater(whichFish, fishSize, fishQuality, (int)difficulty, treasureCaught, perfect);

                Game1.exitActiveMenu();
                Game1.setRichPresence("location", (object)Game1.currentLocation.Name);
            }
        }

        /// <summary>
        /// Increase the chance for spawning treasure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.MouseLeft && Game1.player.CurrentTool is FishingRod fishingRod && Game1.player.CanMove)
            {
#if DEBUG
                // Output Fishing EXP
                var fishingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Fishing];
                Log($"Fishing Experience [{fishingExp}]");

                // TESTING Higher chance of treasure
                Game1.player.LuckLevel = 100;
                Log($"OVERRIDING Game1.player.LuckLevel: [{Game1.player.LuckLevel}]", LogLevel.Alert);
#endif
            }

            //var oldFarmingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Farming];
            //var oldFishingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Fishing];
            //var oldMiningExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Mining];
            //var oldCombatExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Combat];
            //var oldUnknownExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.UNKNOWN];
            //var oldForagingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Foraging];

            //var newFarmingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Farming];
            //Log($"Farming Experience [{oldFarmingExp}] -> [{newFarmingExp}] (+{newFarmingExp - oldFarmingExp})");

            //var newFishingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Fishing];
            //Log($"Fishing Experience [{oldFishingExp}] -> [{newFishingExp}] (+{newFishingExp - oldFishingExp})");

            //var newMiningExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Mining];
            //Log($"Mining Experience [{oldMiningExp}] -> [{newMiningExp}] (+{newMiningExp - oldMiningExp})");

            //var newCombatExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Combat];
            //Log($"Combat Experience [{oldCombatExp}] -> [{newCombatExp}] (+{newCombatExp - oldCombatExp})");

            //var newUnknownExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.UNKNOWN];
            //Log($"UNKNOWN Experience [{oldUnknownExp}] -> [{newUnknownExp}] (+{newUnknownExp - oldUnknownExp})");

            //var newForagingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Foraging];
            //Log($"Foraging Experience [{oldForagingExp}] -> [{newForagingExp}] (+{newForagingExp - oldForagingExp})");
        }

        /// <summary>
        /// Only log if build is DEBUG
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        internal void Log(string message, LogLevel logLevel = LogLevel.Debug)
        {
#if DEBUG
            this.Monitor.Log(message, logLevel);
#endif
        }
    }
}
