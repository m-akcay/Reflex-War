using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private void Awake()
    {
    }
    public void onClick_singlePlayer_button()
    {
        SceneManager.LoadScene("WarScene");
    }
    public void onClick_multiPlayer_button()
    {
        Debug.Log("multi_onClick");
    }
}
