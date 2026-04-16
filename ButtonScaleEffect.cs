using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonScaleEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    [Tooltip("How much the button shrinks when pressed (0.9 = 90% of original size)")]
    public float pressedScale = 0.9f;

    [Tooltip("How fast the scale animation plays")]
    public float scaleSpeed = 10f;

    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    void Start()
    {
        originalScale = transform.localScale;
    }

    // Called when mouse/finger presses down on the button
    public void OnPointerDown(PointerEventData eventData)
    {
        ScaleTo(originalScale * pressedScale);
    }

    // Called when mouse/finger releases on the button
    public void OnPointerUp(PointerEventData eventData)
    {
        ScaleTo(originalScale);
    }

    // Called if the cursor leaves the button while held — snaps back so it doesn't get stuck
    public void OnPointerExit(PointerEventData eventData)
    {
        ScaleTo(originalScale);
    }

    void ScaleTo(Vector3 target)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleRoutine(target));
    }

    IEnumerator ScaleRoutine(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * scaleSpeed);
            yield return null;
        }
        transform.localScale = target;
    }
}