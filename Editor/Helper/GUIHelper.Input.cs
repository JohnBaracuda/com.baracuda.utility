using System.Threading.Tasks;

namespace Baracuda.Utilities.Editor.Helper
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