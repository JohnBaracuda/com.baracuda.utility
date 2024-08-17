using Sirenix.OdinInspector;
using System.Diagnostics;

namespace Baracuda.Bedrock.Odin
{
    [Conditional("UNITY_EDITOR")]
    public class FoldoutAttribute : PropertyGroupAttribute
    {
        public float BackgroundAlpha { get; set; } = .2f;
        public float LineAlpha { get; set; } = .3f;

        public FoldoutAttribute(string foldout, float order) : base(foldout, order)
        {
        }

        public FoldoutAttribute(string foldout) : base(foldout)
        {
        }
    }
}