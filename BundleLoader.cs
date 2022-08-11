using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace RF5.HisaCat.NPCDetails
{
    internal static class BundleLoader
    {
        public static AssetBundle MainBundle { get; private set; }

        public static bool LoadBundle()
        {
            if (MainBundle != null)
            {
                BepInExLog.LogDebug("[BundleLoader] Bundle already loaded");
                return true;
            }
            var bundleDir = System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID);
            var mainBundlePath = System.IO.Path.Combine(bundleDir, "npcdetails.main.unity3d");
            if (System.IO.File.Exists(mainBundlePath) == false)
            {
                BepInExLog.LogError($"[BundleLoader] Bundle missing. bundle must be placed at \"{mainBundlePath}\"");
                return false;
            }

            MainBundle = AssetBundle.LoadFromFile(mainBundlePath);

            if (MainBundle == null)
            {
                BepInExLog.LogError($"[BundleLoader] Cannot load bundle at \"{mainBundlePath}\"");
                return false;
            }

            BepInExLog.LogInfo("[BundleLoader] Bundle loaded.");
            return true;
        }

        public static T LoadIL2CPP<T>(this AssetBundle bundle, string name) where T : UnityEngine.Object
        {
            var asset = bundle.LoadAsset_Internal(name, Il2CppType.Of<T>());
            return asset == null ? null : asset.TryCast<T>();
        }
    }
}
