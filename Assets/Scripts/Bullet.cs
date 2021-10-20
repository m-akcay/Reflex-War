using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject parentGo;
    private float damage;
    [SerializeField]
    private float timeSinceAvailable = 0;
    private bool _isAvailable;
    private float fireRate;
    private Material mat;
    private Vector3 forceDir;
    private Rigidbody rb;
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
    public void init(float damage, float fireRate, Color color)
    {
        rb = this.GetComponent<Rigidbody>();
        mat = this.GetComponent<Renderer>().material;

        this.damage = damage;
        this.fireRate = fireRate;
        mat.SetColor("Color_F3BBF886", color);
    }
    public void fire(Transform target)
    {
        //this.transform.parent = this.transform.parent.parent;
        if (!isAvailable)
            return;

        _isAvailable = false;

        var targetPosWithVerticalOffset = target.position + Vector3.up;
        transform.parent = transform.parent.parent;
        transform.LookAt(targetPosWithVerticalOffset);
        transform.Rotate(90, 0, 0, Space.Self);
        targetPosWithVerticalOffset += Vector3.up * 3;
        this.forceDir = (targetPosWithVerticalOffset - transform.position).normalized;
        rb.isKinematic = false;
        rb.AddForce(forceDir * 0.75f, ForceMode.Impulse);
    }
    private void FixedUpdate()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Tower")
        {
            collision.gameObject.GetComponent<Tower>().applyDamage(this.damage);
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.transform.parent = parentGo.transform;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;
            timeSinceAvailable = Time.realtimeSinceStartup;
            _isAvailable = true;
        }
    }
}
