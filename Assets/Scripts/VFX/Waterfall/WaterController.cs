using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Serialization;
using VFX;

public class WaterController : MonoBehaviour
{
    public float maxLength;
    public float fallSpeed;
    public GameObject splashEffectObject;
    public LayerMask obstacleLayerMask;
    
    private BoxCollider2D _boxCollider2D;
    private ParticleSystem splashEffect;
    private SpriteRenderer _spriteRenderer;
    // private Material _material;
    
    [FormerlySerializedAs("lastLength")] [SerializeField] private float _lastLength;
    [SerializeField] private float _bakedLength;
    [SerializeField] private Material origMaterial;

    public GameObject WaterCutPrefab;
    
    // private static readonly int Length = Shader.PropertyToID("_length");


    // Start is called before the first frame update
    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        splashEffect = splashEffectObject.GetComponentInChildren<ParticleSystem>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.material = new Material(origMaterial);
        // _material = _spriteRenderer.material;
        Resize(_bakedLength, _bakedLength);
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentLength(out float currentMaxLength);
        CalculateActualLength(currentMaxLength, out float currentLength);
        Resize(currentLength, currentMaxLength);
        _lastLength = currentLength;
    }

    void Resize(float currentLength, float currentMaxLength)
    {
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
        currentLength = _lastLength + Game.Instance.DeltaTime * fallSpeed;
        currentLength = Mathf.Clamp(currentLength, 0, currentMaxLength);
    }

    private void ResizeLine(float currentLength)
    {
        /*if (_lastLength > currentLength)
        {
            WaterCut w = Instantiate(WaterCutPrefab, transform.position, Quaternion.identity, transform.parent).GetComponent<WaterCut>();
            float cutLen = _lastLength - currentLength;
            w.Init(transform.localPosition.y-_lastLength, cutLen+6, fallSpeed);
        }*/
        var transform1 = _spriteRenderer.transform;
        Vector3 scale = transform1.localScale;
        currentLength += 6;
        transform1.localScale = new Vector3(scale.x, currentLength, scale.z);
        Vector3 pos = transform1.localPosition;
        transform1.localPosition = new Vector3(pos.x, -currentLength/2 + 1, pos.z);
        // if (_material != null) _material.SetFloat("_length", currentLength);
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

    public void Bake()
    {
        GetCurrentLength(out float l);
        _bakedLength = l;
        _lastLength = l;
        
        _boxCollider2D = GetComponent<BoxCollider2D>();
        splashEffect = splashEffectObject.GetComponentInChildren<ParticleSystem>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.material = new Material(origMaterial);
        Resize(l, l);
    }
}
