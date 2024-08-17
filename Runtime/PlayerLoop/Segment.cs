namespace Baracuda.Bedrock.PlayerLoop
{
    public enum Segment
    {
        None = 0,
        Custom = 1,
        Update = 2,
        LateUpdate = 3,
        FixedUpdate = 4,
        ApplicationFocus = 5,
        ApplicationPause = 6,
        ApplicationQuit = 7,
        InitializationCompleted = 8,
        BeforeFirstSceneLoad = 9,
        AfterFirstSceneLoad = 10,
        FirstUpdate = 12,

        EnteredEditMode = 101,
        ExitingEditMode = 102,
        EnteredPlayMode = 103,
        ExitingPlayMode = 104,
        EditorUpdate = 105,
        EditorLoad = 106,
        BuildPreprocessor = 107,
        BuildPostprocessor = 108,
        OnGUI = 109,

        PreUpdate = 200,
        PostUpdate = 201,
        PreLateUpdate = 202,
        PostLateUpdate = 203
    }
}