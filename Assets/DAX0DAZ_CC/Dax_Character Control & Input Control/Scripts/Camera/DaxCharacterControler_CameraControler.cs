using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DaxCharacterControler_CameraControler : MonoBehaviour
{
    [SerializeField] private CharacterControlData_SO characterData;

    [SerializeField] private CinemachineVirtualCamera Vcamera;

    float targetFOV;

    private void Start()
    {
        targetFOV = characterData.DefaultFov;
    }

    private void Update()
    {
        if(targetFOV != Vcamera.m_Lens.FieldOfView) 
        {
            Vcamera.m_Lens.FieldOfView = Mathf.MoveTowards(Vcamera.m_Lens.FieldOfView, targetFOV, characterData.SlewRate * Time.deltaTime);
        }
    }
   
    public void OnSprintingStateChanged(bool isRunning)
    {
        targetFOV = isRunning ? characterData.RunningFov : characterData.WalkingFOV;
    }
}
