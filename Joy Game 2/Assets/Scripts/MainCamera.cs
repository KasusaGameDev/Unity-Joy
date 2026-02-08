using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowLetterbox2D : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.5f, -10f);
    [SerializeField, Min(0f)] private float smoothTime = 0.20f;

    [Header("Letterbox (lock aspect)")]
    [SerializeField] private bool lockAspect = true;
    [SerializeField] private Vector2 targetAspect = new Vector2(16, 9);

    private Camera cam;
    private Vector3 velocity;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (lockAspect) ApplyLetterbox();
    }

    private void OnValidate()
    {
        if (!cam) cam = GetComponent<Camera>();
        if (lockAspect) ApplyLetterbox();
        else if (cam) cam.rect = new Rect(0f, 0f, 1f, 1f);
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desired = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }

        if (lockAspect) ApplyLetterbox();
    }

    private void ApplyLetterbox()
    {
        float target = targetAspect.x / targetAspect.y;
        float window = (float)Screen.width / Screen.height;

        Rect r = cam.rect;

        if (window > target)
        {
            float scale = target / window;     // pillarbox kiri/kanan
            r.width = scale;
            r.height = 1f;
            r.x = (1f - scale) * 0.5f;
            r.y = 0f;
        }
        else
        {
            float scale = window / target;     // letterbox atas/bawah
            r.width = 1f;
            r.height = scale;
            r.x = 0f;
            r.y = (1f - scale) * 0.5f;
        }

        cam.rect = r; // viewport kamera dinormalisasi 0..1
    }
}
