using System.Collections;
using System.Linq;
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using DewMods.StardewValleyMods.PhoneVillagers.Menus;
using StardewValley.Menus;
using SocialPage = DewMods.StardewValleyMods.PhoneVillagers.Menus.SocialPage;

namespace DewMods.StardewValleyMods.PhoneVillagers
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
        }

        /// <summary>
        /// When the player brings up the menu, replace the SocialPage with our own
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            this.Monitor.Log($"MenuEvents_MenuChanged {e.NewMenu}");

            if (e.NewMenu is GameMenu menu)
            {
                var pages = this.Helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue();
                var socialPage = pages.FirstOrDefault(p => p is StardewValley.Menus.SocialPage);
                if (socialPage != null)
                {
                    var indexOfSocialPage = pages.IndexOf(socialPage);
                    pages[indexOfSocialPage] = new Menus.SocialPage(this, socialPage.xPositionOnScreen, socialPage.yPositionOnScreen, socialPage.width, socialPage.height);
                }
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.");

            switch (e.Button)
            {
                default:
                    break;
            }
        }
    }
}