using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    public float maxLength;
    public float fallSpeed;
    public GameObject splashEffectObject;
    public LayerMask obstacleLayerMask;
    
    private BoxCollider2D _boxCollider2D;
    private ParticleSystem splashEffect;
    private SpriteRenderer _spriteRenderer;
    private Material _material;
    
    private float lastLength;
    private int _length;
    private int _offset;
    // private static readonly int Length = Shader.PropertyToID("_length");


    // Start is called before the first frame update
    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        splashEffect = splashEffectObject.GetComponentInChildren<ParticleSystem>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _material = new Material(_spriteRenderer.material);
        _material = _spriteRenderer.material;
        _length = Shader.PropertyToID("_length");
        _offset = Shader.PropertyToID("_offset");
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentLength(out float currentMaxLength);
        CalculateActualLength(currentMaxLength, out float currentLength);
        ResizeLine(currentLength);
        RescaleCollider(currentLength);
        CheckForSplashEffect(currentLength, currentMaxLength);
    }

    private void GetCurrentLength(out float currentMaxLength) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, maxLength, obstacleLayerMask);
        if (hit)
            currentMaxLength = Vector3.Distance(transform.position, hit.point);
        else
            currentMaxLength = maxLength;
    }

    private void CalculateActualLength(float currentMaxLength,out float currentLength) {
        currentLength = lastLength + Time.deltaTime * fallSpeed;
        currentLength = Mathf.Clamp(currentLength, 0, currentMaxLength);
        lastLength = currentLength;
    }

    private void ResizeLine(float currentLength)
    {
        var transform1 = _spriteRenderer.transform;
        Vector3 scale = transform1.localScale;
        transform1.localScale = new Vector3(scale.x, currentLength, scale.z);
        Vector3 pos = transform1.localPosition;
        transform1.localPosition = new Vector3(pos.x, -currentLength/2, pos.z);
        _material.SetFloat("_length", currentLength);
    }

    private void RescaleCollider(float currentLength) {
        _boxCollider2D.size = new Vector2(_boxCollider2D.size.x, currentLength);
        _boxCollider2D.offset = new Vector2(0, -currentLength / 2);
    }

    private void CheckForSplashEffect(float currentLength, float currentMaxLength) {
        if (currentLength >= currentMaxLength - 30) {
            if (!splashEffect.isPlaying) {
                splashEffect.Play();
            }
        } else {
            if (splashEffect.isPlaying) {
                splashEffect.Stop();
            }
        }
        splashEffectObject.transform.position = transform.position - Vector3.up * currentLength;
    }
}
