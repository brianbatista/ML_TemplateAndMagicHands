using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class HandMeshingExamples_Particles : MonoBehaviour
    {

        #region Private Properties
        [SerializeField, Tooltip("The Hand Meshing Behavior to control")]
        private MLHandMeshingBehavior _behavior = null;

        [SerializeField, Tooltip("Material used in HandMeshing")]
        private Material _handMeshMaterial = null;

        [SerializeField, Tooltip("Key Pose to generate particles")]
        private MLHandKeyPose _keyposeForParticles = MLHandKeyPose.OpenHand;

        private const float _minimumConfidence = 0.9f;

        [SerializeField, Tooltip("GO holding the Particle System")]
        private GameObject _particleSystemToUse;

        //
        #endregion

        //
        #region Unity Methods
        /// <summary>
        /// Validate and initialize properties
        /// </summary>
        void Start()
        {
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
        #endregion

        void Update()
        {
            if (MLHands.IsStarted)
            {
                if (MLHands.Right.KeyPose == _keyposeForParticles && MLHands.Right.KeyPoseConfidence > _minimumConfidence)
                {
                    _particleSystemToUse.SetActive(true);
                    _particleSystemToUse.transform.position =  MLHands.Right.Wrist.KeyPoints[0].Position;
                    
                    // Try to add rotation as well.
                    //_particleSystemToUse.transform.rotation = MLHands.Right.Wrist.K

                    
                }

                else
                {
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
    }
}
