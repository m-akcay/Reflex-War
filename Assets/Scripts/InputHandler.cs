using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private GameManager gm;
    [SerializeField]
    private Camera mainCam;
    private bool drawing = false;
    private int nextButtonIdx = 0;
    [SerializeField]
    private LayerMask mask;

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
            finishReflexPhase();
        }
    }
    private void finishReflexPhase()
    {
        gm.finishReflexPhase();
        this.drawing = false;
        nextButtonIdx = 0;
    }
}
