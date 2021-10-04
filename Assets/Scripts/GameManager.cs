using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float fixedZ = 1.5f;
    [SerializeField]
    private List<GameObject> buttons;
    [SerializeField]
    private List<GameObject> referenceButtons;
    private List<Material> buttonMaterials;
    private List<Material> referenceButtonMaterials;
    [SerializeField]
    private Camera mainCam;
    private float screenWidth;
    public int difficulty { get; set; }
    [SerializeField, Range(4, 10)]
    private int numOfActiveButtons;
    private List<Color> COLORS;
    private List<int> chosenColorIndices;
    [SerializeField]
    private List<Vector3> BUTTON_POSITIONS;
    void Start()
    {
        buttonMaterials = new List<Material>();
        referenceButtonMaterials = new List<Material>();
        foreach (var button in buttons)
        {
            var renderer = button.GetComponent<Renderer>();
            //renderer.enabled = false;
            buttonMaterials.Add(renderer.material);
            button.SetActive(false);
        }

        foreach (var button in referenceButtons)
        {
            var renderer = button.GetComponent<Renderer>();
            //renderer.enabled = false;
            referenceButtonMaterials.Add(renderer.material);
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
        }
        //if (Input.GetMouseButton(0))
        //{
        //    var wpos = new Vector3().fromVec2(Input.mousePosition);
        //    wpos.z = fixedZ;
        //    wpos = mainCam.ScreenToWorldPoint(wpos);
        //    Debug.Log(Input.mousePosition);
        //    mainCam.GetComponent<LineRenderer>().SetPosition(1, wpos);
        //}
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
            referenceButtonMaterials[i].SetColor("Color_2AB042FD", COLORS[chosenColorIndices[i]]);
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
            buttonMaterials[i].SetColor("Color_2AB042FD", COLORS[chosenColorIndices[i]]);
            //renderer.color = COLORS[chosenColorIndices[i]];
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
        for (int i = 0; i < 5; i++)
        {
            float r = i / 4.0f;
            for (int j = 0; j < 5; j++)
            {
                float g = j / 4.0f;
                for (int k = 0; k < 5; k++)
                {
                    float b = k / 4.0f;
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
                                (Screen.height * 0.9f) / aspectHeight);
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

    private void OnDestroy()
    {
        foreach (var mat in this.buttonMaterials)
        {
            Destroy(mat);
        }

        foreach (var mat in this.referenceButtonMaterials)
        {
            Destroy(mat);
        }
    }
}
