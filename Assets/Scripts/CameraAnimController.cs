using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamState
{
    CamDefault,
    CamMoving,
    CamBattle
}

public class CameraAnimController : MonoBehaviour
{
    [SerializeField] Transform player;

    private Vector3 standDiff = Vector3.zero;
    private CamState cCamState = CamState.CamDefault;

    public CamState CCamState { get => cCamState; set => cCamState = value; }

    private void Awake()
    {
        standDiff = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        ResolvingCamByState();
    }

    private void ResolvingCamByState()
    {
        switch (cCamState)
        {
            case CamState.CamDefault:
                ControlCamDefault();
                break; 
            case CamState.CamMoving:
                ControlCamMoving();
                break; 
            case CamState.CamBattle:
                ControlCamBattle();
                break; 
        }
    }

    private void ControlCamDefault()
    {
        var newPos = player.position + standDiff;
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);

        var lookPos = player.position + Vector3.up;
        var dir = lookPos - transform.position;
        var rot = Quaternion.FromToRotation(dir, player.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime);
    }

    private void ControlCamMoving()
    {
        var newPos = player.position - player.forward*4;
        newPos.y += 3;
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime*2);

        var lookPos = player.position + Vector3.up;
        var dir = lookPos - transform.position;
        var rot = Quaternion.FromToRotation(dir, player.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime*2);
    }

    private void ControlCamBattle()
    {

    }
}
