using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

namespace MagicLeap
{
    public class HandMeshingExamples_MultipleHandsParticle : MonoBehaviour
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

        [SerializeField, Tooltip("Right Hand - GO holding the Particle System")]
        private GameObject _particleSystemToUse_Right;

        [SerializeField, Tooltip("Left Hand - GO holding the Particle System")]
        private GameObject _particleSystemToUse_Left;

        [SerializeField]
        private Text _statusLeftHand;

        [SerializeField]
        private Text _statusRightHand;


        //// Not Serializible
        private const float _minimumConfidence = 0.9f;
        
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
                if (MLHands.Left.KeyPose == _keyposeForParticles && MLHands.Left.KeyPoseConfidence > _minimumConfidence)
                {
                    _statusLeftHand.text = "Left hand " + _keyposeForParticles + ": <color=green>Yes</color>";
                    _particleSystemToUse_Left.SetActive(true);
                    _particleSystemToUse_Left.GetComponent<ParticleSystem>().Play();
                    _particleSystemToUse_Left.transform.position =  new Vector3(MLHands.Left.Center.x, MLHands.Left.Center.y, MLHands.Left.Center.z + _particleOffset); 
                }

                if (MLHands.Right.KeyPose == _keyposeForParticles && MLHands.Right.KeyPoseConfidence > _minimumConfidence)
                {
                    _statusRightHand.text = "Right hand " + _keyposeForParticles + ": <color=green>Yes</color>";
                    _particleSystemToUse_Right.SetActive(true);
                    _particleSystemToUse_Right.GetComponent<ParticleSystem>().Play();
                    _particleSystemToUse_Right.transform.position =  new Vector3(MLHands.Right.Center.x, MLHands.Right.Center.y, MLHands.Right.Center.z + _particleOffset); 
                }

                if (MLHands.Left.KeyPoseConfidence <= (_minimumConfidence/ 3.0f))
                {
                    _statusLeftHand.text = "Left hand " + _keyposeForParticles + ": <color=red>No</color>";                    
                    _particleSystemToUse_Left.SetActive(false);
                    _particleSystemToUse_Left.GetComponent<ParticleSystem>().Stop();
                }

                if (MLHands.Right.KeyPoseConfidence <= (_minimumConfidence/ 3.0f))
                {
                    _statusRightHand.text = "Right hand " + _keyposeForParticles + ": <color=red>No</color>";                    
                    _particleSystemToUse_Right.SetActive(false);
                    _particleSystemToUse_Right.GetComponent<ParticleSystem>().Stop();
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
