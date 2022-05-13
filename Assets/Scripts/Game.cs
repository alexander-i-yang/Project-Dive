using UnityEngine;

public class Game : MonoBehaviour {
    public static bool IsPaused;

    public static float DeltaTime;
    public static float FixedDeltaTime;
    public delegate void ResetNFOAction();
    public static event ResetNFOAction ResetNextFrameOffset;

    void Awake() {Application.targetFrameRate = 60;}

    void Update() {
        DeltaTime = IsPaused ? 0 : Time.deltaTime;
    }

    private void FixedUpdate() {
        if (ResetNextFrameOffset != null) ResetNextFrameOffset();
        FixedDeltaTime = IsPaused ? 0 : Time.fixedDeltaTime;
    }
}