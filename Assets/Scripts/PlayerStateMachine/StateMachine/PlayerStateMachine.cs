using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerStateMachine 
{
   public PlayerState CurrentState { get; private set; }

public void InitializeState(PlayerState startingState)
{
    CurrentState = startingState;
    CurrentState.Enter();
}

public void SetNewState(PlayerState newState)
{
    CurrentState.Exit();
    CurrentState = newState;
    CurrentState.Enter();
}
}
