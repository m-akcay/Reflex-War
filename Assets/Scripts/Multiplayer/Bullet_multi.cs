using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_multi : MonoBehaviourPun
{
    public const int EnemyBulletLayer = 14;

    private GameObject parentGo;
    [SerializeField]
    public Transform target = null;
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
    private bool isAvailable 
    { 
        get 
        {
            return _isAvailable && (Time.realtimeSinceStartup - timeSinceAvailable > fireRate); 
        } 
    }

    private void Start()
    {
        _isAvailable = true;
        parentGo = this.transform.parent.gameObject;
    }
    public void init(float reactionMultiplier, float forceMultiplier, Transform target, Color color)
    {
        bulletRb = this.GetComponent<Rigidbody>();
        bulletMat = this.GetComponent<Renderer>().material;

        var scaledReactionMultiplier = reactionMultiplier * GameManager_multi.getDifficultyMultiplier();

        this.forceMultiplier = forceMultiplier;
        this.damage = Shooter_multi.BASE_DAMAGE * scaledReactionMultiplier;
        this.target = target;
        
        this.fireRate = Shooter_multi.BASE_FIRE_RATE / scaledReactionMultiplier;
        bulletMat.SetColor("Color_F3BBF886", color);
        if (!photonView.IsMine)
            this.gameObject.layer = EnemyBulletLayer;
    }
    public void fire()
    {
        if (!isAvailable)
            return;

        if (photonView.IsMine)
            photonView.RPC("shootSignal", RpcTarget.Others);
        _isAvailable = false;

        shoot();
    }
    
    private void shoot()
    {
        var targetPosWithVerticalOffset = target.position + Vector3.up;
        transform.parent = transform.parent.parent;
        transform.LookAt(targetPosWithVerticalOffset);
        transform.Rotate(90, 0, 0, Space.Self);
        targetPosWithVerticalOffset += Vector3.up * 2;
        var forceDir = (targetPosWithVerticalOffset - transform.position).normalized;
        bulletRb.isKinematic = false;
        bulletRb.AddForce(forceDir * this.forceMultiplier, ForceMode.Impulse);
    }

    private void reload()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.transform.parent = parentGo.transform;
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        timeSinceAvailable = Time.realtimeSinceStartup;
        if (photonView.IsMine)
            _isAvailable = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tower")
        {
            collision.gameObject.GetComponent<Tower_multi>().applyDamage(this.damage, this.gameObject.layer);
            reload();   
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Reloader")
            reload();
    }

    [PunRPC]
    private void shootSignal()
    {
        shoot();
    }

    private void OnDestroy()
    {
        Destroy(this.bulletMat);
    }
}
