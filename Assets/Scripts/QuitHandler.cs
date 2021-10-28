using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitHandler : MonoBehaviour
{
    [SerializeField] private GameObject quitPanel = null;

    private void Start()
    {
        quitPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitPanel.SetActive(!quitPanel.activeInHierarchy);
        }
    }

    public void quit_onClick()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void quit_multiplayer_onClick()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MenuScene");
    }
    public void stay_onClick()
    {
        quitPanel.SetActive(false);
    }
}
