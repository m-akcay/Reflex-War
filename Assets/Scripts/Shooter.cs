using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public static List<GameObject> TOWERS = new List<GameObject>();
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
    private Rigidbody[] wheels = null;
    private Rigidbody rb;
    [SerializeField]
    private GameObject bullet = null;
    private Material mat;

    private bool rotating = false;
    private bool rotateCCW = false;

    private bool finished = false;
    private bool gettingDestroyed = false;

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
        if (TOWERS.Count == 0)
            setTowers();
        setTarget();

        rb = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;

        var scaledReactionMultiplier = reactionMultiplier * GameManager.getDifficultyMultiplier();
        speed = BASE_SPEED * scaledReactionMultiplier;
        range = BASE_RANGE * scaledReactionMultiplier;
        if (range > 10)
            range = 10;

        var color = GameManager.getReactionColor(reactionMultiplier);
        mat.SetColor("Color_D1155CB1", color);
        bullet.GetComponent<Bullet>().init(reactionMultiplier, this.range * 0.2f, target,
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
        else if (!rotating && !attacking)
        {
            startRotation();
        }
    }
    private void Update()
    {
        if (!target)
        {
            setTarget();
        }

        if (finished)
        {
            if (!gettingDestroyed)
            {
                Destroy(this.gameObject, 5);
                gettingDestroyed = true;
            }

            return;
        }

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

    private void startRotation()
    {
        stop();
        setRotationDirection();
        rotating = true;
    }

    private void rotate(bool rotateCCW)
    {
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
        if (TOWERS.Count == 0)
        {
            finished = true;
            return;
        }

        float minDistance = float.MaxValue;
        int idx = 0;

        for (int i = 0; i < TOWERS.Count; i++)
        {
            float distance = Vector3.Distance(this.transform.position, TOWERS[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                idx = i;
            }
        }

        this.target = TOWERS[idx].transform;
        bullet.GetComponent<Bullet>().target = this.target;
        attacking = false;
    }
    private void OnDestroy()
    {
        Destroy(this.mat);
    }
    private static void setTowers()
    {
        TOWERS = GameObject.FindGameObjectsWithTag("Tower").ToList();
    }
}
