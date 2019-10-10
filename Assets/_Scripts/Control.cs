using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Control : MonoBehaviour
{
    // Initially, three different button states are declared 
    // (Normal, Pressed and JustReleased); they all refer to the 
    // Bumper button of the Control.
    #region Public Variables 
    public enum ButtonStates
    {
        Normal,
        Pressed,
        JustReleased
    };
    public ButtonStates BtnState;
    #endregion

    #region Private Variables
    // The float TIME_MESH_SCANNING_TOGGLE constant is the 
    // time that the Bumper needs to be pressed down to toggle mesh scanning.
    private const float TIME_MESH_SCANNING_TOGGLE = 3.0f;

    // The bool _held is used so that the mesh visibility 
    // does not get toggled when the bumper is released.
    private bool _held = false;

    // The float _startTime is used to calculate how long the Bumper is being held down.
    private float _startTime = 0.0f;

    // A reference to the MeshingScript is created in order to have access to its public methods.

    private MeshingScript _meshing;
    #endregion

    // MLInput.Start() and MLInput.Stop() methods are essential; we need to use both of them.
    #region Unity Methods
    private void Start()
    {
        // Start input
        MLInput.Start();

        // Add button callbacks
        MLInput.OnControllerButtonDown += HandleOnButtonDown;
        MLInput.OnControllerButtonUp += HandleOnButtonUp;

        // Assign meshing component
        _meshing = GetComponent<MeshingScript>();

        // Initial State of the Control is Normal
        BtnState = ButtonStates.Normal;
    }

    // These methods (HandleOnButtonDown/Up) have to be dereferenced in the OnDestroy() method.
    private void OnDestroy()
    {
        // Stop input
        MLInput.Stop();

        // Remove button callbacks
        MLInput.OnControllerButtonDown -= HandleOnButtonDown;
        MLInput.OnControllerButtonUp -= HandleOnButtonUp;
    }

    // BtnState is what controls the entire Bumper 
    private void Update()
    {
        // Bumper button held down - toggle scanning if timer reaches max
        if (GetTime() >= TIME_MESH_SCANNING_TOGGLE && BtnState == ButtonStates.Pressed)
        {
            _held = true;
            _startTime = Time.time;
            _meshing.ToggleMeshScanning();
        }
        // Bumper was just released - toggle visibility
        else if (BtnState == ButtonStates.JustReleased)
        {
            // When Bumper is released, resets _startTime and changes BtnState to Normal.
            BtnState = ButtonStates.Normal;
            _startTime = 0.0f;
            if (!_held)
            {
                _meshing.ToggleMeshVisibility();
            }
            else
            {
                _held = false;
            }
        }
    }
    #endregion

    // the float GetTime() method will return either this difference 
    // (if the timer has been initialized), or the value of -1f.
    #region Private Methods
    public float GetTime()
    {
        float returnTime = -1.0f;
        // if _startTime has started...
        if (_startTime > 0.0f)
        {
            // difference between when bumper started getting pressed
            // and when was released.
            returnTime = Time.time - _startTime;
        }
        return returnTime;
    }
    #endregion

    // Receiving the Button Up and Button Down callbacks.
    #region Event Handlers
    void HandleOnButtonUp(byte controller_id, MLInputControllerButton button)
    {
        // Callback - Button Up
        if (button == MLInputControllerButton.Bumper)
        {
            BtnState = ButtonStates.JustReleased;
        }
    }

    void HandleOnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        // Callback - Button Down
        // MLInputControllerButton.Bumper input (that is when a Bumper input is registered).
        if (button == MLInputControllerButton.Bumper)
        {
            // Start bumper timer
            _startTime = Time.time;
            BtnState = ButtonStates.Pressed;
        }
    }
    #endregion
}
