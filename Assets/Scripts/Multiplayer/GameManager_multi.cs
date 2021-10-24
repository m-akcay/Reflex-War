﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_multi : MonoBehaviourPun
{
    private bool reflexStarted = false;
    private List<int> randPosIdx;
    [SerializeField]
    private Color purple = new Color();
    private static Color PURPLE;
    [SerializeField]
    private Color green = new Color();
    private static Color GREEN;
    [SerializeField]
    private Color white = new Color();
    private static Color WHITE;
    [SerializeField]
    private Color red = new Color();
    private static Color RED;

    private const float fixedZ = 1.5f;
    [SerializeField]
    private Camera _alternativeCam = null;
    public static Camera mainCam;

    [SerializeField, Range(4, 10)]
    private int numOfActiveButtons = 4;
    private static int difficulty = 4;

    private List<GameObject> referenceButtons = null;
    private List<Color> COLORS;
    private List<int> chosenColorIndices;
    private List<Vector3> BUTTON_POSITIONS;
    private GameObject blurredPanel = null;

    [SerializeField]
    public bool spawnAvailable;
    public List<ReflexButton_multi> reflexButtons { get; private set; }
    public bool reflexPhase { get; private set; }
    public float phaseStartTime { get; private set; }
    public float reactionMultiplier
    {
        get
        {
            float reactionTime = Time.realtimeSinceStartup - phaseStartTime;
            if (reactionTime < numOfActiveButtons)
                return 1.5f;
            else if (reactionTime < numOfActiveButtons * 1.5f)
                return 1.25f;
            else
                return 1f;
        }
    }
    public List<GameObject> activeTroops;

    private void Awake()
    {
        setMainCamera();
    }
    private void Start()
    {
        WHITE = white;
        GREEN = green;
        PURPLE = purple;
        activeTroops = new List<GameObject>(GameObject.FindGameObjectsWithTag("Troop"));
        reflexPhase = false;
        referenceButtons = new List<GameObject>(GameObject.FindGameObjectsWithTag("ReferenceButton"));
        var buttons = new List<GameObject>(GameObject.FindGameObjectsWithTag("Button"));
        var lines = new List<GameObject>(GameObject.FindGameObjectsWithTag("Line"));
        Debug.Log(lines.Count);
        blurredPanel = GameObject.Find("BlurredPanel");
        blurredPanel.SetActive(false);
        reflexButtons = new List<ReflexButton_multi>();
        for (int i = 0; i < buttons.Count; i++)
        {
            reflexButtons.Add(new ReflexButton_multi(buttons[i], lines[i]));
        }
        createPositionArray();
        createColorArray();

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(enableReflexPhase());
    }
    public void startReflexPhase()
    {
        setRandomColorIndices();

        setReferenceButtons();
        setButtons();
        this.reflexPhase = true;
        photonView.RPC("setReflexPhase", RpcTarget.All, true);
        photonView.RPC("setUniversalVars", RpcTarget.All, difficulty, chosenColorIndices.ToArray(), randPosIdx.ToArray());
    }
    public void startReflexPhase_client()
    {
        setReferenceButtons_client();
        setButtons_client();
    }
    public void finishReflexPhase()
    {
        this.reflexPhase = false;
        reflexButtons.ForEach(btn => btn.deactivate());
        referenceButtons.ForEach(btn => btn.SetActive(false));
        blurredPanel.SetActive(false);
    }
    public bool isFinalButton(int buttonIdx)
    {
        return buttonIdx == numOfActiveButtons - 1;
    }
    private IEnumerator enableReflexPhase()
    {
        while (true)
        {
            if (difficulty < 11)
                this.numOfActiveButtons = difficulty;
            float waitTime = this.numOfActiveButtons;
            yield return new WaitForSeconds(waitTime);
            spawnAvailable = false;
            startReflexPhase();
            phaseStartTime = Time.realtimeSinceStartup;
            //blurredPanel.SetActive(true);
            yield return new WaitForSeconds(waitTime * 2);
            if (reflexPhase)
                finishReflexPhase();
        }
    }
    private IEnumerator enableReflexPhase_client()
    {
        if (difficulty < 11)
            this.numOfActiveButtons = difficulty;
        float waitTime = this.numOfActiveButtons * 2;
        spawnAvailable = false;
        startReflexPhase_client();
        phaseStartTime = Time.realtimeSinceStartup;
        //blurredPanel.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        if (reflexPhase)
            finishReflexPhase();
        reflexStarted = false;
    }
    public void setReferenceButtons()
    {
        float buttonSpacing = Screen.width * 0.09f;
        float startPosX = (Screen.width - (buttonSpacing * (this.numOfActiveButtons - 1))) / 2;
        var startPos = new Vector3(startPosX, Screen.height * 0.95f, fixedZ);
        
        for (int i = 0; i < this.numOfActiveButtons; i++)
        {
            var screenPos = startPos.addX(i * buttonSpacing);
            var referenceButton = referenceButtons[i];
            referenceButton.SetActive(true);
            referenceButton.transform.position = mainCam.ScreenToWorldPoint(screenPos);
            referenceButton.GetComponent<SpriteRenderer>().color = COLORS[chosenColorIndices[i]];
        }

    }
    public void setReferenceButtons_client()
    {
        float buttonSpacing = Screen.width * 0.09f;
        float startPosX = (Screen.width - (buttonSpacing * (this.numOfActiveButtons - 1))) / 2;
        var startPos = new Vector3(startPosX, Screen.height * 0.95f, fixedZ);

        for (int i = 0; i < this.numOfActiveButtons; i++)
        {
            var screenPos = startPos.addX(i * buttonSpacing);
            var referenceButton = referenceButtons[i];
            referenceButton.SetActive(true);
            referenceButton.transform.position = mainCam.ScreenToWorldPoint(screenPos);
            referenceButton.GetComponent<SpriteRenderer>().color = COLORS[chosenColorIndices[i]];
        }

    }
    public void setButtons()
    {
        setRandomPositionIdx();
        for (int i = 0; i < this.numOfActiveButtons; i++)
        {
            reflexButtons[i].setPositionAndColor(BUTTON_POSITIONS[randPosIdx[i]], COLORS[chosenColorIndices[i]]);
            reflexButtons[i].activate();
        }
    }
    public void setButtons_client()
    {
        for (int i = 0; i < this.numOfActiveButtons; i++)
        {
            reflexButtons[i].setPositionAndColor(BUTTON_POSITIONS[randPosIdx[i]], COLORS[chosenColorIndices[i]]);
            reflexButtons[i].activate();
        }
    }
    private void setRandomPositionIdx()
    {
        randPosIdx = new List<int>();
        while (randPosIdx.Count < this.numOfActiveButtons)
        {
            int randIdx = Random.Range(0, BUTTON_POSITIONS.Count - 1);
            while (randPosIdx.Contains(randIdx))
            {
                randIdx = Random.Range(0, BUTTON_POSITIONS.Count - 1);
            }
            randPosIdx.Add(randIdx);
        }
    }
    // called once in the start
    private void createColorArray()
    {
        COLORS = new List<Color>();

        // minimum difference between colors is set to be 0.3f
        for (int i = 0; i < 3; i++)
        {
            float r = i / 2.0f;
            for (int j = 0; j < 3; j++)
            {
                float g = j / 2.0f;
                for (int k = 0; k < 3; k++)
                {
                    float b = k / 2.0f;
                    this.COLORS.Add(new Color(r, g, b));
                }
            }
        }
    }
    private void setRandomColorIndices()
    {
        this.chosenColorIndices = new List<int>();
        while (chosenColorIndices.Count < this.numOfActiveButtons)
        {
            int randIdx = Random.Range(0, this.COLORS.Count - 1);
            while (chosenColorIndices.Contains(randIdx))
            {
                randIdx = Random.Range(0, this.COLORS.Count - 1);
            }
            chosenColorIndices.Add(randIdx);
        }
    }
    private void createPositionArray()
    {
        this.BUTTON_POSITIONS = new List<Vector3>();
        // 16:9 screen assumed
        int aspectWidth = 16;
        int aspectHeight = 9;

        var offset = new Vector2((Screen.width * 0.95f) / aspectWidth,
                                (Screen.height * 0.9f) / aspectHeight) + new Vector2(0.1f, 0.05f);
        var startPos = new Vector2(Screen.width * 0.05f, Screen.height * 0.05f);
        for (int i = 0; i < aspectWidth; i++)
        {
            float x = startPos.x + offset.x * i;
            for (int j = 0; j < aspectHeight; j++)
            {
                float y = startPos.y + offset.y * j;
                var buttonPos = new Vector3(x, y, fixedZ);
                buttonPos = mainCam.ScreenToWorldPoint(buttonPos);
                this.BUTTON_POSITIONS.Add(buttonPos);
            }
        }
    }
    public static void increaseDifficulty()
    {
        difficulty++;
    }
    public static int getDifficulty()
    {
        return difficulty;
    }
    public static float getDifficultyMultiplier()
    {
        return 1 + ((difficulty / 10.0f) - 0.4f);
    }
    public static Color getReactionColor(float reactionMultiplier)
    {
        switch (reactionMultiplier)
        {
            case 1f:
                return WHITE;
            case 1.25f:
                return GREEN;
            case 1.5f:
                return PURPLE;
            default:
                return WHITE;
        }
    }
    private void setMainCamera()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            mainCam = Camera.main;
        }
        else
        {
            mainCam = _alternativeCam;
            Camera.main.gameObject.SetActive(false);
            mainCam.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    private void setReflexPhase(bool reflexPhase)
    {
        this.reflexPhase = reflexPhase;
    }
    [PunRPC]
    private void setUniversalVars(int difficulty, int[] chosenColorIndices, int[] randPosIdx)
    {
        GameManager_multi.difficulty = difficulty;
        this.chosenColorIndices = new List<int>(chosenColorIndices);
        this.randPosIdx = new List<int>(randPosIdx);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Time.timeScale = 0;
            StopAllCoroutines();
        }


        if (PhotonNetwork.IsMasterClient)
            return;

        if (reflexPhase && !reflexStarted)
        {
            Debug.Log("entered here");
            reflexStarted = true;
            StartCoroutine(enableReflexPhase_client());
        }
    }
}