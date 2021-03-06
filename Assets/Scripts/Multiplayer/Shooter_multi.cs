using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooter_multi : MonoBehaviourPun
{
    public static Color FriendColor = new Color(0.3529412f, 0.7764706f, 1f, 1f);
    public static Color EnemyColor = new Color(1f, 0.5f, 0f, 1f);

    [SerializeField]
    private LayerMask _enemyMask = 0;
    public static LayerMask EnemyMask;

    public const int EnemyLayer = 13;

    public const float BASE_FIRE_RATE = 1.5f;
    public const float BASE_DAMAGE = 30;
    public const float BASE_SPEED = 0.4f;
    public const float BASE_RANGE = 4f;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float range;
    [SerializeField]
    public Transform target { get; private set; }
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
    private Material[] wheelMaterials;
    private Material gunMat;
    public Color color { get; private set; }

    private bool rotating = false;
    private bool rotateCCW = false;
    private void Awake()
    {
        EnemyMask = _enemyMask;
    }
    private void Start()
    {
        
    }
    // reactionMultiplier can take 3 values -> {1, 1.25, 1.5}
    public void init(float reactionMultiplier)
    {
        setTarget();

        rb = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;

        var scaledReactionMultiplier = reactionMultiplier * GameManager_multi.getDifficultyMultiplier();
        speed = BASE_SPEED * scaledReactionMultiplier;
        range = BASE_RANGE * scaledReactionMultiplier;
        if (range > 10)
            range = 10;

        color = GameManager_multi.getReactionColor(reactionMultiplier);
        mat.SetColor("Color_D1155CB1", color);
        
        bullet.GetComponent<Bullet_multi>().init(reactionMultiplier, this.range * 0.2f, target,
                                            color);
        
        if (photonView.IsMine)
        {
            photonView.RPC("syncVars", RpcTarget.Others, reactionMultiplier);
        }
        else
        {
            this.gameObject.layer = 13;
            rb.useGravity = false;

            mat.SetColor("Color_DC628308", EnemyColor);

            wheelMaterials = new Material[4];
            for (int i = 0; i < wheels.Length; i++)
            {
                var wheel = wheels[i];
                wheelMaterials[i] = wheel.gameObject.GetComponent<Renderer>().material;
                wheelMaterials[i].SetColor("_BaseColor", EnemyColor);
                wheel.useGravity = false;
                wheel.gameObject.GetComponent<SphereCollider>().enabled = false;
            }

            var gunObj = transform.Find("Gun");
            gunMat = gunObj.GetComponent<Renderer>().material;
            gunMat.SetColor("_BaseColor", EnemyColor);
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

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
        if (!photonView.IsMine)
            return;

        float angle = Vector3.Angle(transform.forward.xz(), target.transform.position.xz() - transform.position.xz());

        if (rotating)
        {
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
            if (angle > 5f)
            {
                startRotation();
                return;
            }
            bullet.GetComponent<Bullet_multi>().fire();
        }
    }
    private void rotate(bool rotateCCW)
    {
        transform.Rotate(Vector3.up, 20f * Time.deltaTime * (rotateCCW ? -1 : 1));
    }
    private void setRotationDirection()
    {
        float angleDefault = Vector3.Angle(transform.forward.xz(), target.transform.position.xz() - transform.position.xz());
        float angleShiftedToRight = Vector3.Angle(transform.forward.xz(), (target.transform.position.xz() + transform.right * 0.01f) - transform.position.xz());
        if (angleDefault > angleShiftedToRight)
            rotateCCW = true;
        else
            rotateCCW = false;
    }
    private void startRotation()
    {
        stop();
        setRotationDirection();
        rotating = true;
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

        for (int i = 0; i < GameManager_multi.TOWERS.Length; i++)
        {
            float distance = Vector3.Distance(this.transform.position, GameManager_multi.TOWERS[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                idx = i;
            }
        }

        this.target = GameManager_multi.TOWERS[idx].transform;
        bullet.GetComponent<Bullet_multi>().target = this.target;
    }
    private void OnDestroy()
    {
        Destroy(this.mat);
        if (!photonView.IsMine)
        {
            foreach (var wheelMat in wheelMaterials)
            {
                Destroy(wheelMat);
            }

            Destroy(gunMat);
        }
    }

    [PunRPC]
    private void syncVars(float reactionMultiplier)
    {
        if (!photonView.IsMine)
        {
            this.init(reactionMultiplier);
        }
    }

    public void disable()
    {
        bullet.SetActive(false);

        foreach (var wheel in wheels)
        {
            wheel.isKinematic = true;
        }
        rb.isKinematic = true;

        attacking = false;
    }
}
