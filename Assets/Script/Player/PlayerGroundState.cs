using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackholeUnlocked)
        {
            if(player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("Cooldown!");
                return;
            }
            
            stateMachine.ChengeState(player.blackholeState);
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
            stateMachine.ChengeState(player.aimSword);

        if(Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)
            stateMachine.ChengeState(player.counterAttackState);

        if(Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChengeState(player.primaryAttack);

        if(!player.isGroundDetected())
            stateMachine.ChengeState(player.airState);

        if(Input.GetKeyDown(KeyCode.Space) && player.isGroundDetected())
            stateMachine.ChengeState(player.jumpState);
    }

    private bool HasNoSword()
    {
        if(!player.sword){
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
