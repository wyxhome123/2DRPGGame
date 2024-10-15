using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // player.skill.clone.CreateClone(player.transform, Vector3.zero);
        player.skill.dash.CloneonDash();
        stateTimer = player.dashDuration;

        player.stats.MakeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.skill.dash.CloneOnArrival();
        player.SetVelocity(0, rb.velocity.y);

        player.stats.MakeInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("I'm doing Dash!");

        if(!player.isGroundDetected() && player.isWallDetected())
            stateMachine.ChengeState(player.wallSlideState);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if(stateTimer < 0)
            stateMachine.ChengeState(player.idleState);

        player.fx.CreateAfterImage();
    }
}
