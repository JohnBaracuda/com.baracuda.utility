using System.Threading.Tasks;
using Baracuda.Utility.Utilities;

namespace Baracuda.Utility.Editor.Utilities
{
    public static partial class GUIUtility
    {
        private static object lastClicked;

        public static bool IsDoubleClick(object clicked)
        {
            if (clicked.IsNull())
            {
                return false;
            }

            var result = clicked == lastClicked;
            lastClicked = clicked;
            ResetDoubleClickCache();
            return result;
        }

        private static async void ResetDoubleClickCache()
        {
            await Task.Delay(160);
            lastClicked = null;
        }
    }
}