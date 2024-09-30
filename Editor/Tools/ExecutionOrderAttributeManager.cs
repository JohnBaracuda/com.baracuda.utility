using System;
using System.Collections.Generic;
using Baracuda.Utility.PlayerLoop;
using UnityEngine;

namespace Baracuda.Utility.Editor.Tools
{
    /// <summary>
    ///     Dynamically manages script execution order for Mono scripts based on these attributes:<br />
    ///     <see cref="ExecuteAfterAttribute" />, <see cref="ExecuteAfterAttribute" /> or
    ///     <see cref="ExecutionOrderAttribute" />
    /// </summary>
    public static class ExecutionOrderAttributeManager
    {
        #region Nested Types

        private static class Graph
        {
            public struct Edge
            {
                public UnityEditor.MonoScript Node;
                public int Weight;
            }

            public static Dictionary<UnityEditor.MonoScript, List<Edge>> Create(List<OrderDefinition> definitions,
                List<OrderDependency> dependencies)
            {
                var graph = new Dictionary<UnityEditor.MonoScript, List<Edge>>();

                for (var i = 0; i < dependencies.Count; i++)
                {
                    var dependency = dependencies[i];
                    var source = dependency.FirstScript;
                    var dest = dependency.SecondScript;

                    if (!graph.TryGetValue(source, out var edges))
                    {
                        edges = new List<Edge>();
                        graph.Add(source, edges);
                    }

                    edges.Add(new Edge
                    {
                        Node = dest,
                        Weight = dependency.OrderDelta
                    });
                    if (!graph.ContainsKey(dest))
                    {
                        graph.Add(dest, new List<Edge>());
                    }
                }

                for (var i = 0; i < definitions.Count; i++)
                {
                    var definition = definitions[i];
                    var node = definition.Script;
                    if (!graph.ContainsKey(node))
                    {
                        graph.Add(node, new List<Edge>());
                    }
                }

                return graph;
            }

            private static bool IsCyclicRecursion(
                Dictionary<UnityEditor.MonoScript, List<Edge>> graph,
                UnityEditor.MonoScript node,
                Dictionary<UnityEditor.MonoScript, bool> visited,
                Dictionary<UnityEditor.MonoScript, bool> inPath)
            {
                if (visited[node])
                {
                    return inPath[node];
                }

                visited[node] = true;
                inPath[node] = true;

                for (var i = 0; i < graph[node].Count; i++)
                {
                    var edge = graph[node][i];
                    var succeeding = edge.Node;
                    if (!IsCyclicRecursion(graph, succeeding, visited, inPath))
                    {
                        continue;
                    }

                    inPath[node] = false;
                    return true;
                }

                inPath[node] = false;
                return false;
            }

            public static bool IsCyclic(Dictionary<UnityEditor.MonoScript, List<Edge>> graph)
            {
                var visited = new Dictionary<UnityEditor.MonoScript, bool>();
                var inPath = new Dictionary<UnityEditor.MonoScript, bool>();

                var keys = graph.Keys;

                foreach (var node in keys)
                {
                    visited.Add(node, false);
                    inPath.Add(node, false);
                }

                foreach (var node in keys)
                {
                    if (IsCyclicRecursion(graph, node, visited, inPath))
                    {
                        return true;
                    }
                }

                return false;
            }

            public static List<UnityEditor.MonoScript> GetRoots(Dictionary<UnityEditor.MonoScript, List<Edge>> graph)
            {
                var degrees = new Dictionary<UnityEditor.MonoScript, int>();

                foreach (var node in graph.Keys)
                {
                    degrees.Add(node, 0);
                }

                foreach (var (_, edges) in graph)
                {
                    foreach (var edge in edges)
                    {
                        var succeeding = edge.Node;
                        degrees[succeeding]++;
                    }
                }

                var roots = new List<UnityEditor.MonoScript>();
                foreach (var (node, degree) in degrees)
                {
                    if (degree == 0)
                    {
                        roots.Add(node);
                    }
                }

                return roots;
            }

            public static void PropagateValues(Dictionary<UnityEditor.MonoScript, List<Edge>> graph,
                Dictionary<UnityEditor.MonoScript, int> values)
            {
                var queue = new Queue<UnityEditor.MonoScript>();

                foreach (var node in values.Keys)
                {
                    queue.Enqueue(node);
                }

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();
                    var currentValue = values[node];

                    foreach (var edge in graph[node])
                    {
                        var succeeding = edge.Node;
                        var newValue = currentValue + edge.Weight;
                        var hasPrevValue = values.TryGetValue(succeeding, out var previousValue);
                        var newValueBeyond =
                            edge.Weight > 0 ? newValue > previousValue : newValue < previousValue;
                        if (hasPrevValue && !newValueBeyond)
                        {
                            continue;
                        }

                        values[succeeding] = newValue;
                        queue.Enqueue(succeeding);
                    }
                }
            }
        }

        private struct OrderDefinition
        {
            public UnityEditor.MonoScript Script { get; set; }
            public int Order { get; set; }
        }

        private struct OrderDependency
        {
            public UnityEditor.MonoScript FirstScript { get; set; }
            public UnityEditor.MonoScript SecondScript { get; set; }
            public int OrderDelta { get; set; }
        }

        #endregion


        #region Setup

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            var types = GetTypeDictionary();
            var definitions = GetExecutionOrderDefinitions(types);
            var dependencies = GetExecutionOrderDependencies(types);
            var graph = Graph.Create(definitions, dependencies);

            if (Graph.IsCyclic(graph))
            {
                Debug.LogError("Circular script execution order definitions");
                return;
            }

            var roots = Graph.GetRoots(graph);
            var orders = GetInitialExecutionOrder(definitions, roots);
            Graph.PropagateValues(graph, orders);

            UpdateExecutionOrder(orders);
        }

        #endregion


        #region Logic

        private static List<OrderDependency> GetExecutionOrderDependencies(
            Dictionary<Type, UnityEditor.MonoScript> types)
        {
            var list = new List<OrderDependency>();

            foreach (var (type, script) in types)
            {
                var hasExecutionOrderAttribute = Attribute.IsDefined(type, typeof(ExecutionOrderAttribute), true);
                var hasExecuteAfterAttribute = Attribute.IsDefined(type, typeof(ExecuteAfterAttribute), true);
                var hasExecuteBeforeAttribute = Attribute.IsDefined(type, typeof(ExecuteBeforeAttribute), true);

                if (hasExecuteAfterAttribute &&
                    GetExecuteAfterDependencies(types, hasExecutionOrderAttribute, script, type, list))
                {
                    continue;
                }

                if (hasExecuteBeforeAttribute)
                {
                    GetExecuteBeforeDependencies(types, hasExecutionOrderAttribute, script, hasExecuteAfterAttribute,
                        type, list);
                }
            }

            return list;
        }

        private static void GetExecuteBeforeDependencies(Dictionary<Type, UnityEditor.MonoScript> types,
            bool hasExecutionOrderAttribute, UnityEditor.MonoScript script,
            bool hasExecuteAfterAttribute, Type type, List<OrderDependency> list)
        {
            if (hasExecutionOrderAttribute)
            {
                Debug.LogError(
                    $"Script {script.name} has both [ExecutionOrder] and [ExecuteBefore] attributes. Ignoring the [ExecuteBefore] attribute.",
                    script);
                return;
            }

            if (hasExecuteAfterAttribute)
            {
                Debug.LogError(
                    $"Script {script.name} has both [ExecuteAfter] and [ExecuteBefore] attributes. Ignoring the [ExecuteBefore] attribute.",
                    script);
                return;
            }

            var attributes =
                (ExecuteBeforeAttribute[])Attribute.GetCustomAttributes(type, typeof(ExecuteBeforeAttribute), true);
            for (var i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                if (attribute.OrderDecrease < 0)
                {
                    Debug.LogError(
                        $"Script {script.name} has an [ExecuteBefore] attribute with a negative orderDecrease. Use the [ExecuteAfter] attribute instead. Ignoring this [ExecuteBefore] attribute.",
                        script);
                    continue;
                }

                if (!attribute.TargetType.IsSubclassOf(typeof(MonoBehaviour)) &&
                    !attribute.TargetType.IsSubclassOf(typeof(ScriptableObject)))
                {
                    Debug.LogError(
                        $"Script {script.name} has an [ExecuteBefore] attribute with targetScript={attribute.TargetType.Name} which is not a MonoBehaviour nor a ScriptableObject. Ignoring this [ExecuteBefore] attribute.",
                        script);
                    continue;
                }

                var targetScript = types[attribute.TargetType];
                var dependency = new OrderDependency
                {
                    FirstScript = targetScript,
                    SecondScript = script,
                    OrderDelta = -attribute.OrderDecrease
                };
                list.Add(dependency);
            }
        }

        private static bool GetExecuteAfterDependencies(Dictionary<Type, UnityEditor.MonoScript> types,
            bool hasExecutionOrderAttribute, UnityEditor.MonoScript script, Type type, List<OrderDependency> list)
        {
            if (hasExecutionOrderAttribute)
            {
                Debug.LogError(
                    $"Script {script.name} has both [ExecutionOrder] and [ExecuteAfter] attributes. Ignoring the [ExecuteAfter] attribute.",
                    script);
                return true;
            }

            var attributes =
                (ExecuteAfterAttribute[])Attribute.GetCustomAttributes(type, typeof(ExecuteAfterAttribute), true);
            foreach (var attribute in attributes)
            {
                if (!attribute.TargetType.IsSubclassOf(typeof(MonoBehaviour)) &&
                    !attribute.TargetType.IsSubclassOf(typeof(ScriptableObject)))
                {
                    Debug.LogError(
                        $"Script {script.name} has an [ExecuteAfter] attribute with targetScript={attribute.TargetType.Name} which is not a MonoBehaviour nor a ScriptableObject. Ignoring this [ExecuteAfter] attribute.",
                        script);
                    continue;
                }

                var targetScript = types[attribute.TargetType];
                var dependency = new OrderDependency
                {
                    FirstScript = targetScript,
                    SecondScript = script,
                    OrderDelta = (int)attribute.OrderIncrease
                };
                list.Add(dependency);
            }

            return false;
        }

        private static List<OrderDefinition> GetExecutionOrderDefinitions(
            Dictionary<Type, UnityEditor.MonoScript> types)
        {
            var list = new List<OrderDefinition>();

            foreach (var (type, script) in types)
            {
                if (!Attribute.IsDefined(type, typeof(ExecutionOrderAttribute), true))
                {
                    continue;
                }

                var attribute =
                    (ExecutionOrderAttribute)Attribute.GetCustomAttribute(type, typeof(ExecutionOrderAttribute), true);
                var definition = new OrderDefinition
                {
                    Script = script,
                    Order = attribute.Order
                };
                list.Add(definition);
            }

            return list;
        }

        private static Dictionary<UnityEditor.MonoScript, int> GetInitialExecutionOrder(
            List<OrderDefinition> definitions,
            List<UnityEditor.MonoScript> graphRoots)
        {
            var orders = new Dictionary<UnityEditor.MonoScript, int>();
            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                var script = definition.Script;
                var order = definition.Order;
                orders.Add(script, order);
            }

            for (var i = 0; i < graphRoots.Count; i++)
            {
                var script = graphRoots[i];
                if (orders.ContainsKey(script))
                {
                    continue;
                }

                var order = UnityEditor.MonoImporter.GetExecutionOrder(script);
                orders.Add(script, order);
            }

            return orders;
        }

        private static void UpdateExecutionOrder(Dictionary<UnityEditor.MonoScript, int> orders)
        {
            var startedEdit = false;

            foreach (var (script, order) in orders)
            {
                if (UnityEditor.MonoImporter.GetExecutionOrder(script) == order)
                {
                    continue;
                }

                if (!startedEdit)
                {
                    UnityEditor.AssetDatabase.StartAssetEditing();
                    startedEdit = true;
                }

                UnityEditor.MonoImporter.SetExecutionOrder(script, order);
            }

            if (startedEdit)
            {
                UnityEditor.AssetDatabase.StopAssetEditing();
            }
        }

        #endregion


        #region Helper

        private static Dictionary<Type, UnityEditor.MonoScript> GetTypeDictionary()
        {
            var types = new Dictionary<Type, UnityEditor.MonoScript>();

            var scripts = UnityEditor.MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var script in scripts)
            {
                var type = script.GetClass();

                if (!IsTypeValid(type))
                {
                    continue;
                }

                types.TryAdd(type, script);
            }

            return types;
        }

        private static bool IsTypeValid(Type type)
        {
            if (type != null)
            {
                return type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject));
            }

            return false;
        }

        #endregion
    }
}