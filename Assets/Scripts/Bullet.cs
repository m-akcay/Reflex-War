using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject parentGo;
    private float damage;
    public void init(float damage)
    {
        this.damage = damage;
        parentGo = this.transform.parent.gameObject;
        this.transform.parent = this.transform.parent.parent;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Tower")
        {
            collision.gameObject.GetComponent<Tower>().applyDamage(this.damage);
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.transform.parent = parentGo.transform;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;
            
            //this.transform.
            //Destroy(this.gameObject);
        }
    }
}
