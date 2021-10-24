﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_multi : MonoBehaviourPun
{
    public const int EnemyBulletLayer = 14;

    [SerializeField]
    private Shooter_multi shooter;
    private GameObject parentGo;
    private Shooter.TroopColor troopColor;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float timeSinceAvailable = 0;
    private bool _isAvailable;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float damage;
    private Material bulletMat;
    [SerializeField]
    private float forceMultiplier;
    private Rigidbody bulletRb;
    private bool destroyTimerStarted = false;
    private float lifeTime;
    private bool isAvailable 
    { 
        get 
        {
            return _isAvailable && (Time.realtimeSinceStartup - timeSinceAvailable > fireRate); 
        } 
    }
    private void Awake()
    {
        if (!photonView.IsMine)
            this.gameObject.layer = EnemyBulletLayer;
    }
    private void Start()
    {
        _isAvailable = true;
        parentGo = this.transform.parent.gameObject;
    }
    public void init(float reactionMultiplier, float lifeTime, float forceMultiplier, Transform target, Color color)
    {
        var scaledReactionMultiplier = reactionMultiplier * GameManager.getDifficultyMultiplier();

        this.troopColor = Shooter.getTroopColor(reactionMultiplier);
        this.lifeTime = lifeTime;
        this.forceMultiplier = forceMultiplier;
        this.damage = Shooter.BASE_DAMAGE * scaledReactionMultiplier;
        this.target = target;
        bulletRb = this.GetComponent<Rigidbody>();
        bulletMat = this.GetComponent<Renderer>().material;
        this.fireRate = Shooter.BASE_FIRE_RATE / scaledReactionMultiplier;
        bulletMat.SetColor("Color_F3BBF886", color);
    }
    public void fire()
    {
        if (!isAvailable)
            return;

        _isAvailable = false;

        var targetPosWithVerticalOffset = target.position + Vector3.up;
        transform.parent = transform.parent.parent;
        transform.LookAt(targetPosWithVerticalOffset);
        transform.Rotate(90, 0, 0, Space.Self);
        targetPosWithVerticalOffset += Vector3.up * 2;
        var forceDir = (targetPosWithVerticalOffset - transform.position).normalized;
        bulletRb.isKinematic = false;
        bulletRb.AddForce(forceDir * this.forceMultiplier, ForceMode.Impulse);

        if (!destroyTimerStarted)
        {
            startDestroyTimer();
        }
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tower")
        {
            collision.gameObject.GetComponent<Tower_multi>().applyDamage(this.damage, this.gameObject.layer);
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.transform.parent = parentGo.transform;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;
            timeSinceAvailable = Time.realtimeSinceStartup;
            _isAvailable = true;
        }
    }
    private void startDestroyTimer()
    {
        destroyTimerStarted = true;
        Destroy(transform.parent.gameObject, this.lifeTime);
    }
    private void OnDestroy()
    {
        Destroy(this.bulletMat);
    }
}
