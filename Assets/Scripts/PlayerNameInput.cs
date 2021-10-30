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
    [SerializeField] private GameObject backButton = null;
    [SerializeField] private List<GameObject> uiButtons;
    [SerializeField] private GameObject settingsPanel = null;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        uiButtons = new List<GameObject>(GameObject.FindGameObjectsWithTag("UIButton"));

        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            //disableNameInput();
            settingsPanel.SetActive(false);
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
            nameInput.placeholder.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString(PlayerNameKey);
            currentName.text = $"Current username: {PlayerPrefs.GetString(PlayerNameKey)}";
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change name";
            //settingsPanel.SetActive(false);
        }
        else
        {
            currentName.text = "";
            nameInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Enter name...";
            backButton.SetActive(false);
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
        bool firstTime = !PlayerPrefs.HasKey(PlayerNameKey);

        PlayerPrefs.SetString(PlayerNameKey, name);

        currentName.text = "Current username: " + name;

        PhotonNetwork.NickName = name;

        if (firstTime)
        {
            settingsPanel.SetActive(false);
            disableNameInput();
        }
    }

    public void onTextChanged()
    {
        continueButton.interactable = nameInput.text.Length > 3;
    }
}
