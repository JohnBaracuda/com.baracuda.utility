using System;

namespace Baracuda.Bedrock.Types
{
    [Serializable]
    public struct Margin
    {
        public float top;
        public float right;
        public float bottom;
        public float left;

        public Margin(float top, float right, float bottom, float left)
        {
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.left = left;
        }
    }
}