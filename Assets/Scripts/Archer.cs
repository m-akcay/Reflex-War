using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    private const float BASE_FIRE_RATE = 1.5f;
    private const float BASE_DAMAGE = 30;
    private const float BASE_SPEED = 0.2f;
    private const float BASE_RANGE = 5f;
    private float damage;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float range;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private bool attacking;
    [SerializeField]
    private float distance;
    [SerializeField]
    private Rigidbody[] wheels;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private GameObject bullet;
    private Material mat;
    private Material bulletMat;

    private bool rotating = false;
    private bool rotateCCW = false;

    void Start()
    {
        target = GameObject.Find("RuinedTower").transform;
        //this.transform.LookAt(target, Vector3.up);
        this.speed = BASE_SPEED;
        this.damage = BASE_DAMAGE;
        foreach (var wheel in wheels)
        {
            wheel.maxAngularVelocity *= 2;
        }
    }

    // reactionMultiplier can take 3 values -> {1, 1.25, 1.5}
    public void init(float reactionMultiplier)
    {
        this.damage = BASE_DAMAGE * reactionMultiplier;
        this.speed = BASE_SPEED * reactionMultiplier;
        range = BASE_RANGE * reactionMultiplier;

        Color color;
        GameManager.SPAWN_COLOR_MAP.TryGetValue(reactionMultiplier, out color);
        bullet.GetComponent<Bullet>().init(damage, 
                                        BASE_FIRE_RATE / reactionMultiplier, 
                                        color);
    }

    private void FixedUpdate()
    {
        Vector3 posDiff = (target.position - transform.position);
        Vector3 moveDirection = posDiff.normalized;
        distance = posDiff.magnitude;
        if (distance > range)
        {
            Vector3 torqueDirection = Vector3.Cross(Vector3.up, moveDirection).normalized;
            foreach (var wheel in wheels)
            {
                wheel.AddTorque(torqueDirection * speed);
            }
            attacking = false;
        }
        else if (!rotating)
        {
            stop();
            setRotationDirection();
            rotating = true;
        }
    }
    private void Update()
    {
        if (rotating)
        {
            float angle = Vector3.Angle(transform.forward.xz(), target.transform.position.xz() - transform.position.xz());
            if (angle < 5f)
            {
                rotating = false;
                attacking = true;
                return;
            }
            rotate(rotateCCW);
        }
        else if (attacking)
        {
            bullet.GetComponent<Bullet>().fire(target);
        }
    }

    private void rotate(bool rotateCCW)
    {
        //transform.Rotate()
        transform.Rotate(Vector3.up, 20f * Time.deltaTime * (rotateCCW ? -1 : 1));
    }
    private void setRotationDirection()
    {
        float angleDefault = Vector3.Angle(transform.forward.xz(), target.transform.position.xz() - transform.position.xz());
        float angleShiftedToRight = Vector3.Angle(transform.forward.xz(), (target.transform.position.xz() + Vector3.right * 0.01f) - transform.position.xz());
        if (angleDefault > angleShiftedToRight)
            rotateCCW = true;
        else
            rotateCCW = false;
    }
    private void stop()
    {
        foreach (var wheel in wheels)
        {
            wheel.isKinematic = true;
        }
        rb.isKinematic = true;
        rb.isKinematic = false;
        foreach (var wheel in wheels)
        {
            wheel.isKinematic = false;
        }
    }
}
