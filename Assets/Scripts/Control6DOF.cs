//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.XR.MagicLeap;

//public class Control6DOF : MonoBehaviour
//{
//    #region Public Variables
//    public GameObject _speakerAnchor;
//    public GameObject _textDisplay;
//    #endregion

//    #region Private Variables
//    private MLInput.Controller _control;
//    private bool _bumperBtn, _homeBtnPressed;
//    private bool _triggerLatch, _showDisplay = true;
//    #endregion

//    #region Unity Methods
//    void Start()
//    {
//        MLInput.Start();
//        MLInput.OnControllerButtonDown += OnButtonDown;
//        MLInput.OnControllerButtonUp += OnButtonUp;
//        _control = MLInput.GetController(MLInput.Hand.Left);
//    }
//    void OnDestroy()
//    {
//        MLInput.OnControllerButtonDown -= OnButtonDown;
//        MLInput.OnControllerButtonUp -= OnButtonUp;
//        MLInput.Stop();
//    }
//    void Update()
//    {
//        checkTrigger();

//        if (_bumperBtn && _control.TriggerValue == 1.0f)
//        {
//            //_speakerAnchor.transform.rotation = _control.Orientation;
//            Vector3 speakerOrientation = new Vector3(0.0f, _control.Orientation.eulerAngles.y, 0.0f);
//            _speakerAnchor.transform.eulerAngles = speakerOrientation;
//            _speakerAnchor.transform.localPosition = _control.Position;
//            _speakerAnchor.transform.Translate(0.0f, 0.0f, 0.1f);
//        }

//        if (_homeBtnPressed)
//        {
//            _showDisplay = !_showDisplay;
//            _homeBtnPressed = false;
//        }


//        if (_showDisplay)
//        {
//            _textDisplay.GetComponent<Text>().enabled = true;
//        }
//        else
//        {
//            _textDisplay.GetComponent<Text>().enabled = false;
//        }
//    }

//    private void checkTrigger()
//    {
//        if (_control.TriggerValue > 0.2f && _triggerLatch == false)
//        {
//            _triggerLatch = true;
//        }
//        else if (_control.TriggerValue == 0.0f)
//        {
//            _triggerLatch = false;
//        }
//    }

//    private void OnButtonDown(byte controllerId, MLInput.Controller.Button button)
//    {
//        if (button == MLInput.Controller.Button.Bumper)
//        {
//            _bumperBtn = true;
//        }
//    }

//    private void OnButtonUp(byte controllerId, MLInput.Controller.Button button)
//    {
//        if (button == MLInput.Controller.Button.Bumper)
//        {
//            _bumperBtn = false;
//        }
//        if (button == MLInput.Controller.Button.HomeTap)
//        {
//            _homeBtnPressed = true;
//        }
//    }
//    #endregion
//}
