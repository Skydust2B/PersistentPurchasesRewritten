using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace PersistentPurchasesRewritten;

public class ConfigManager
{
    public static ConfigFile configFile { get; set; }
    public static Dictionary<int, ConfigEntry<bool>> unlockablesConfigEntries = new Dictionary<int, ConfigEntry<bool>>();
    public static ConfigEntry<bool> shouldKeepSuits { get; set; }
    public static ConfigEntry<bool> shouldKeepFurniture { get; set; }
    public static ConfigEntry<bool> shouldKeepPlacement  { get; set; }
    
    private static string[] uselessButCoolFurnitures =
    {
        "Cozy lights", "Television", "Cupboard", "File Cabinet", "Toilet", "Shower", "Light switch", "Record player", 
        "Table", "Romantic table", "Bunkbeds", "JackOLantern", "Welcome mat", "Goldfish", "Plushie pajama man",
        // Lethal things
        "Small Rug", "Large Rug", "Fatalities Sign"
    };

    private static string[] uselessButCoolSuits =
    {
        "Orange suit", "Green suit", "Hazard suit", "Pajama suit", "Purple Suit"
    };

    public static bool IsWhitelisted(UnlockableItem item)
    {
        switch (item.unlockableType)
        {
            case (int)UnlockableTypes.Suits:
                return uselessButCoolSuits.Contains(item.unlockableName);
            case (int)UnlockableTypes.Furniture:
                return uselessButCoolFurnitures.Contains(item.unlockableName);
        }
        return false;
    }

    public static void SetupConfig(List<UnlockableItem> unlockableItems)
    {
        shouldKeepSuits = configFile.Bind("General", "KeepSuits", true, "Keeps every suits you've unlocked");
        shouldKeepFurniture = configFile.Bind("General", "KeepFurnitures", true, "Keeps every cosmetic furnitures you've unlocked");
        shouldKeepPlacement = configFile.Bind("General", "KeepPlacement", true, "Places back every furnitures inside the ship and puts everything else in storage.");

        for (int i = 0; i < unlockableItems.Count; i++)
        {
            if (!IsWhitelisted(unlockableItems[i]))
            {
                Plugin.log.LogDebug($"The { unlockableItems[i].unlockableName } item is not whitelisted internally.");
                continue;
            }
            
            Plugin.log.LogDebug($"Trying to register the config for {unlockableItems[i].unlockableName} with the type {unlockableItems[i].unlockableType}(id {i})");
            
            switch (unlockableItems[i].unlockableType)
            {
                case 0:
                {
                    unlockablesConfigEntries[i] = configFile.Bind("Items.Suits", unlockableItems[i].unlockableName, true);
                    break;
                }
                case 1:
                {
                    unlockablesConfigEntries[i] = configFile.Bind("Items.Furniture", unlockableItems[i].unlockableName, true);
                    break;
                }
            }
        }
    }
}
