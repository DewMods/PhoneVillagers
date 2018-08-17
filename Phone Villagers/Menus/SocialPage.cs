using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace DewMods.StardewValleyMods.PhoneVillagers.Menus
{
    /// <summary>
    /// Mod specific version of the SocialPage
    /// </summary>
    internal class SocialPage : StardewValley.Menus.SocialPage
    {
        private readonly ModEntry _mod;

        public SocialPage(ModEntry mod, int x, int y, int width, int height) : base(x, y, width, height)
        {
            _mod = mod;
        }

        /// <summary>
        /// Calls base receiveLeftClick logic, then adds PV functionality to detect clicks on NPCs
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            var sprites = _mod.Helper.Reflection.GetField<List<ClickableTextureComponent>>(this, "sprites")?.GetValue();
            _mod.Monitor.Log($"Found sprites {sprites?.Count(s => s.bounds.Contains(x, y))}");

            // Bug #4 - When clicking on the top displayed row, we get 2 results (first row + the clicked row)
            var clickedSprites = sprites?.LastOrDefault(s => s.bounds.Contains(x, y));
            if (clickedSprites != null)
            {
                var clickedIndex = sprites.IndexOf(clickedSprites);

                var names = _mod.Helper.Reflection.GetField<List<object>>(this, "names")?.GetValue();
                var clickedNpcName = names?[clickedIndex] as string;

                _mod.Monitor.Log($"You clicked on {clickedNpcName}");

                TalkToNpc(clickedNpcName);
            }
        }

        /// <summary>
        /// Initiate dialogue with the given NPC
        /// </summary>
        /// <param name="npcName"></param>
        private void TalkToNpc(string npcName)
        {
            var who = Game1.player;

            var npc = Game1.getCharacterFromName(npcName);
            if (npc != null)
            {
                var npcFriendshipBefore = who.tryGetFriendshipLevelForNPC(npcName);
                _mod.Monitor.Log(
                    $"{npc.Name} location: {npc.currentLocation.Name}. Friendship with {who.Name}: {npcFriendshipBefore}.");

                // Friendship check
                if (npcFriendshipBefore <= _mod.Config.MinimumFriendshipRequired)  // 250 per heart
                {
                    Game1.drawDialogueNoTyping($"{who.Name} does not have {npcName}'s phone number yet.");
                    return;
                }

                // Check if NPC has dialogue
                if (npc.CurrentDialogue.Count > 0)
                {
                    // Prevent gifting to NPC
                    var activeObject = who.ActiveObject;
                    if (activeObject != null)
                    {
                        who.ActiveObject = null;
                    }

                    // Talk
                    npc.checkAction(who, npc.currentLocation);

                    // Restore active object
                    if (activeObject != null)
                    {
                        who.ActiveObject = activeObject;
                    }
                }
                else
                {
                    // NPC has no dialogue, no answer
                    Game1.drawDialogueNoTyping("*No one answers...*");
                }
            }
        }
    }
}