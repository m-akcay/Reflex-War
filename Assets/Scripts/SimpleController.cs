using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward * Time.deltaTime);
            Debug.Log(transform.position);
        }
        if (Input.GetKey(KeyCode.A))
            transform.Translate(-transform.right * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(transform.right * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(-transform.forward * Time.deltaTime);

    }
}
