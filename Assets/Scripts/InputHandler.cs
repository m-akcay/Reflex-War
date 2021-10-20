using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private struct SpawnLimits
    {
        public float right;
        public float left;
        public float up;
        public float down;
        public SpawnLimits(float right, float left, float up, float down)
        {
            this.right = right;
            this.left = left;
            this.up = up;
            this.down = down;
        }
    }
    [SerializeField]
    private Color purple;
    [SerializeField]
    private Color green;
    [SerializeField]
    private Color white;
    [SerializeField]
    private Color red;

    [SerializeField]
    private GameManager gm;
    [SerializeField]
    private Camera mainCam;
    private bool drawing = false;
    [SerializeField]
    private int nextButtonIdx = 0;
    [SerializeField]
    private GameObject spawnIndicator;
    private Material spawnMaterial;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private LayerMask groundMask;

    private SpawnLimits SPAWN_LIMITS;
    private float reactionMultiplier;
    private bool spawnIndicatorIsRed;
    [SerializeField]
    private Color spawnColor;
    private Vector3 lastAvailableSpawnPos;

    // max scaleX is 23 (full width)
    // max world distance is 3 (-1.5 to 1.5)
    // for line sprites
    public const float SCREEN_FACTOR = 23 / 3.1f;
    public bool firstButton
    {
        get
        {
            return nextButtonIdx > 0;
        }
    }
    void Start()
    {
        SPAWN_LIMITS = new SpawnLimits(8.35f, -8.35f, -5.7f, -8.75f);
        spawnMaterial = spawnIndicator.GetComponent<Renderer>().material;
        reactionMultiplier = 1f;
        spawnIndicatorIsRed = true;
        spawnColor = white;
        lastAvailableSpawnPos = new Vector3();
    }

    void Update()
    {
        if (gm.reflexPhase)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit outHit;
                bool hit = Physics.Raycast(ray, out outHit, 3, mask);
                var buttonShouldBeHit = gm.reflexButtons[nextButtonIdx].buttonGo;
                if (hit && outHit.collider.gameObject == buttonShouldBeHit)
                {
                    drawing = true;
                    if (nextButtonIdx > 0)
                    {
                        var finalPos = buttonShouldBeHit.transform.position;
                        gm.reflexButtons[nextButtonIdx - 1].finalizeLine(finalPos);
                        if (gm.isFinalButton(nextButtonIdx))
                        {
                            finishReflexPhase();
                            gm.spawnAvailable = true;
                            setReactionMultiplier();
                            spawnMaterial.SetColor("Color_D03AD5CF", spawnColor);
                            Debug.Log("reaction time -> " + this.reactionMultiplier);
                            return;
                        }
                    }
                    nextButtonIdx++;
                }

                if (drawing)
                {
                    gm.reflexButtons[nextButtonIdx - 1].drawLine();
                }
            }
            else if (drawing && Input.GetMouseButtonUp(0))
            {
                drawing = false;
                nextButtonIdx--;
                gm.reflexButtons[nextButtonIdx].hideLine();
            }
        }
        else
        {
            resetVariables();
            if (gm.spawnAvailable)
            {
                Vector3 spawnPos;
                bool l_spawnPositionAvailable = spawnPositionAvailable(Input.mousePosition, out spawnPos);
                setUpSpawner(l_spawnPositionAvailable, spawnPos);

                if (l_spawnPositionAvailable)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        var prefab = Resources.Load("Prefabs/Shooter") as GameObject;
                        var troop = Instantiate(prefab, spawnIndicator.transform.position, Quaternion.identity);
                        troop.GetComponent<Archer>().init(this.reactionMultiplier);
                        gm.spawnAvailable = false;
                        gm.activeTroops.Add(troop);
                    }
                }
            }
            else
            {
                spawnIndicator.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    private bool spawnPositionAvailable(Vector3 mousePos, out Vector3 pos)
    {
        RaycastHit outHit;
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        bool hit = Physics.Raycast(ray, out outHit, 200, groundMask);
        bool l_spawnPositionAvailable = false;
        pos = lastAvailableSpawnPos;

        if (hit)
        {
            var rawPos = outHit.point + new Vector3(0, 2.3f, 0);
            pos = clampToSpawnZone(rawPos, SPAWN_LIMITS);

            l_spawnPositionAvailable = posOccupied(spawnIndicator.transform.position);
        }

        lastAvailableSpawnPos = pos;

        return l_spawnPositionAvailable;
    }
    private bool posOccupied(Vector3 pos)
    {
        foreach (var troop in gm.activeTroops)
        {
            if (Vector2.Distance(troop.transform.position.xz(), pos.xz()) < 2f)
                return false;
        }
        return true;
    }
    private void setUpSpawner(bool isAvailable, Vector3 spawnPos)
    {
        var spawnRenderer = spawnIndicator.GetComponent<Renderer>();
        if (!spawnRenderer.enabled)
            spawnRenderer.enabled = true;
        spawnIndicator.transform.position = spawnPos;

        if (isAvailable)
        {

            if (spawnIndicatorIsRed)
            {
                spawnMaterial.SetColor("Color_D03AD5CF", spawnColor);
                spawnIndicatorIsRed = false;
            }
        }
        else
        {
            spawnIndicatorIsRed = true;
            spawnMaterial.SetColor("Color_D03AD5CF", red);
        }
    }
    private void setReactionMultiplier()
    {
        this.reactionMultiplier = gm.reactionMultiplier;
        GameManager.SPAWN_COLOR_MAP.TryGetValue(reactionMultiplier, out spawnColor);
        //switch (this.reactionMultiplier)
        //{
        //    case 1f:
        //        spawnColor = white;
        //        break;
        //    case 1.25f:
        //        spawnColor = green;
        //        break;
        //    case 1.5f:
        //        spawnColor = purple;
        //        break;
        //    default:
        //        spawnColor = white;
        //        break;
        //}
    }
    private Vector3 clampToSpawnZone(Vector3 pos, SpawnLimits limits)
    {
        Vector3 clampedPos;
        clampedPos.x = Mathf.Clamp(pos.x, limits.left, limits.right);
        clampedPos.z = Mathf.Clamp(pos.z, limits.down, limits.up);
        clampedPos.y = pos.y;
        return clampedPos;
    }
    private void finishReflexPhase()
    {
        gm.finishReflexPhase();
        resetVariables();
    }
    private void resetVariables()
    {
        this.drawing = false;
        nextButtonIdx = 0;
    }
    private void OnDestroy()
    {
        Destroy(spawnMaterial);
    }
}
