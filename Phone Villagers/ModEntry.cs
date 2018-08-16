using System.Collections;
using System.Linq;
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using PhoneVillagers.Menus;
using StardewValley.Menus;

namespace PhoneVillagers
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
                var socialPage = pages.FirstOrDefault(p => p is SocialPage);
                if (socialPage != null)
                {
                    var indexOfSocialPage = pages.IndexOf(socialPage);
                    pages[indexOfSocialPage] = new SocialPagePV(this, socialPage.xPositionOnScreen, socialPage.yPositionOnScreen, socialPage.width, socialPage.height);
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
                case SButton.F9:
                    foreach (var l in Game1.locations)
                    {
                        foreach (var npc in l.characters)
                        {
                            this.Monitor.Log($"NPC: {npc.getName()}. Location: {npc.currentLocation.Name}. Can Socialise: {npc.CanSocialize}. Can Talk: {npc.canTalk()}. Is Invisible {npc.IsInvisible}");
                        }
                    }
                    break;
                case SButton.F10:
                    if (Game1.activeClickableMenu is GameMenu menu)
                    {
                        var pages = this.Helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue();
                        if (pages[menu.currentTab] is SocialPage socialPage)
                        {
                            pages[menu.currentTab] = new SocialPagePV(this, socialPage.xPositionOnScreen, socialPage.yPositionOnScreen, socialPage.width, socialPage.height);
                        }
                    }

                    break;
                case SButton.F11:
                    var firstNpc = Game1.locations.FirstOrDefault(
                        l => l.characters.Count(c => c.isVillager() && c.canTalk()) > 0)?.characters.FirstOrDefault(
                        c => c.isVillager() && c.canTalk());

                    var who = Game1.player;

                    if (firstNpc != null)
                    {
                        // Get NPC dialogue
                        bool flag1 = false;
                        if (who.friendshipData.ContainsKey(firstNpc.Name))
                        {
                            flag1 = firstNpc.checkForNewCurrentDialogue(who.friendshipData[firstNpc.Name].Points,
                                false);
                            if (!flag1)
                                flag1 = firstNpc.checkForNewCurrentDialogue(who.friendshipData[firstNpc.Name].Points,
                                    true);
                        }

                        if (firstNpc.CurrentDialogue.Count == 0)
                        {
                            // NPC has no dialogue, no answer
                            Game1.drawDialogueNoTyping("*Dial tone*...");
                        }
                        else
                        {
                            // Display NPC dialogue
                            Game1.drawDialogue(firstNpc);
                        }
                    }

                    break;
                default:
                    break;
            }
        }
    }
}