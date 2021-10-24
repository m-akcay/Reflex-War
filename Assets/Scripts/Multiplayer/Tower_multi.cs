using Photon.Pun;
using UnityEngine;

public class Tower_multi : MonoBehaviourPun
{
    [SerializeField]
    private float remainingHealth;
    [SerializeField]
    private GameObject healthBar;
    private Material healthBarMat;

    [SerializeField]
    private float damageTakenFromPlayer;
    [SerializeField]
    private float damageTakenFromEnemy;
    void Start()
    {
        //if (GameManager_multi.mainCam.transform)
        //    Debug.Log("null");
        healthBar.transform.LookAt(GameManager_multi.mainCam.transform);
        healthBarMat = healthBar.GetComponent<SpriteRenderer>().material;
        damageTakenFromEnemy = 0;
        damageTakenFromPlayer = 0;
    }
    public void applyDamage(float damage, int layer)
    {
        Debug.Log(damage);
        //if (layer == Bullet_multi.EnemyBulletLayer)
        //{
        //    damageTakenFromEnemy += damage;
        //    if (damageTakenFromPlayer > damage)
        //        damageTakenFromPlayer -= damage / 2;
        //}
        //else
        //{
            damageTakenFromPlayer += damage;
            if (damageTakenFromEnemy > damage)
                damageTakenFromEnemy -= damage / 2;
        //}

        //Debug.Log(string.Format("fromPlayer->{0}   fromEnemy->{1}", damageTakenFromPlayer, damageTakenFromEnemy));
        
        photonView.RPC("syncDamage", RpcTarget.Others, damageTakenFromEnemy, damageTakenFromPlayer);
    }

    [PunRPC]
    public void syncDamage(float damageTakenFromEnemy, float damageTakenFromPlayer)
    {
        var totalDamage = damageTakenFromEnemy + damageTakenFromPlayer;

        if (photonView.IsMine)
        {
            this.damageTakenFromEnemy = damageTakenFromEnemy;
            this.damageTakenFromPlayer = damageTakenFromPlayer;
            healthBarMat.SetFloat("_SplitPos", damageTakenFromPlayer / totalDamage);
        }
        else
        {
            this.damageTakenFromEnemy = damageTakenFromPlayer;
            this.damageTakenFromPlayer = damageTakenFromEnemy;
            healthBarMat.SetFloat("_SplitPos", damageTakenFromEnemy / totalDamage);
        }
    }

    private void OnDestroy()
    {
        Destroy(healthBarMat);
    }
}
