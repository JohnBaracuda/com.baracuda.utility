using Baracuda.Utilities.Helper;

namespace Baracuda.Utilities.Inspector
{
    public static class InspectorSearch
    {
        public static bool IsActive { get; private set; }
        public static string Filter { get; private set; }

        public static string ContextQuery { get; private set; }

        public static bool IsActiveByContextQuery()
        {
            return IsActive
                   && (ContextQuery.IsNotNullOrWhitespace() && ContextQuery.ContainsIgnoreCaseAndSpace(Filter)
                   || (FoldoutHandler.ActiveTitle.IsNotNullOrWhitespace() && FoldoutHandler.ActiveTitle.ContainsIgnoreCaseAndSpace(Filter)));
        }

        public static void SetContextQuery(string context)
        {
            ContextQuery = context;
        }

        public static void ResetContextQuery()
        {
            ContextQuery = null;
        }

        public static bool IsValid(string query)
        {
            return !IsActive || query.ContainsIgnoreCaseAndSpace(Filter);
        }

        public static void BeginSearchContext()
        {
            Filter = GUIHelper.SearchBar(Filter);
            IsActive = Filter.IsNotNullOrWhitespace();
            FoldoutHandler.ForceFoldout = IsActive;
        }

        public static string BeginSearchContext(string filter)
        {
            Filter = GUIHelper.SearchBar(filter);
            IsActive = Filter.IsNotNullOrWhitespace();
            FoldoutHandler.ForceFoldout = IsActive;
            return Filter;
        }

        public static void EndSearchContext(bool clearFilter = false)
        {
            IsActive = false;
            FoldoutHandler.ForceFoldout = false;
            if (clearFilter)
            {
                Filter = null;
            }
        }
    }
}