using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RF5.HisaCat.NPCDetails.Utils
{
    internal static class UnityExtension
    {
    }

    internal static class GameObjectExtension
    {
        internal static bool TryFindComponent<T>(this Component self, string name, out T component, bool showErrLog = true) where T : Component
        {
            component = self.FindComponent<T>(name);
            if (showErrLog && component == null)
                BepInExLog.LogError($"Cannot find \"{typeof(T).FullName}\" component at \"{name}\" from \"{self.name}\"");
            return component != null;
        }
        internal static bool TryFindComponent<T>(this GameObject self, string name, out T component, bool showErrLog = true) where T : Component
        {
            component = self.FindComponent<T>(name);
            if (showErrLog && component == null)
                BepInExLog.LogError($"Cannot find \"{typeof(T).FullName}\" component at \"{name}\" from \"{self.name}\"");
            return component != null;
        }

        internal static bool TryFindGameObject(this Component self, string name, out GameObject gameObject, bool showErrLog = true)
        {
            gameObject = self.FindGameObject(name);
            if (showErrLog && gameObject == null)
                BepInExLog.LogError($"Cannot find GameObject at \"{name}\" from \"{self.name}\"");
            return gameObject != null;
        }
        internal static bool TryFindGameObject(this GameObject self, string name, out GameObject gameObject, bool showErrLog = true)
        {
            gameObject = self.FindGameObject(name);
            if (showErrLog && gameObject == null)
                BepInExLog.LogError($"Cannot find GameObject at \"{name}\" from \"{self.name}\"");
            return gameObject != null;
        }
    }
}
