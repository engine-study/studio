using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState{None = -1, Idle, Dead, Interact, Carry, Tool}

[CreateAssetMenu(fileName = "State", menuName = "Engine/State/StatePlayer", order = 1)]
public class SPState : ScriptableObject, IState
{
    [Header("Player")]
    public float moveSpeedMultiplier = 1f;
    public List<ActionRestriction> restrictions;

    public PlayerState StateType;

    public SPState(PlayerState state) {
        StateType = state;
    }

    public virtual void EnterState(IActor player) {

    }
    public virtual void UpdateState(IActor player) {
        
    }
    public virtual void ExitState(IActor player) {
        
    }
}

public interface IState {

    void EnterState(IActor player);
    void UpdateState(IActor player);
    void ExitState(IActor player);

}





