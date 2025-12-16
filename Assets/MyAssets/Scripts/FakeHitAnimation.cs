using System;
using System.Collections;
using UnityEngine;

public class FakeHitAnimation : MonoBehaviour
{
    [Header("Scale")] 
    public Vector3 hitScale = new Vector3(1.2f, 0.8f, 1.2f);
    public float scaleDuration = 0.08f;

    [Header("Push Back")] 
    public float pushDistance = 0.15f;
    public float pushDuration = 0.08f;

    private Vector3 _originalScale;
    private Vector3 _originalLocalPosition;
    private Coroutine _hitRoutine;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _originalLocalPosition = transform.localPosition;
    }

    public void PlayHit(Vector3 hitDirection)
    {
        if (_hitRoutine != null)
            StopCoroutine(_hitRoutine);

        _hitRoutine = StartCoroutine(HitRoutine(hitDirection));
    }

    private IEnumerator HitRoutine(Vector3 hitDirection)
    {
        Vector3 targetScale = Vector3.Scale(_originalScale, hitScale);
        Vector3 pushOffset = hitDirection.normalized * pushDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / scaleDuration;
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, t);
            transform.localPosition = Vector3.Lerp(_originalLocalPosition, _originalLocalPosition + pushOffset, t);
            yield return null;
        }
        
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / scaleDuration;
            transform.localScale = Vector3.Lerp(targetScale, _originalScale, t);
            transform.localPosition = Vector3.Lerp(_originalLocalPosition + pushOffset, _originalLocalPosition, t);
            yield return null;
        }
        
        transform.localScale =  _originalScale;
        transform.localPosition = _originalLocalPosition;
    }
}
