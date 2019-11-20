using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;


public class _objectSpawner : MonoBehaviour
{
    public enum ButtonsState
    {
        Normal,
        Pressed,
        Released
    }

    public ButtonsState _buttonState;

    public GameObject prefabSpawn;

    public MLInputController _controller;

    private Vector3 _headPose;

    // Start is called before the first frame update
    void Start()
    {
        MLInput.Start();

        MLInput.OnControllerButtonDown += HandleOnButtonDown;
        MLInput.OnControllerButtonUp += HandleOnButtonUp;

        _buttonState = ButtonsState.Normal; // Check the difference between them.

        _controller = MLInput.GetController(MLInput.Hand.Left);
    }

    private void HandleOnButtonUp(byte controllerId, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {
            _buttonState = ButtonsState.Released;

        }
    }

    private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {
            raycastHandler();

            _buttonState = ButtonsState.Pressed;
        }
        
    }

    private void raycastHandler()
    {
        _headPose = transform.forward;

        RaycastHit _hit;

        if (Physics.Raycast(_controller.Position, _headPose, out _hit))
        {
            CubeController(_hit);
        }
    }

    private void CubeController(RaycastHit hit)
    {
        GameObject go = Instantiate(prefabSpawn, hit.point, Quaternion.identity);
        go.transform.LookAt(gameObject.transform.position);
    }



    // Update is called once per frame
    void Update()
    {
        if (_buttonState == ButtonsState.Released)
        {
            _buttonState = ButtonsState.Normal;
        }
    }
}
