namespace Baracuda.Utility.PlayerLoop
{
    public enum Segment
    {
        None = 0,
        Update = 2,
        LateUpdate = 3,
        FixedUpdate = 4,
        ApplicationFocus = 5,
        ApplicationPause = 6,
        ApplicationQuit = 7,
        InitializationCompleted = 8,
        BeforeFirstSceneLoad = 9,
        AfterFirstSceneLoad = 10,
        EditorUpdate = 105,
        OnGUI = 109
    }
}