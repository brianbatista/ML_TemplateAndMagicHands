using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class EyeTracker : MonoBehaviour
{
	// Start is called before the first frame update

	

	public Camera mainCamera;

	public GameObject cursor;

	private Vector3 cHeadPose;

	public GameObject prefab;

	public ButtonStates BtnState;

	public enum ButtonStates
	{
		Normal,
		Pressed,
		JustReleased
	};

	private Vector3 headPose;

	void Start()
	{
		MLInput.Start();
		MLEyes.Start();

		MLInput.OnControllerButtonDown += HandleOnButtonDown;
		MLInput.OnControllerButtonUp += HandleOnButtonUp;

		//_meshing = GetComponent<MeshingScript>();

		BtnState = ButtonStates.Normal;


	}



	// Update is called once per frame
	void Update()
	{

		if (MLEyes.IsStarted)
		{

			cHeadPose = MLEyes.FixationPoint - mainCamera.transform.position;

			RaycastHit _hit;

			if (Physics.Raycast(mainCamera.transform.position, headPose, out _hit))
			{

				cursor.transform.position = _hit.point;
				cursor.transform.LookAt(_hit.normal + _hit.point);

				if (BtnState == ButtonStates.JustReleased)
				{
					BtnState = ButtonStates.Normal;
				}

			}
		}


	}

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
		if (button == MLInputControllerButton.Bumper)
		{
			raycastHandler();
			BtnState = ButtonStates.Pressed;
		}
	}


	void raycastHandler()
	{
		if (MLEyes.IsStarted)
		{

			headPose = MLEyes.FixationPoint - mainCamera.transform.position;

			RaycastHit _1hit;

			if (Physics.Raycast(mainCamera.transform.position, headPose, out _1hit))
			{

				StartCoroutine(CubeController(_1hit));

			}
		}

	}

	private IEnumerator CubeController(RaycastHit _1hit)
	{
		Vector3 orient = _1hit.normal; /* +_1hit.point*/
		GameObject go = Instantiate(prefab, _1hit.point, Quaternion.Euler(0, 0, 0));
		go.transform.LookAt(orient);
		yield return new WaitForSeconds(15);
		Destroy(go);
	}


	
}