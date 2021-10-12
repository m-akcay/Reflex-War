using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexButton
{
    private const float fixedZ = 1.5f;

    public readonly GameObject buttonGo;
    public readonly GameObject lineSprite;
    public ReflexButton(GameObject buttonGo, GameObject lineSprite)
    {
        this.buttonGo = buttonGo;
        buttonGo.SetActive(false);
        this.lineSprite = lineSprite;
    }
    public void activate()
    {
        this.buttonGo.SetActive(true);
        this.lineSprite.SetActive(true);
    }
    public void deactivate()
    {
        this.buttonGo.SetActive(false);
        this.lineSprite.transform.position = new Vector3(100, 100);
        this.lineSprite.SetActive(false);
    }
    public void setPositionAndColor(Vector3 pos, Color color)
    {
        this.buttonGo.transform.position = pos;
        this.buttonGo.GetComponent<SpriteRenderer>().color = color;
        this.lineSprite.GetComponent<SpriteRenderer>().color = color;
    }
    public void setLineColor(Color color)
    {
        this.lineSprite.GetComponent<SpriteRenderer>().color = color;
    }
    public void hideLine()
    {
        this.lineSprite.transform.position = new Vector3(100, 100);
    }
    public void drawLine()
    {
        var wpos = Input.mousePosition;
        wpos.z = fixedZ;
        wpos = GameManager.mainCam.ScreenToWorldPoint(wpos);

        //var activeIdx = buttonShouldBeHitIdx - 1;
        var buttonShouldBeHit = this.buttonGo;
        var lineSprite = this.lineSprite;

        float distance = Vector3.Distance(buttonShouldBeHit.transform.position, wpos);
        lineSprite.transform.localScale = new Vector3(distance * InputHandler.SCREEN_FACTOR, 0.3f, 1);

        var midPt = (buttonShouldBeHit.transform.position + wpos) / 2;
        lineSprite.transform.position = midPt;

        var screenPtButton = GameManager.mainCam.WorldToScreenPoint(buttonShouldBeHit.transform.position);
        var angle = Vector3.Angle(new Vector2(1, 0), Input.mousePosition - screenPtButton);
        if (screenPtButton.y > Input.mousePosition.y)
            angle = -angle;

        lineSprite.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
    public void finalizeLine(Vector3 finalPos)
    {
        var wpos = finalPos;
        var mpos = GameManager.mainCam.WorldToScreenPoint(wpos);
        var buttonShouldBeHit = this.buttonGo;
        var lineSprite = this.lineSprite;

        float distance = Vector3.Distance(buttonShouldBeHit.transform.position, wpos);
        lineSprite.transform.localScale = new Vector3(distance * InputHandler.SCREEN_FACTOR, 0.3f, 1);

        var midPt = (buttonShouldBeHit.transform.position + wpos) / 2;
        lineSprite.transform.position = midPt;

        var screenPtButton = GameManager.mainCam.WorldToScreenPoint(buttonShouldBeHit.transform.position);
        var angle = Vector3.Angle(new Vector2(1, 0), mpos - screenPtButton);
        if (screenPtButton.y > mpos.y)
            angle = -angle;
        
        lineSprite.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
