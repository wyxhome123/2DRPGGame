using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletionBattleState : EnemyState
{
    private Transform player;
    Enemy_Skeleton enemy;
    private int moveDir;
    public SkeletionBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _aniBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _aniBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;

        if(player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(enemy.IsPlayerDetected()){
            stateTimer = enemy.battleTime;

            if(enemy.IsPlayerDetected().distance < enemy.attackDistance){
                // Debug.Log("I Attack");
                if(CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                }
                enemy.SetZeroVelocity();
                return;
            }
        }else{
            if(stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 7){
                stateMachine.ChangeState(enemy.idleState);
            }
        }

        if(player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if(player.position.x < enemy.transform.position.x)
            moveDir = -1;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    private bool CanAttack()
    {
        if(Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown){
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        Debug.Log("Attack is CoolDown");
        return false;
    }

}
