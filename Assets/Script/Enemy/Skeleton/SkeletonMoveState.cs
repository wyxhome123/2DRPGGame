using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMoveState : SkeletonGroundState
{
    public SkeletonMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _aniBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _aniBoolName, _enemy)
    {
        enemy = _enemy;
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

        enemy.SetVelocity(enemy.moveSpeed * enemy.faceDir, rb.velocity.y);

        if(enemy.isWallDetected() || !enemy.isGroundDetected()){
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
