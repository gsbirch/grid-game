using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnEntity
{
    // A turn entity is anything that gets a turn in the initiative order
    // They can be units, traps, or anything else that needs to act on its turn

    void TakeTurn();

    // These will be used by various status conditions
    void StartOfTurn();
    void EndOfTurn();

    // Picture for the initiative order UI
    // Could be updated to reflect various stats
    Sprite GetInitiativePicture();
}
