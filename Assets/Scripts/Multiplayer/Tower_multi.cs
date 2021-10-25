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
        healthBar.transform.LookAt(GameManager_multi.mainCam.transform);
        healthBarMat = healthBar.GetComponent<SpriteRenderer>().material;
        damageTakenFromEnemy = 0;
        damageTakenFromPlayer = 0;
    }
    public void applyDamage(float damage, int layer)
    {
        if (layer == Bullet_multi.EnemyBulletLayer)
        {
            damageTakenFromEnemy += damage;
            if (damageTakenFromPlayer > damage)
                damageTakenFromPlayer -= damage / 2;
        }
        else
        {
            damageTakenFromPlayer += damage;
            if (damageTakenFromEnemy > damage)
                damageTakenFromEnemy -= damage / 2;
        }

        var totalDamage = damageTakenFromEnemy + damageTakenFromPlayer;
        healthBarMat.SetFloat("_SplitPos", damageTakenFromPlayer / totalDamage);
    }

    private void OnDestroy()
    {
        Destroy(healthBarMat);
    }
}
