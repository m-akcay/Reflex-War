using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private float totalHealth;
    private float remainingHealth;
    [SerializeField]
    private Image healthBar;
    void Start()
    {
        remainingHealth = totalHealth;
    }
    public void applyDamage(float damage)
    {
        if (this.remainingHealth > 0)
            this.remainingHealth -= damage;
        healthBar.fillAmount = remainingHealth / totalHealth;
        healthBar.color = Util.lerp(Color.red, Color.green, healthBar.fillAmount);
    }
}
