#if false
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KSP_Log;

namespace List_Transforms
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class ListTransforms : MonoBehaviour
    {
        const string FOLDER = "GameData/transforms";
        void Start()
        {
            foreach (AvailablePart loadedPart in PartLoader.LoadedPartsList)
            {
                
                var t = FindModelTransformEx(loadedPart.partPrefab, loadedPart.partUrl);
            }
        }
        private static Transform FindModelTransformEx(Part part, string transformPath)
        {
            if (string.IsNullOrEmpty(transformPath))
                throw new ArgumentException("must specify a url", "transformPath");

            var pathParts = transformPath.Trim('/').Split('/');
            var modelTransform = part.transform.Find("model");

            if (pathParts.Length == 1)
            {
                if (modelTransform.childCount == 1)
                    return modelTransform.GetChild(0); // no way to tell for certain this transform name matches without parsing the mu again

                throw new ArgumentException(
                    "The part has multiple models and there isn't enough information to figure out which one " +
                    transformPath + " specifies");
            }

            var targetPath = string.Join("/", pathParts.Skip(1).ToArray());

            if (!part.partInfo.partConfig.HasNode("MODEL"))
                return pathParts.Length == 1
                    ? modelTransform
                    : modelTransform.Find(targetPath);

            foreach (Transform model in modelTransform)
            {
                var found = model.transform.Find(targetPath);

                if (found) return found;
            }

            return null;
        }

    }

#if true
    public static class GameObjectExtensions
    {
        public delegate void VisitorDelegate(GameObject go, int depth);


        public static void TraverseHierarchy(this GameObject go, VisitorDelegate visitor)
        {
            if (go == null) throw new ArgumentNullException("go");
            if (visitor == null) throw new ArgumentNullException("visitor");
            TraverseHierarchy(go, visitor, 0);
        }

        private static void TraverseHierarchy(GameObject go, VisitorDelegate visitor, int depth)
        {
            visitor(go, depth);

            foreach (Transform t in go.transform)
                TraverseHierarchy(t.gameObject, visitor, depth + 1);
        }


        public static void PrintComponents(this GameObject go, Log baseLog)
        {
            if (go == null) throw new ArgumentNullException("go");
            if (baseLog == null) throw new ArgumentNullException("baseLog");

            go.TraverseHierarchy((gameObject, depth) =>
            {
                baseLog.Info(depth > 0 ? new string('-', depth) + ">" : "" + gameObject.name + "{0}{1} has components:");

                var components = gameObject.GetComponents<Component>();
                foreach (var c in components)
                {

                    baseLog.Info(
                         new string('.', depth + 3) + "c" + " " + c == null ? "[missing script]" : c.GetType().FullName);
                }
            });
        }
    }
#endif
    
}
#endif
