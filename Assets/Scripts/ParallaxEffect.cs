using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    UnityEngine.Vector2 startingPosition;

    float startingZ;
    UnityEngine.Vector2 camMoveSinceStart
    {
        get
        {
            if (cam == null) return Vector2.zero;
            return (Vector2)cam.transform.position - startingPosition;
        }
    }

    float zDistanceFromTarget
    {
        get
        {
            if (followTarget == null) return 0f;
            return transform.position.z - followTarget.position.z;
        }
    }

    float clippingPlane
    {
        get
        {
            if (cam == null) return float.PositiveInfinity;
            return (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));
        }
    }

    float parralaxFactor
    {
        get
        {
            var cp = clippingPlane;
            if (cp == 0f || float.IsInfinity(cp) || float.IsNaN(cp)) return 0f;
            return Mathf.Abs(zDistanceFromTarget) / cp;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Auto-assign common references when possible to avoid unassigned reference exceptions
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (followTarget == null && cam != null)
        {
            followTarget = cam.transform;
        }

        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector2 newPosition = startingPosition + camMoveSinceStart * parralaxFactor;
        transform.position = new UnityEngine.Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
