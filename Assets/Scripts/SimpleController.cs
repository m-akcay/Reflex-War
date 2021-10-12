using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.Play("Attack3");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.Play("idle2");
        }
    }
}
