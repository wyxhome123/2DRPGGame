using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //修改朝向
        player.SetVelocity(xInput, rb.velocity.y);
        //出现moveState和Idlestate频繁转换的问题, 添加&& !player.isWallDetected()解决问题
        //xInput！=0，朝向的方向有墙不进入movesate
        //作者解法
        if(xInput == player.faceDir && player.isWallDetected())
            return;

        if(xInput != 0 && !player.isBusy){
            stateMachine.ChengeState(player.moveState);
        }

    }


}
