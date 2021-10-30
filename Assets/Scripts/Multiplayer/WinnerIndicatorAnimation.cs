using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerIndicatorAnimation : MonoBehaviour
{
    [SerializeField]
    private Transform winnerIndicator_target = null;
    private bool waitIsOver = false;
    private bool waiting = false;
    private float initialDistance;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            winnerIndicator_target.position = winnerIndicator_target.position.fromZ(-winnerIndicator_target.position.z);
        initialDistance = Vector3.Distance(this.transform.position, winnerIndicator_target.position);

    }

    private void Update()
    {
        if (GameManager_multi.GameOver && !waiting)
            StartCoroutine(wait());
        else if (waitIsOver)
        {
            moveToDesiredPos();
        }
    }
    private void moveToDesiredPos()
    {
        var desiredPos = winnerIndicator_target.position;
        var posDiff = desiredPos - this.transform.position;
        var distance = posDiff.magnitude;

        if (distance > 0.05f)
        {
            var moveDir = posDiff.normalized;

            var fastest = 8.0f;
            var slowest = 1.0f;
            var lerp_tValue = distance / initialDistance;

            var speed = slowest * (1 - lerp_tValue) + fastest * lerp_tValue;

            //Debug.Log(this.gameObject.name
            //    + "\nspeed -> " + speed
            //    + "\ndistance -> " + distance
            //    + "\ninitialDistance ->" + initialDistance
            //    + "\ndesiredPos -> " + desiredPos
            //    + "\nmoveTo -> " + moveTo);
            transform.Translate(moveDir * speed * Time.deltaTime);
        }
        else
        {
            // finish animation
            waitIsOver = false;
            return;
        }
    }

    private IEnumerator wait()
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(1f);
        waitIsOver = true;
    }
}
