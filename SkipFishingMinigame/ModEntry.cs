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
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Display.MenuChanged += Display_MenuChanged;
        }


        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            Log($"MenuEvents_MenuChanged {e.NewMenu}");
            Log($"Game1.player.CurrentTool {Game1.player.CurrentTool}");
            Log($"Is Festival [{Game1.isFestival()}]");
            Log($"Is Event Up [{Game1.eventUp}]");

            if (e.NewMenu is BobberBar bobberBar
                    && Game1.player.CurrentTool is FishingRod fishingRod)
            {
                var whichFish = this.Helper.Reflection.GetField<int>(bobberBar, "whichFish").GetValue();
                var fishSize = this.Helper.Reflection.GetField<int>(bobberBar, "fishSize").GetValue();
                var fishQuality = this.Helper.Reflection.GetField<int>(bobberBar, "fishQuality").GetValue();
                var difficulty = this.Helper.Reflection.GetField<float>(bobberBar, "difficulty").GetValue();
                var perfect = this.Helper.Reflection.GetField<bool>(bobberBar, "perfect").GetValue();
                var treasureCaught = this.Helper.Reflection.GetField<bool>(bobberBar, "treasure").GetValue();
                var fromFishPond = this.Helper.Reflection.GetField<bool>(bobberBar, "fromFishPond").GetValue();
                var bossFish = this.Helper.Reflection.GetField<bool>(bobberBar, "bossFish").GetValue();

                int num = Game1.player.CurrentTool == null || !(Game1.player.CurrentTool is FishingRod) || (Game1.player.CurrentTool as FishingRod).attachments[0] == null ?
                    -1 :
                    (Game1.player.CurrentTool as FishingRod).attachments[0].ParentSheetIndex;
                bool caughtDouble = !bossFish && num == 774 && Game1.random.NextDouble() < 0.25 + Game1.player.DailyLuck / 2.0;

                Game1.player.fishCaught.TryGetValue(whichFish, out var whichFishCaughtTimes);
                Log($"whichFish       [{whichFish}]. Times caught [{whichFishCaughtTimes?[0] + 1}]. Largest [{whichFishCaughtTimes?[1]}].");
                Log($"fishSize        [{fishSize}]");
                Log($"fishQuality     [{fishQuality}]");
                Log($"difficulty      [{difficulty}]");
                Log($"perfect         [{perfect}]");
                Log($"treasureCaught  [{treasureCaught}]");
                Log($"fromFishPond    [{fromFishPond}]");
                Log($"bossFish        [{bossFish}]");
                Log($"caughtDouble    [{caughtDouble}]");

                // Perfect Catch during Fall Festival fishing minigame
                if (Game1.isFestival())
                { Game1.CurrentEvent.perfectFishing(); }

                // We always catch the treasure if one was spawned
                fishingRod.pullFishFromWater(whichFish, fishSize, fishQuality, (int)difficulty, treasureCaught, perfect, fromFishPond, caughtDouble);

                Game1.exitActiveMenu();
                Game1.setRichPresence("location", (object)Game1.currentLocation.Name);
            }
        }

        /// <summary>
        /// Increase the chance for spawning treasure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
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
            var oldFishingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Fishing];
            //var oldMiningExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Mining];
            //var oldCombatExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Combat];
            //var oldUnknownExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.UNKNOWN];
            //var oldForagingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Foraging];

            //var newFarmingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Farming];
            //Log($"Farming Experience [{oldFarmingExp}] -> [{newFarmingExp}] (+{newFarmingExp - oldFarmingExp})");

            var newFishingExp = Game1.player.experiencePoints[(int)Player.ExperiencePointsIndex.Fishing];
            Log($"Fishing Experience [{oldFishingExp}] -> [{newFishingExp}] (+{newFishingExp - oldFishingExp})");

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
