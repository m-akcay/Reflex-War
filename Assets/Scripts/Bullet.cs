using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject parentGo;
    [SerializeField]
    public Transform target;
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
        var scaledReactionMultiplier = reactionMultiplier * GameManager.getDifficultyMultiplier();

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

    private void OnDestroy()
    {
        Destroy(this.bulletMat);
    }
}
