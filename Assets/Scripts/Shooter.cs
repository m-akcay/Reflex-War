using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum TroopColor
    { 
        WHITE,
        GREEN,
        PURPLE
    }

    private TroopColor troopColor;

    private static GameObject[] TOWERS = new GameObject[] { };
    public const float BASE_FIRE_RATE = 1.5f;
    public const float BASE_DAMAGE = 30;
    public const float BASE_SPEED = 0.4f;
    public const float BASE_RANGE = 4f;
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
    private Rigidbody rb;
    [SerializeField]
    private GameObject bullet;
    private Material mat;

    private bool rotating = false;
    private bool rotateCCW = false;

    private void Start()
    {
        foreach (var wheel in wheels)
        {
            wheel.maxAngularVelocity *= 2;
        }
    }

    // reactionMultiplier can take 3 values -> {1, 1.25, 1.5}
    public void init(float reactionMultiplier, float lifeTime)
    {
        if (TOWERS.Length == 0)
            setTowers();
        setTarget();

        troopColor = getTroopColor(reactionMultiplier);
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;

        var scaledReactionMultiplier = reactionMultiplier * GameManager.getDifficultyMultiplier();
        speed = BASE_SPEED * scaledReactionMultiplier;
        range = BASE_RANGE * scaledReactionMultiplier;
        if (range > 10)
            range = 10;

        var color = GameManager.getReactionColor(reactionMultiplier);
        mat.SetColor("Color_D1155CB1", color);
        bullet.GetComponent<Bullet>().init(reactionMultiplier, lifeTime, this.range * 0.2f, target,
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
            bullet.GetComponent<Bullet>().fire();
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
    private void setTarget()
    {
        float minDistance = float.MaxValue;
        int idx = 0;

        for (int i = 0; i < TOWERS.Length; i++)
        {
            float distance = Vector3.Distance(this.transform.position, TOWERS[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                idx = i;
            }
        }

        this.target = TOWERS[idx].transform;
    }
    private void OnDestroy()
    {
        Destroy(this.mat);
    }
    private static void setTowers()
    {
        TOWERS = GameObject.FindGameObjectsWithTag("Tower");
    }
    public static TroopColor getTroopColor(float reactionMultiplier)
    {
        switch (reactionMultiplier)
        {
            case 1f:
                return TroopColor.WHITE;
            case 1.25f:
                return TroopColor.GREEN;
            case 1.5f:
                return TroopColor.PURPLE;
            default:
                return TroopColor.WHITE;
        }
    }
}
