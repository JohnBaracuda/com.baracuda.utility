using System;
using UnityEngine;

namespace Baracuda.Utilities
{
    public static class UnityExtensions
    {
        public static Color ToColor(this LogType logType)
        {
            return logType switch
            {
                LogType.Log => RichTextExtensions.SoftWhite,
                LogType.Error => Color.red,
                LogType.Exception => Color.red,
                LogType.Assert => Color.red,
                LogType.Warning => Color.yellow,
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
        }
    }
}