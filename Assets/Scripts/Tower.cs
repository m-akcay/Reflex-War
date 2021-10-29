using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
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
        healthBar.transform.LookAt(Camera.main.transform);
        healthBarMat = healthBar.GetComponent<SpriteRenderer>().material;
        damageTakenFromEnemy = 0;
        damageTakenFromPlayer = 0;
    }
    public void applyDamage(float damage, Shooter.TroopColor troopColor)
    {
        if (troopColor == Shooter.TroopColor.GREEN || troopColor == Shooter.TroopColor.WHITE)
        {
            damageTakenFromEnemy += damage;
            if (damageTakenFromPlayer > damage)
                damageTakenFromPlayer -= damage / 2;
        }
        else if (troopColor == Shooter.TroopColor.PURPLE)
        {
            damageTakenFromPlayer += damage;
            if (damageTakenFromEnemy > damage)
                damageTakenFromEnemy -= damage / 2;
        }

        //Debug.Log(string.Format("fromPlayer->{0}   fromEnemy->{1}", damageTakenFromPlayer, damageTakenFromEnemy));
        var totalDamage = damageTakenFromEnemy + damageTakenFromPlayer;
        healthBarMat.SetFloat("_SplitPos", damageTakenFromPlayer / totalDamage);
    }

    private void OnDestroy()
    {
        Destroy(healthBarMat);
    }
}
