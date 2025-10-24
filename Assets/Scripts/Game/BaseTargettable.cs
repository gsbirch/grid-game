using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTargettable : MonoBehaviour, ITargettable {

    public Dictionary<StatusEffect, int> statuses = new();

    public abstract int GetStat(string stat);
    public abstract void ModifyStat(string stat, int amount);
    public abstract void SetSelectionState(SelectArrowState state);

    public virtual int GetStacksOfStatus(StatusEffect status) {
        return statuses.ContainsKey(status) ? statuses[status] : 0;
    }

    public virtual void ApplyStatus(StatusEffect status, int count) {
        if (count < 0 && !status.canGoNegative) {
            Debug.LogError("Cannot apply negative status stacks!");
        }
        if (statuses.ContainsKey(status)) {
            statuses[status] += count;
        }
        else {
            statuses[status] = count;
        }
        if (statuses[status] > status.maxStacks)
            statuses[status] = status.maxStacks;
        if (statuses[status] == 0 || (statuses[status] <= 0 && !status.canGoNegative)) {
            statuses.Remove(status);
        }
    }

    public virtual bool HasStatus(StatusEffect status) {
        return statuses.ContainsKey(status);
    }

    public virtual void RemoveStatus(StatusEffect status, int count) {
        if (count < 0) {
            Debug.LogError("Cannot remove negative status stacks!");
        }

        if (!statuses.ContainsKey(status)) return;
        statuses[status] -= count;
        if (statuses[status] == 0 || (statuses[status] <= 0 && !status.canGoNegative)) {
            statuses.Remove(status);
        }
    }


    public virtual void TakeDamage(int amount, ITargettable source, bool procs) {
        float damageScale = 1.0f;
        if (HasStatus(Game.STATUS_Shatter)) {
            damageScale *= 2f;
            RemoveStatus(Game.STATUS_Shatter, 1);
        }
        if (HasStatus(Game.STATUS_Defend)) {
            damageScale /= 2f;
            RemoveStatus(Game.STATUS_Defend, 1);
        }
        if (amount > 0) {
            amount = Mathf.FloorToInt(amount * damageScale);
        }
        if (HasStatus(Game.STATUS_Defense)) {
            // Defense does not get removed, so it is prioritized over ReduceDamage
            var stacks = GetStacksOfStatus(Game.STATUS_Defense);
            amount = Mathf.Max(amount - stacks, 0);
        }

        if (HasStatus(Game.STATUS_ReduceDamage)) {
            var stacks = GetStacksOfStatus(Game.STATUS_ReduceDamage);
            var stacksConsumed = Mathf.Min(stacks, amount);
            amount -= stacksConsumed;
            RemoveStatus(Game.STATUS_ReduceDamage, stacksConsumed);

        }
        
        ModifyStat(GameManager.HEALTH_STAT, -amount);
        GameManager.Instance.CreateDamageText(transform.position, -amount);
    }
}
