using HarmonyLib;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.UIElements.Collections;

namespace PersistentPurchasesRewritten;

[HarmonyPatch]
public class Patches
{
    [HarmonyPostfix, HarmonyPatch(typeof(Terminal), "Start")]
    [HarmonyPriority(-23)] // Should be run after everything else registers their unlockables to the list
    public static void generateConfig()
    {
        ConfigManager.SetupConfig(StartOfRound.Instance.unlockablesList.unlockables);
    }

    [HarmonyPrefix, HarmonyPatch(typeof(StartOfRound), "playersFiredGameOver")]
    public static void StoreUnlocked(StartOfRound __instance)
    {
        if (!GameNetworkManager.Instance.isHostingGame)
            return;
        
        Plugin.log.LogInfo("Taking note of bought unlockables");
        SavedUnlockables.unlockablesStore = new Dictionary<int, UnlockableItem>();
        
        List<UnlockableItem> items = __instance.unlockablesList.unlockables;
        
        for (int i = 0; i < items.Count; i++)
        {
            UnlockableItem currentItem = items[i];
            if (currentItem.unlockableType == (int)UnlockableTypes.Suits && !ConfigManager.shouldKeepSuits.Value) 
                continue;
            
            if (currentItem.unlockableType == (int)UnlockableTypes.Furniture && !ConfigManager.shouldKeepFurniture.Value) 
                continue;
            
            if (currentItem.hasBeenUnlockedByPlayer && ConfigManager.IsWhitelisted(currentItem) && ConfigManager.unlockablesConfigEntries[i].Value)
            {
                Plugin.log.LogInfo($"{currentItem.unlockableName}(ID: { i }) was unlocked");
                SavedUnlockables.unlockablesStore[i] = SavedUnlockables.CopyUnlockable(currentItem);
            }
        }
    }
    
    [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ResetShip))]
    public static void LoadUnlocked(StartOfRound __instance)
    {
        Plugin.log.LogInfo("Rebuying unlockables");
        foreach (KeyValuePair<int, UnlockableItem> unlockedItem in SavedUnlockables.unlockablesStore)
        {
            Plugin.log.LogInfo($"Restoring item { unlockedItem.Key } { unlockedItem.Value.unlockableName }");
            __instance.BuyShipUnlockableServerRpc(unlockedItem.Key, TimeOfDay.Instance.quotaVariables.startingCredits);
            if (unlockedItem.Value.unlockableType == (int)UnlockableTypes.Furniture && __instance.SpawnedShipUnlockables.ContainsKey(unlockedItem.Key))
            {
                NetworkObject networkObject = __instance.SpawnedShipUnlockables.Get(unlockedItem.Key).GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    if (!ConfigManager.shouldKeepPlacement.Value || unlockedItem.Value.inStorage)
                    {
                        ShipBuildModeManager.Instance.StoreObjectServerRpc(networkObject, 0);
                        Plugin.log.LogInfo($"Put item { unlockedItem.Value.unlockableName } in storage");
                    }
                    else if (unlockedItem.Value.IsPlaceable)
                    {
                        PlaceableShipObject placedObject = __instance.SpawnedShipUnlockables.Get(unlockedItem.Key).GetComponentInChildren<PlaceableShipObject>();
                        ShipBuildModeManager.Instance.PlaceShipObject(unlockedItem.Value.placedPosition, unlockedItem.Value.placedRotation, placedObject, false);
                        ShipBuildModeManager.Instance.PlaceShipObjectServerRpc(unlockedItem.Value.placedPosition, unlockedItem.Value.placedRotation, networkObject, 0);
                        Plugin.log.LogInfo($"Placed item { unlockedItem.Value.unlockableName } in the ship");
                    }
                }
                else
                {
                    Plugin.log.LogWarning($"Failed to find NetworkObject for {unlockedItem.Value.unlockableName}");
                }
            }
            else
            {
                Plugin.log.LogWarning($"SpawnedShipUnlockables did not contain {unlockedItem.Value.unlockableName}");
            }
        }
        SavedUnlockables.unlockablesStore.Clear();
    }
}
