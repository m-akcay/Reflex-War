using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    private const string PlayerNameKey = "PlayerName";
    [SerializeField] private TMP_InputField nameInput = null;
    [SerializeField] private Button continueButton = null;
    private List<GameObject> uiButtons;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();

        uiButtons = new List<GameObject>(GameObject.FindGameObjectsWithTag("UIButton"));
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            nameInput.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);
            PhotonNetwork.NickName = PlayerPrefs.GetString(PlayerNameKey);
        }
        else
        {
            uiButtons.ForEach(btn => btn.SetActive(false));
            continueButton.interactable = false;
        }
    }

    public void setName()
    {
        var name = nameInput.text;
        if (name.Length > 3)
        {
            PlayerPrefs.SetString(PlayerNameKey, name);
            nameInput.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);
            uiButtons.ForEach(btn => btn.SetActive(true));
            PhotonNetwork.NickName = name;
        }
    }

    public void onTextChanged()
    {
        continueButton.interactable = nameInput.text.Length > 3;    
    }
}
