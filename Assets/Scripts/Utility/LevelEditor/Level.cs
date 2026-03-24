using System.Collections.Generic;
using System.Linq;
using Data.Scheme.Public;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utility.LevelEditor
{
    public class Level : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private readonly Dictionary<LevelElementType, List<LevelElement>> collection = new();
        [ShowInInspector, ReadOnly] private readonly Dictionary<LevelElementType, Transform> typeRoots = new();
        
        public IReadOnlyList<LevelElement> GetElements()
        {
            return collection.Values
                .SelectMany(elements => elements)
                .Where(element => element != null)
                .ToList();
        }
        
        public IReadOnlyList<LevelElement> GetElements(LevelElementType type)
        {
            if (collection.TryGetValue(type, out List<LevelElement> elements))
                return elements;

            return System.Array.Empty<LevelElement>();
        }
        
        public void AddElement(IEnumerable<LevelElement> elements)
        {
            if (elements == null)
                return;

            foreach (LevelElement element in elements)
                AddElement(element);
        }

        public void RemoveElement(IEnumerable<LevelElement> elements)
        {
            if (elements == null)
                return;

            foreach (LevelElement element in elements)
                RemoveElement(element);
        }

        public void AddElement(LevelElement element)
        {
            if (element == null)
                return;

            LevelElementType type = element.LevelElementType;

            if (!collection.ContainsKey(type))
                collection.Add(type, new List<LevelElement>());

            if (!collection[type].Contains(element))
                collection[type].Add(element);

            Transform root = GetOrCreateTypeRoot(type);
            element.transform.SetParent(root, true);
        }

        public void RemoveElement(LevelElement element)
        {
            if (element == null)
                return;

            LevelElementType type = element.LevelElementType;

            if (!collection.ContainsKey(type))
                return;

            collection[type].Remove(element);

            if (collection[type].Count == 0)
            {
                collection.Remove(type);

                if (typeRoots.TryGetValue(type, out Transform root))
                {
                    typeRoots.Remove(type);

#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        DestroyImmediate(root.gameObject);
                    else
#endif
                        Destroy(root.gameObject);
                }
            }
        }
        
        public void Clear()
        {
            foreach (List<LevelElement> elements in collection.Values)
            {
                foreach (LevelElement element in elements)
                {
                    if (element == null)
                        continue;

#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        DestroyImmediate(element.gameObject);
                    else
#endif
                        Destroy(element.gameObject);
                }
            }

            foreach ((_, Transform root) in typeRoots)
            {
                if (root == null)
                    continue;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(root.gameObject);
                else
#endif
                    Destroy(root.gameObject);
            }

            collection.Clear();
            typeRoots.Clear();
        }

        private Transform GetOrCreateTypeRoot(LevelElementType type)
        {
            if (typeRoots.TryGetValue(type, out Transform root) && root != null)
                return root;

            GameObject rootObject = new(type.ToString());
            root = rootObject.transform;
            root.SetParent(transform, false);

            typeRoots[type] = root;
            return root;
        }
    }
}