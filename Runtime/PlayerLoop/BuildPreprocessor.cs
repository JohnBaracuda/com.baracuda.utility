namespace Baracuda.Bedrock.PlayerLoop
{
#if UNITY_EDITOR
    internal class BuildPreprocessor : UnityEditor.Build.IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            Gameloop.RaiseBuildReportPreprocessor(new BuildReportData(report));
        }
    }
#endif
}