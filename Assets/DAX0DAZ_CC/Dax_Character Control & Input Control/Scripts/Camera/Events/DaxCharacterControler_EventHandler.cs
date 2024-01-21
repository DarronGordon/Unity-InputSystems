using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DaxCharacterControler_EventHandler : MonoBehaviour
{
    public static DaxCharacterControler_EventHandler Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region [[[[[ GROUND ]]]]]

    public static event Action OnStepEvent;
    public void CallOnStepEvent()
    {
        if (OnStepEvent != null)
        {
            OnStepEvent();
        }
    }

    public static event Action<bool> OnSprintChangeEvent;
    public void CallOnSprintChangeEvent(bool d)
    {
        if (OnSprintChangeEvent != null)
        {
            OnSprintChangeEvent(d);
        }
    }

    public static event Action OnJumpEvent;
    public void CallOnJumpEvent()
    {
        if (OnJumpEvent != null)
        {
            OnJumpEvent();
        }
    }

    public static event Action OnLandEvent;
    public void CallOnLandEvent()
    {
        if (OnLandEvent != null)
        {
            OnLandEvent();
        }
    }

    public static event Action OnStairStepEvent;
    public void CallOnStairStepEvent()
    {
        if (OnStairStepEvent != null)
        {
            OnStairStepEvent();
        }
    }

    #endregion

    #region [[[[[ INTERACT  ]]]]]

        public static event Action OnObjectInteractEvent;
        public void CallOnObjectInteractEvent()
        {
            if (OnObjectInteractEvent != null)
            {
                OnObjectInteractEvent();
            }
        }
       
    #endregion
}


