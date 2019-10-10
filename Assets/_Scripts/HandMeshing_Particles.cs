// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class HandMeshing_Particles : MonoBehaviour
    {

        #region Private Properties
        ///
        [SerializeField, Tooltip("The Hand Meshing Behavior to control")]
        private MLHandMeshingBehavior _behavior = null;

        [SerializeField, Tooltip("Material used in Handmeshing")]
        private Material _handMeshMaterial = null;

        [SerializeField, Tooltip("Key Pose to generate particles")]
        private MLHandKeyPose _keyposeForParticles = MLHandKeyPose.OpenHand;

        private const float _minimumConfidence = 0.8f;

        
        [SerializeField, Tooltip("GO holding the Particle System")]
        private GameObject _particleSystemToUse;

        
        ///
        #endregion

        #region Unity Methods
        /// <summary>
        /// Validate and initialize properties
        /// </summary>
        void Start()
        {
            if (_behavior == null)
            {
                Debug.LogError("Error: HandMeshingExample._behavior is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_handMeshMaterial == null)
            {
                Debug.LogError("Error: HandMeshingExample._wireframeMaterial is not set, disabling script.");
                enabled = false;
                return;
            }

            // Note: MLHands is not necessary to use Hand Meshing.
            // It is only used for switching the render modes in this example.
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

        }


        void Update()
        {
            if(MLHands.IsStarted)
            {  
                if(MLHands.Right.KeyPose == _keyposeForParticles && MLHands.Right.KeyPoseConfidence > _minimumConfidence)
                {
                    Debug.Log("Activate particles.");
                    _particleSystemToUse.SetActive(true);
                }
                else
                {
                    Debug.Log("Turn off particles.");
                    _particleSystemToUse.SetActive(false);
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
        #endregion
    }
}
