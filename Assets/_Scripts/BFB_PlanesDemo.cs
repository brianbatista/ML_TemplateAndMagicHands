using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class BFB_PlanesDemo : MonoBehaviour
{

    public Transform _BBoxTransform;
    public Vector3 _BBoxExtents;

    public GameObject PlaneGameObject;
    private List<GameObject> _planeCache = new List<GameObject>();

    private float timeout = 5f;
    private float timeSinceLastRquest = 0f;

    private MLWorldPlanesQueryParams _queryParameters = new MLWorldPlanesQueryParams();
    public MLWorldPlanesQueryFlags _queryFlags;

    // Start is called before the first frame update
    void Start()
    {
        MLWorldPlanes.Start();
    }

    private void OnDestroy()
    {
        MLWorldPlanes.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastRquest += Time.deltaTime;
        if (timeSinceLastRquest > timeout)
        {
            timeSinceLastRquest = 0f;
            RequestPlanes();
        }
    }

    private void RequestPlanes()
    {
        _queryParameters.Flags = _queryFlags;
        _queryParameters.MaxResults = 100;
        _queryParameters.BoundsCenter = _BBoxTransform.position;
        _queryParameters.BoundsRotation = _BBoxTransform.rotation;
        _queryParameters.BoundsExtents = _BBoxExtents;

        MLWorldPlanes.GetPlanes(_queryParameters, HandleOnReceivedPlanes);
    }

    private void HandleOnReceivedPlanes(MLResult result, MLWorldPlane[] planes, MLWorldPlaneBoundaries[] boundaries)
    {

        for (int i = _planeCache.Count - 1; i >= 0; --i)
        {
            Destroy(_planeCache[i]);
            _planeCache.Remove(_planeCache[i]);
        }


        GameObject newPlane;
        for (int i = 0; i < planes.Length; ++i)
        {
            newPlane = Instantiate(PlaneGameObject);
            newPlane.transform.position = planes[i].Center;
            newPlane.transform.rotation = planes[i].Rotation;
            newPlane.transform.localScale = new Vector3(planes[i].Width, planes[i].Height, 1f);
            _planeCache.Add(newPlane);

        }
    }
}
