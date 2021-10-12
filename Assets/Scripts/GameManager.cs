﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float fixedZ = 1.5f;
    [SerializeField]
    private List<GameObject> buttons;
    [SerializeField]
    private List<GameObject> referenceButtons;
    [SerializeField]
    private Camera mainCam;
    public int difficulty { get; set; }
    [SerializeField, Range(4, 10)]
    private int numOfActiveButtons;
    private List<Color> COLORS;
    private List<int> chosenColorIndices;
    [SerializeField]
    private List<Vector3> BUTTON_POSITIONS;

    private bool reflexPhase = false;
    private bool drawing = false;
    private int buttonShouldBeHitIdx = 0;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private GameObject[] lineSprites;

    // max scaleX is 23 (full width)
    // max world distance is 3 (-1.5 to 1.5)
    // for line sprites
    private const float SCREEN_FACTOR = 23 / 3.1f;
    void Start()
    {
        foreach (var button in buttons)
        {
            button.SetActive(false);
        }

        foreach (var button in referenceButtons)
        {
            button.SetActive(false);
        }

        createPositionArray();
        createColorArray();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            setReferenceButtons();
            setButtons();
            reflexPhase = true;
        }

        if (reflexPhase)
        {
            
            if (Input.GetMouseButton(0))
            {
                //var lineRenderer = mainCam.gameObject.GetComponent<LineRenderer>();
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit outHit;
                bool hit = Physics.Raycast(ray, out outHit, 3, mask);
                var buttonShouldBeHit = buttons[buttonShouldBeHitIdx];
                if (hit && outHit.collider.gameObject == buttonShouldBeHit)
                {
                    Debug.Log(outHit.collider.gameObject.name);
                    drawing = true;
                    if (buttonShouldBeHitIdx > 0)
                    {
                        Debug.Log(buttonShouldBeHitIdx);
                        finalizeLineSprite();
                    }
                    buttonShouldBeHitIdx++;
                }

                if (drawing)
                {
                    setLineSprite();
                }
            }
            else if (drawing && Input.GetMouseButtonUp(0))
            {
                drawing = false;
                //var lineRenderer = mainCam.gameObject.GetComponent<LineRenderer>();
                //lineRenderer.positionCount--;
                buttonShouldBeHitIdx--;
            }
        }
        
    }
    private void setLineSprite()
    {
        var wpos = new Vector3().fromVec2(Input.mousePosition);
        wpos.z = fixedZ;
        wpos = mainCam.ScreenToWorldPoint(wpos);

        var activeIdx = buttonShouldBeHitIdx - 1;
        var buttonShouldBeHit = buttons[activeIdx];
        var lineSprite = lineSprites[activeIdx];

        float distance = Vector3.Distance(buttonShouldBeHit.transform.position, wpos);
        lineSprite.transform.localScale = new Vector3(distance * SCREEN_FACTOR, 0.3f, 1);

        var midPt = (buttonShouldBeHit.transform.position + wpos) / 2;
        lineSprite.transform.position = midPt;

        var screenPtButton = mainCam.WorldToScreenPoint(buttonShouldBeHit.transform.position);
        var angle = Vector3.Angle(new Vector2(1, 0), Input.mousePosition - screenPtButton);
        if (screenPtButton.y > Input.mousePosition.y)
            angle = -angle;
        Debug.Log(angle);
        lineSprite.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    [SerializeField]
    private GameObject prevButton;
    [SerializeField]
    private GameObject button;
    private void finalizeLineSprite()
    {
        var wpos = buttons[buttonShouldBeHitIdx].transform.position;
        var mpos = mainCam.WorldToScreenPoint(wpos);
        var activeIdx = buttonShouldBeHitIdx - 1;
        var buttonShouldBeHit = buttons[activeIdx];
        var lineSprite = lineSprites[activeIdx];

        float distance = Vector3.Distance(buttonShouldBeHit.transform.position, wpos);
        lineSprite.transform.localScale = new Vector3(distance * SCREEN_FACTOR, 0.3f, 1);

        var midPt = (buttonShouldBeHit.transform.position + wpos) / 2;
        lineSprite.transform.position = midPt;

        var screenPtButton = mainCam.WorldToScreenPoint(buttonShouldBeHit.transform.position);
        var angle = Vector3.Angle(new Vector2(1, 0), mpos - screenPtButton);
        if (screenPtButton.y > mpos.y)
            angle = -angle;
        Debug.Log(angle);
        lineSprite.transform.localRotation = Quaternion.Euler(0, 0, angle);
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
            var button = buttons[i];
            button.SetActive(true);
            button.transform.position = BUTTON_POSITIONS[randPosIdx[i]];
            button.GetComponent<SpriteRenderer>().color = COLORS[chosenColorIndices[i]];
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

        // minimum difference between colors is set to be 0.2f
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
