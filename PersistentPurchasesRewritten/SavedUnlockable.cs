using System.Collections.Generic;

namespace PersistentPurchasesRewritten;

public class SavedUnlockables
{
    public static Dictionary<int, UnlockableItem> unlockablesStore { get; set; }

    public static UnlockableItem CopyUnlockable(UnlockableItem unlockableItem)
    {
        UnlockableItem savedUnlockable = new();
        savedUnlockable.unlockableName = unlockableItem.unlockableName;
        savedUnlockable.placedPosition = unlockableItem.placedPosition;
        savedUnlockable.placedRotation = unlockableItem.placedRotation;
        savedUnlockable.IsPlaceable = unlockableItem.IsPlaceable;
        savedUnlockable.inStorage = unlockableItem.inStorage;
        savedUnlockable.unlockableType = unlockableItem.unlockableType;
        return savedUnlockable;
    }
}

