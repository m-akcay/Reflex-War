using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private float totalHealth;
    void Start()
    {
    }
    public void applyDamage(float damage)
    {
        this.totalHealth -= damage;
    }
}
