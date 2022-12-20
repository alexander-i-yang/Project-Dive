using UnityEngine;
using MyBox;

public class Game : Singleton<Game> {
    [Range(0, 1)] public float TimeScale = 1;
    public bool IsPaused;

    public float DeltaTime;
    public float FixedDeltaTime;
    public delegate void ResetNFOAction();
    public event ResetNFOAction ResetNextFrameOffset;

    void Awake() {
        Application.targetFrameRate = 60;
        InitializeSingleton();
    }

    void Update() {
        DeltaTime = IsPaused ? 0 : Time.deltaTime * TimeScale;
    }

    private void FixedUpdate() {
        if (ResetNextFrameOffset != null) ResetNextFrameOffset();
        FixedDeltaTime = IsPaused ? 0 : Time.fixedDeltaTime * TimeScale;
    }
}