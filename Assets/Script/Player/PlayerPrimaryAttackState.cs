using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter{get; private set;}

    private float lastTimeAttacked;
    private float comboWindow = 2;
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        xInput = 0; //修复多次左右跑动后攻击方向与实际方向相反的问题

        if(comboCounter > 2 || Time.time > lastTimeAttacked + comboWindow)
            comboCounter = 0;

        player.anim.SetInteger("ComboCounter", comboCounter);

        # region 选择攻击方向
        float attackDir = player.faceDir;

        if(xInput != 0)
            attackDir = xInput;

        #endregion

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);

        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer < 0)
            player.SetZeroVelocity();
        if(triggerCalled)
            stateMachine.ChengeState(player.idleState);
    }
}
