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

namespace NoFishingMinigame
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            Log($"MenuEvents_MenuChanged {e.NewMenu}");
            Log($"Game1.player.CurrentTool {Game1.player.CurrentTool}");
            Log($"Game1.dailyLuck {Game1.dailyLuck}");

            if (e.NewMenu is BobberBar bobberBar && Game1.player.CurrentTool is FishingRod fishingRod)
            {
                var whichFish = this.Helper.Reflection.GetField<int>(bobberBar, "whichFish").GetValue();
                var fishSize = this.Helper.Reflection.GetField<int>(bobberBar, "fishSize").GetValue();
                var fishQuality = this.Helper.Reflection.GetField<int>(bobberBar, "fishQuality").GetValue();
                var difficulty = this.Helper.Reflection.GetField<float>(bobberBar, "difficulty").GetValue();
                var perfect = this.Helper.Reflection.GetField<bool>(bobberBar, "perfect").GetValue();
                var treasure = this.Helper.Reflection.GetField<bool>(bobberBar, "treasure").GetValue();

                Log($"whichFish         {whichFish}", LogLevel.Trace);
                Log($"fishSize          {fishSize}", LogLevel.Trace);
                Log($"fishQuality       {fishQuality}", LogLevel.Trace);
                Log($"difficulty        {difficulty}", LogLevel.Trace);
                Log($"perfect           {perfect}", LogLevel.Trace);
                Log($"treasure           {treasure}", LogLevel.Trace);

                var treasureCaught = true;
                fishingRod.pullFishFromWater(whichFish, fishSize, fishQuality, (int) difficulty, treasureCaught, perfect);
                Game1.exitActiveMenu();
                Game1.setRichPresence("location", (object) Game1.currentLocation.Name);
            }
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
