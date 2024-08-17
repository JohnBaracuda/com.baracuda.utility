namespace Baracuda.Bedrock.PlayerLoop
{
    public struct BuildReportData
    {
#if UNITY_EDITOR
        public readonly UnityEditor.Build.Reporting.BuildReport Report;

        public BuildReportData(UnityEditor.Build.Reporting.BuildReport report)
        {
            Report = report;
        }
#endif
    }
}