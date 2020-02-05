﻿using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
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

            helper.Events.Display.MenuChanged += Display_MenuChanged;
        }


        /// <summary>
        /// When the player brings up the menu, replace the SocialPage with our own
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            Log($"MenuEvents_MenuChanged {e.NewMenu}");

            if (e.NewMenu is GameMenu menu)
            {
                var pages = this.Helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue();
                var socialPage = pages.FirstOrDefault(p => p is StardewValley.Menus.SocialPage);
                if (socialPage != null)
                {
                    var indexOfSocialPage = pages.IndexOf(socialPage);
                    pages[indexOfSocialPage] = new SocialPage(this, socialPage.xPositionOnScreen, socialPage.yPositionOnScreen, socialPage.width, socialPage.height);
                }
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