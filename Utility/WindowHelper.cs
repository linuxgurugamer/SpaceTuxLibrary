using System;
using System.Collections.Generic;
using System.Linq;


namespace SpaceTuxUtility
{
    /// <summary>
    /// Provides a method to return a randomized sequence of numbers for window ids
    /// </summary>
    public class WindowHelper
    {
        static System.Random random; 
        static int ransomSeed = 0;
        private static Dictionary<string, int> _windowDictionary;

        /// <summary>
        /// Given a windowKey, provides a sequential series of numbers for the window ids. 
        /// </summary>
        /// <param name="windowKey">Name of a mod would suffice to keep the different lists seperate</param>
        /// <returns>New window id</returns>
        public static int NextWindowId(string windowKey)
        {
            if (_windowDictionary == null)
            {
                int seed = (int)System.DateTime.Now.Ticks; 
                random = new System.Random(seed);
                ransomSeed = random.Next();
                _windowDictionary = new Dictionary<string, int>();
            }

            if (_windowDictionary.ContainsKey(windowKey))
            {
                return _windowDictionary[windowKey];
            }

            var newId = ransomSeed + _windowDictionary.Count() + 1;

            _windowDictionary.Add(windowKey, newId);

            return newId;
        }
    }
}
