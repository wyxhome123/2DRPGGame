using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;

    public float counterAttackDuration = .2f;

    public bool isBusy{get; private set;}
    [Header("Move info")]
    public float moveSpeed = 8f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defalutMoveSpeed;
    private float defalutJumpForce;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDir{get; private set;}
    private float defalutDashSpeed;

    public SkillManager skill{get; private set;}
    public GameObject sword{get; private set;}

    public PlayerFX fx{get; private set;}



#region 状态
    public PlayerStateMachine stateMachine{get; private set;}
    public PlayerIdleState idleState{get; private set;}
    public PlayerMoveState moveState{get; private set;}

    public PlayerJumpState jumpState{get; private set;}

    public PlayerAirState airState{get; private set;}

    public PlayerWallSlideState wallSlideState{get; private set;}
    public PlayerWallJumpState wallJumpState{get; private set;}
    public PlayerDashState dashState{get; private set;}

    public PlayerPrimaryAttackState primaryAttack{get; private set;}

    public PlayerCounterAttackState counterAttackState{get; private set;}

    public PlayerAimSwordState aimSword{get; private set;}

    public PlayerCatchSwordState catchSword{get; private set;}

    public PlayerBlackholeState blackholeState{get; private set;}

    public PlayerDeadState deadState{get; private set;}

#endregion


    protected override void Awake(){
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");

        blackholeState = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");

        
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        fx = GetComponent<PlayerFX>();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defalutMoveSpeed = moveSpeed;
        defalutJumpForce = jumpForce;
        defalutDashSpeed = dashSpeed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(Time.timeScale == 0)
            return;

        stateMachine.currentState.Update();
        CheckForDashInput();

        if(Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
            skill.crystal.CanUseSkill();
        // Debug.Log(isWallDetected());

        if(Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.UseFlask();
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1- _slowPercentage);
        jumpForce = jumpForce * (1- _slowPercentage);
        dashSpeed = dashSpeed * (1- _slowPercentage);

        anim.speed = anim.speed * (1- _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        
        moveSpeed = defalutMoveSpeed;
        jumpForce = defalutJumpForce;
        dashSpeed = defalutDashSpeed;
    }

    public void AssignNewSword(GameObject _newSword){
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChengeState(catchSword);
        Destroy(sword);
    }



    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();
    private void CheckForDashInput()
    {
        if(isWallDetected())
            return;

        if(skill.dash.dashUnlocked == false)
            return;
        
        //dashUsageTimer -= Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            //dashUsageTimer = dashCoolDown;
            dashDir  = Input.GetAxisRaw("Horizontal");
            if(dashDir == 0)
                dashDir = faceDir;

            stateMachine.ChengeState(dashState);
        }
    }

    public IEnumerator BusyFor(float _seconds){
        isBusy = true;
        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChengeState(deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0,0);
    }



}
