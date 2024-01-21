using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlyState : PlayerState
{
    public PlayerFlyState(Player player, CharacterControlData_SO playerData, PlayerStateMachine stateMachine, string animBoolName) : base(player, playerData, stateMachine, animBoolName)
    {

    }


    public override void AnimTrigg()
    {
        base.AnimTrigg();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void BasicUpdates()
    {
        base.BasicUpdates();
    }

    public override void PhysicsUpdates()
    {
        base.PhysicsUpdates();
    }

    public override void LateUpdates()
    {
        base.LateUpdates();
    }
}

