using StardewValley;

class ModConfig
{
    public int MinimumFriendshipRequired { get; set; } = NPC.maxFriendshipPoints / 2;
    public int CostPerCall { get; set; } = 5;
}