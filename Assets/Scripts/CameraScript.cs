using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    static float SCROLL_STRENGTH = 1.5f;
    static float MIN_SIZE = 3, MAX_SIZE = 40;

    public Camera cam;
    public EventSystem eventSystem;
    public MeshRenderer backgroundRenderer;
    public float backgroundShaderFadeMinSize, backgroundShaderFadeMaxSize;
    public Vector3? lastDragPosition;

    void Update() {
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        // Pan.
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
            if (lastDragPosition != null) {
                transform.Translate(lastDragPosition.GetValueOrDefault() - mouseWorldPosition);
                mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            }
            lastDragPosition = mouseWorldPosition;
        } else {
            lastDragPosition = null;
        }
        // Zoom.
        if (eventSystem.IsPointerOverGameObject()) {
            return;
        }
        float currentSize = cam.orthographicSize;
        float newSize = Mathf.Clamp(currentSize - Input.mouseScrollDelta.y * SCROLL_STRENGTH, MIN_SIZE, MAX_SIZE);
        cam.orthographicSize = newSize;
        if (newSize != currentSize) {
            transform.Translate((mouseWorldPosition - transform.localPosition) * (1 - newSize / currentSize));
            float backgroundShaderFade = Mathf.InverseLerp(backgroundShaderFadeMinSize, backgroundShaderFadeMaxSize, newSize);
            backgroundRenderer.material.SetFloat("_Fade", backgroundShaderFade);
        }
    }
}