using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitHandler : MonoBehaviour
{
    [SerializeField] private GameObject quitPanel = null;
    [SerializeField] private GameObject quitButton_touch = null;
    public static bool QuitAvailable = true;

    [SerializeField]
    private GameManager gm = null;

    [SerializeField]
    private GameManager_multi gm_multi = null;
    private void Start()
    {
        quitPanel.SetActive(false);
    }

    private void Update()
    {
        if (QuitAvailable)
        {
            if (gm && !gm.reflexPhase)
                quitButton_touch.SetActive(true);
            else if (gm_multi && !gm_multi)
                quitButton_touch.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                quitButton_touch.SetActive(!quitButton_touch.activeInHierarchy);
                quitPanel.SetActive(!quitPanel.activeInHierarchy);
            }
        }
        else
        {
            if (quitButton_touch.activeInHierarchy)
                quitButton_touch.SetActive(false);
        }
    }

    public void quit_onClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void quit_multiplayer_onClick()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MenuScene");
    }

    public void quit_onTouch()
    {
        quitButton_touch.SetActive(false);
        quitPanel.SetActive(true);
    }

    public void stay_onClick()
    {
        quitButton_touch.SetActive(true);
        quitPanel.SetActive(false);
    }
}
