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
    private GameObject winnerIndicator;
    private Material winnerIndicatorMat;

    [SerializeField]
    private float damageTakenFromPlayer;
    [SerializeField]
    private float damageTakenFromEnemy;
    void Start()
    {
        healthBar.transform.LookAt(GameManager_multi.mainCam.transform);
        healthBarMat = healthBar.GetComponent<SpriteRenderer>().material;

        winnerIndicatorMat = winnerIndicator.GetComponent<Renderer>().material;

        damageTakenFromEnemy = 0;
        damageTakenFromPlayer = 0;
    }
    public void applyDamage(float damage, int layer)
    {
        if (layer == Bullet_multi.EnemyBulletLayer)
        {
            damageTakenFromEnemy += damage;
            damageTakenFromPlayer = Mathf.Clamp(damageTakenFromPlayer - damage, 0, float.MaxValue);
        }
        else
        {
            damageTakenFromPlayer += damage;
            damageTakenFromEnemy = Mathf.Clamp(damageTakenFromEnemy - damage, 0, float.MaxValue);
        }

        setColors();
    }

    private void setColors()
    {
        var totalDamage = damageTakenFromEnemy + damageTakenFromPlayer;
        float splitPos = damageTakenFromPlayer / totalDamage;

        healthBarMat.SetFloat("_SplitPos", splitPos);
        winnerIndicatorMat.SetColor("_BaseColor", (splitPos < 0.5f) ? Shooter_multi.EnemyColor : Shooter_multi.FriendColor);
    }

    private void OnDestroy()
    {
        Destroy(healthBarMat);
        Destroy(winnerIndicatorMat);
    }
}
