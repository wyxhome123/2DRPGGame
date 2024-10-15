using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravity;
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }
    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;

        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        rb.gravityScale = defaultGravity;
        player.fx.Maketransprent(false);
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer > 0)
            rb.velocity = new Vector2(0, 15);

        if(stateTimer < 0){
            rb.velocity = new Vector2(0, -.1f);

            if(!skillUsed){
                // Debug.Log("Cast BlackHole");
                if(player.skill.blackhole.CanUseSkill())
                    skillUsed = true;
            }
        }

        //攻击所有敌人后退出黑洞状态
        if(player.skill.blackhole.SkillCompleted())
            stateMachine.ChengeState(player.airState);
    }
}
