using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float fixedZ = 1.5f;
    [SerializeField]
    public static Camera mainCam;
    [SerializeField, Range(4, 10)]
    private int numOfActiveButtons;
    private int difficulty;
    [SerializeField]
    private List<GameObject> referenceButtons;
    private List<Color> COLORS;
    private List<int> chosenColorIndices;
    private List<Vector3> BUTTON_POSITIONS;
    
    [SerializeField]
    private bool spawnAvailable;
    public List<ReflexButton> reflexButtons { get; private set; }
    public bool reflexPhase { get; private set; }
    private float phaseStartTime;
    public float reactionTime 
    { 
        get
        {
            return Time.realtimeSinceStartup - phaseStartTime;
        }
    }

    private void Start()
    {
        mainCam = Camera.main;
        difficulty = 1;
        reflexPhase = false;
        var buttons = new List<GameObject>(GameObject.FindGameObjectsWithTag("Button"));
        var lines = new List<GameObject>(GameObject.FindGameObjectsWithTag("Line"));
        reflexButtons = new List<ReflexButton>();
        for (int i = 0; i < buttons.Count; i++)
        {
            reflexButtons.Add(new ReflexButton(buttons[i], lines[i]));
        }
        createPositionArray();
        createColorArray();

        StartCoroutine(enableReflexPhase(numOfActiveButtons * 1.5f));
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //    startReflexPhase();
    }
    public void startReflexPhase()
    {
        setReferenceButtons();
        setButtons();
        this.reflexPhase = true;
    }
    public void finishReflexPhase()
    {
        Debug.Log("finish called");
        this.reflexPhase = false;
        reflexButtons.ForEach(btn => btn.deactivate());
        referenceButtons.ForEach(btn => btn.SetActive(false));
    }
    public bool isFinalButton(int buttonIdx)
    {
        return buttonIdx == numOfActiveButtons - 1;
    }
    private IEnumerator enableReflexPhase(float secs)
    {
        while (true)
        {
            spawnAvailable = true;
            yield return new WaitForSeconds(secs);
            spawnAvailable = false;
            startReflexPhase();
            phaseStartTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(secs);
            if (reflexPhase)
                finishReflexPhase();
        }
    }

    public void setReferenceButtons()
    {
        float buttonSpacing = Screen.width * 0.09f;
        float startPosX = (Screen.width - (buttonSpacing * (this.numOfActiveButtons - 1))) / 2;
        var startPos = new Vector3(startPosX, Screen.height * 0.95f, fixedZ);

        setRandomColorIndices();

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
        var randPosIdx = getRandomPositionIdx();
        for (int i = 0; i < this.numOfActiveButtons; i++)
        {
            reflexButtons[i].setPositionAndColor(BUTTON_POSITIONS[randPosIdx[i]], COLORS[chosenColorIndices[i]]);
            reflexButtons[i].activate();
        }
    }
    private List<int> getRandomPositionIdx()
    {
        var randPosIdx = new List<int>();
        while (randPosIdx.Count < this.numOfActiveButtons)
        {
            int randIdx = Random.Range(0, BUTTON_POSITIONS.Count - 1);
            while (randPosIdx.Contains(randIdx))
            {
                randIdx = Random.Range(0, BUTTON_POSITIONS.Count - 1);
            }
            randPosIdx.Add(randIdx);
        }
        return randPosIdx;
    }
    // called once in the start
    private void createColorArray()
    {
        COLORS = new List<Color>();

        // minimum difference between colors is set to be 0.3f
        for (int i = 0; i < 4; i++)
        {
            float r = i / 3.0f;
            for (int j = 0; j < 4; j++)
            {
                float g = j / 3.0f;
                for (int k = 0; k < 4; k++)
                {
                    float b = k / 3.0f;
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
}
