using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class MeshingScript : MonoBehaviour
{
    // We declare the three Materials 
    // which will have to be linked to the ones stored in the Materials folder.
    #region Public Variables
    public Material BlackMaterial;
    public Material GroundMaterial;
    public Material InactiveMaterial;
    // We declare a reference to the MLSpatialMapper. 
    // This will be helpful so as to enable and disable the world reconstruction process.
    public MLSpatialMapper Mapper;
    #endregion

    // We create a _visible variable to store the state.
    #region Private Variables
    private bool _visible = true;
    #endregion

    //Our Update() method, uses an UpdateMeshMaterial() call 
    // to update the meshes in every frame.
    #region Unity Methods
    private void Update()
    {
        UpdateMeshMaterial();
    }
    #endregion

    // We create a new ToggleMeshVisibility() method. 
    // It will be used to toggle the visibility state of the world mesh.
    #region Public Methods
    public void ToggleMeshVisibility()
    {
        _visible = _visible ? false : true;
    }
    // We create a ToggleMeshScanning() method.
    // It will be used to toggle the enabled state of the MLSpatialMapper script
    //  (the enabled property will turn true or false). 
    // When it is disabled, the world mesh will pause updating.
    public void ToggleMeshScanning()
    {
        Mapper.enabled = Mapper.enabled ? false : true;
    }
    #endregion


    #region Private Methods
    // We use an UpdateMeshMaterial() method. This method will be called in Update()
    /// Switch mesh material based on whether meshing is active and mesh is visible
    /// visible & active = ground material
    /// visible & inactive = meshing off material
    /// invisible = black mesh
    private void UpdateMeshMaterial()
    {
        // Loop over all the child mesh nodes created by MLSpatialMapper script
        for (int i = 0; i < transform.childCount; i++)
        {
            // Get the child gameObject
            GameObject gameObject = transform.GetChild(i).gameObject;
            // Get the meshRenderer component
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            // Get the assigned material
            // Using the sharedMaterial property and the material object, 
            // the materials of the GameObject meshes will be swapped between 
            // the visible materials and the BlackMaterial (invisible material).
            Material material = meshRenderer.sharedMaterial;
            if (_visible)
            {
                if (Mapper.enabled)
                {
                    if (material != GroundMaterial)
                    {
                        meshRenderer.material = GroundMaterial;
                    }
                }
                else if (material != InactiveMaterial)
                {
                    meshRenderer.material = InactiveMaterial;
                }
            }
            else if (material != BlackMaterial)
            {
                meshRenderer.material = BlackMaterial;
            }
        }
    }
    #endregion
}