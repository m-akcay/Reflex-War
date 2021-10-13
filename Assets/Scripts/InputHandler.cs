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
    private GameManager gm;
    [SerializeField]
    private Camera mainCam;
    private bool drawing = false;
    private int nextButtonIdx = 0;
    [SerializeField]
    private GameObject spawnIndicator;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private LayerMask groundMask;

    private SpawnLimits SPAWN_LIMITS;

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
                            Debug.Log("reaction time -> " + gm.reactionTime);
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
            if (Input.GetMouseButton(0))
            {
                RaycastHit outHit;
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                bool hit = Physics.Raycast(ray, out outHit, 20, groundMask);
                if (hit)
                {
                    var rawPos = outHit.point + new Vector3(0, 2.3f, 0);
                    spawnIndicator.transform.position = clampToSpawnZone(rawPos, SPAWN_LIMITS);
                }
            }
        }
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
        localFinishReflexPhase();
    }
    private void localFinishReflexPhase()
    {
        this.drawing = false;
        nextButtonIdx = 0;
    }
}
