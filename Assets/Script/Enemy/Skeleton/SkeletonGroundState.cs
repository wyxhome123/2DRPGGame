using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundState : EnemyState
{

    protected Enemy_Skeleton enemy;

    protected Transform player;
    public SkeletonGroundState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _aniBoolName) : base(_enemyBase, _stateMachine, _aniBoolName)
    {
    }

    public SkeletonGroundState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _aniBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _aniBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position) < 2)
            stateMachine.ChangeState(enemy.battleState);
    }

}
