using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargettable
{
    // A targettable is anything that can be targeted by an action
    // This includes units, traps, and anything else that can be affected by actions

    int GetStat(string stat);
    void ModifyStat(string stat, int amount);

    void SetSelectionState(SelectArrowState state);

    // This allows a targettable to deal with their own statuses
    // and do things with the source like deal thorns damage
    // procs indicates whether or not this damage should trigger effects
    // thorns for example would not proc, otherwise it would cause infinite loops
    void TakeDamage(int amount, ITargettable source, bool procs);

    // Add more stuff for statuses later
    void ApplyStatus(StatusEffect status, int count);
    bool HasStatus(StatusEffect status);
    void RemoveStatus(StatusEffect status, int count);
}
