using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private float totalHealth = 0;
    [SerializeField]
    private float remainingHealth;
    [SerializeField]
    private GameObject healthBar = null;
    private Material healthBarMat;

    void Start()
    {
        remainingHealth = totalHealth;
        healthBar.transform.LookAt(Camera.main.transform);
        healthBarMat = healthBar.GetComponent<SpriteRenderer>().material;
    }
    public void applyDamage(float damage)
    {
        remainingHealth -= damage;
        if (remainingHealth < 0)
        {
            GameManager.TOWERS.Remove(this.gameObject);
            Destroy(this.gameObject);
        }

        healthBarMat.SetFloat("_SplitPos", remainingHealth / totalHealth);
    }

    private void OnDestroy()
    {
        Destroy(healthBarMat);
    }
}
