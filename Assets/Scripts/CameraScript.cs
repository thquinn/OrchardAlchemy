using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    static float SCROLL_STRENGTH = 1.5f;
    static float MIN_SIZE = 3, MAX_SIZE = 40;

    public BoardManagerScript boardManagerScript;
    public Camera cam;
    public EventSystem eventSystem;
    public MeshRenderer backgroundRenderer;
    public float backgroundShaderFadeMinSize, backgroundShaderFadeMaxSize;
    public Vector3? lastDragPosition;

    Vector3 vPosition;
    float vSize;

    void Update() {
        if (boardManagerScript.state.progression.cameraTakeover) {
            Vector3 targetPosition = boardManagerScript.state.progression.cameraTargetPosition;
            float targetSize = 5;
            targetPosition.z = transform.localPosition.z;
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref vPosition, .2f, float.MaxValue, Time.unscaledDeltaTime);
            if (targetSize != 0) {
                cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref vSize, .2f, float.MaxValue, Time.unscaledDeltaTime);
            }
            if (Vector3.Distance(targetPosition, transform.localPosition) < .01f) {
                boardManagerScript.state.progression.cameraTakeover = false;
            }
            return;
        }
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
