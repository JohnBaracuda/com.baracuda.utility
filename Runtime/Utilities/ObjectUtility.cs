using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Baracuda.Utility.Utilities
{
    public class ObjectUtility : MonoBehaviour
    {
        public static T CloneObjectByFields<T>(T source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is string text)
            {
                return (T)(object)new string(text);
            }

            var clone = Activator.CreateInstance<T>();
            var type = source.GetType();

            // Copy each field from the source to the clone
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var value = field.GetValue(source);
                field.SetValue(clone, value);
            }

            return clone;
        }

        public static T DeepCopy<T>(T source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}