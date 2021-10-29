using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    private const string PlayerNameKey = "PlayerName";
    [SerializeField] private TMP_InputField nameInput = null;
    [SerializeField] private TMP_Text currentName = null;
    [SerializeField] private Button continueButton = null;
    [SerializeField] private List<GameObject> uiButtons;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        uiButtons = new List<GameObject>(GameObject.FindGameObjectsWithTag("UIButton"));

        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            //disableNameInput();
            GameObject.Find("SettingsPanel").SetActive(false);
            PhotonNetwork.NickName = PlayerPrefs.GetString(PlayerNameKey);
        }
        else
        {
            enableNameInput();
        }
    }

    public void enableNameInput()
    {
        uiButtons.ForEach(btn => btn.SetActive(false));
        
        continueButton.gameObject.SetActive(true);
        continueButton.interactable = false;
        
        nameInput.gameObject.SetActive(true);
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            nameInput.text = PlayerPrefs.GetString(PlayerNameKey);
            currentName.text = $"Current username: {nameInput.text}";
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change name";
        }
        else
        {
            currentName.text = "";
            nameInput.text = "Enter name...";
        }
    }

    public void disableNameInput()
    {
        nameInput.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        uiButtons.ForEach(btn => btn.SetActive(true));
    }

    public void setName()
    {
        var name = nameInput.text;
        PlayerPrefs.SetString(PlayerNameKey, name);

        currentName.text = "Current username: " + name;

        PhotonNetwork.NickName = name;
    }

    public void onTextChanged()
    {
        continueButton.interactable = nameInput.text.Length > 3;
    }
}
