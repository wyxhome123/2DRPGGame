using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerFX : EntityFX
{
    
    
    private CinemachineImpulseSource screenShake;
    [Header("Screen shake FX")]
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordImpact;
    public Vector3 shakeHighDamage;


    [Header("After image fx")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;


    
    [Space]
    [SerializeField] private ParticleSystem dustFx;

    protected override void Start()
    {
        base.Start();
        
        screenShake = GetComponent<CinemachineImpulseSource>();
    }



    private void Update() {
        
        afterImageCooldownTimer -= Time.deltaTime;
    }

    

    
    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.faceDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    
    public void CreateAfterImage()
    {
        if(afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite);

        }
    }

    public void PlayDustFX()
    {
        if(dustFx != null)
        {
            dustFx.Play();
        }
    }

}
