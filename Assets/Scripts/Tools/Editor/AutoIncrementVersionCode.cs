#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HGS.Tools.Editor
{
    public class AutoIncrementVersionCode : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android)
            {
                int currentVersionCode = PlayerSettings.Android.bundleVersionCode;
                int newVersionCode = currentVersionCode + 1;
                PlayerSettings.Android.bundleVersionCode = newVersionCode;

                Debug.Log($"[AutoIncrement] Android Bundle Version Code increased to: {newVersionCode}");
            }
        }
    }
}
#endif