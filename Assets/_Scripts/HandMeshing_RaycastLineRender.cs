using UnityEngine;
using UnityEngine.XR.MagicLeap;
using System.Collections;
using UnityEngine.UI;

namespace MagicLeap
{
    public class HandMeshing_RaycastLineRender : MonoBehaviour
    {

        #region Private Properties
        [SerializeField, Tooltip("The Hand Meshing Behavior to control")]
        private MLHandMeshingBehavior _behavior = null;

        [SerializeField, Tooltip("Material used in HandMeshing")]
        private Material _handMeshMaterial = null;

        [SerializeField, Tooltip("Key Pose to generate particles")]
        private MLHandKeyPose _keyposeForParticles = MLHandKeyPose.OpenHand;

        [SerializeField, Tooltip("Offset of the particle system from the hands")]
        private float _particleOffset = 0.1f;

        [SerializeField]
        private Text _statusLeftHand;

        [SerializeField]
        private Text _statusRaycast;

        // Particle effect when object hit by raycast is destroyed.
        [SerializeField]
        private GameObject _destroyParticleEffect;

        //// Line Renderer Stuff

        public Transform startPoint;
        public Transform endPoint;
        // Laser line
        public LineRenderer laserLine;

        //// Not Serializible
        private const float _minimumConfidence = 0.9f;
        
        // ML camera
        private Camera _camera;

        // Maximum distance raycast shoots
        private float _rayMaxDistance = 20f;

        #endregion

        //
        #region Unity Methods
        /// <summary>
        /// Validate and initialize properties
        /// </summary>

        void Awake()
        {
            // Starting the laser line and disabling it.
            laserLine = laserLine.GetComponent<LineRenderer>();
            laserLine.startWidth = .01f;
            laserLine.endWidth = .01f;
            laserLine.transform.position = new Vector3 (0f, 0f, 0f);
            laserLine.enabled = false;
            //
            
            _camera = Camera.main;
            // Checks if camera does, in fact, exist.
            if (_camera == null)
            {
                Debug.LogError("Error: _camera is null, disabling script.");
                enabled = false;
                return;
            }
        }

        void Start()
        {
            MLResult result = MLHands.Start();

            if (!result.IsOk)
            {
                Debug.LogError("Error: HandMeshingExample failed to start MLHands, disabling script.");
                enabled = false;
                return;
            }

            MLHands.KeyPoseManager.EnableKeyPoses(new[] { _keyposeForParticles }, true, true);
            MLHands.KeyPoseManager.SetPoseFilterLevel(MLPoseFilterLevel.ExtraRobust);
            MLHands.KeyPoseManager.SetKeyPointsFilterLevel(MLKeyPointFilterLevel.ExtraSmoothed);

            _behavior.MeshMaterial = _handMeshMaterial;

            _statusRaycast.text = "Raycast <color=red>hasn't hit.</color>";
        }
        #endregion

        void Update()
        {
            RaycastHit hit;
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, _rayMaxDistance))
                {
                    if(!hit.transform.CompareTag("mesh"))
                    {
                    _statusRaycast.text = "Raycast <color=green>hit</color>: " + hit.collider.gameObject.name;                    
                    }
                }

            if (MLHands.IsStarted)
            {
                if (MLHands.Left.KeyPoseConfidence <= (_minimumConfidence/ 3.0f))
                {
                _statusLeftHand.text = _keyposeForParticles + ": <color=red>No</color>";
                laserLine.SetPosition(0, Vector3.zero);
                laserLine.SetPosition(1, Vector3.zero);
                laserLine.enabled = false;
                }

                if (MLHands.Left.KeyPose == _keyposeForParticles && MLHands.Left.KeyPoseConfidence > _minimumConfidence)
                {
                    _statusLeftHand.text = _keyposeForParticles + ": <color=green>Yes</color>";

                    RaycastHit hit_2;
                    // If raycast hits...
                    if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit_2, _rayMaxDistance))
                    {
                        _statusRaycast.text = "Raycast <color=green>hit</color>: " + hit_2.collider.gameObject.name;
                        if(hit_2.transform.CompareTag("destroyable"))
                        {
                            laserLine.enabled = true;
                            laserLine.SetPosition(0, new Vector3 (MLHands.Left.Center.x, MLHands.Left.Center.y + _particleOffset, MLHands.Left.Center.z));
                            laserLine.SetPosition(1, hit_2.transform.position);
                            GameObject _destroyParticleEffectClone = Instantiate (_destroyParticleEffect, hit_2.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
                            Destroy(hit_2.transform.gameObject, 1.0f);
                            Destroy (_destroyParticleEffectClone, 1.5f);
                        }
                    }

                    // If it doesn't...
                    else
                    {
                        _statusRaycast.text = "Raycast <color=red>hasn't hit anything.</color>";                    
                        laserLine.enabled = false;
                    }  
                }
            }
        }

        void OnDestroy()
        {
            if (MLHands.IsStarted)
            {
                MLHands.KeyPoseManager.DisableAllKeyPoses();
                MLHands.Stop();
            }
        }
    }
}
