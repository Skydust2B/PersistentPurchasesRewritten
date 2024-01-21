using System.Collections.Generic;

namespace PersistentPurchasesRewritten;

public class SavedUnlockables
{
    public static Dictionary<int, UnlockableItem> unlockablesStore { get; set; }

    public static UnlockableItem CopyUnlockable(UnlockableItem unlockableItem)
    {
        return new UnlockableItem
        {
            unlockableName = unlockableItem.unlockableName,
            placedPosition = unlockableItem.placedPosition,
            placedRotation = unlockableItem.placedRotation,
            IsPlaceable = unlockableItem.IsPlaceable,
            inStorage = unlockableItem.inStorage,
            unlockableType = unlockableItem.unlockableType
        };
    }
}

