using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF5.HisaCat.NPCDetails
{
    internal static class BepInExLog
    {
        /// <summary>
        /// Tips: BepInEx 'Debug' LogLevel is ignored defaults. if you want to see, edit 'LogLevels' option on '[Logging.Console]' section.
        /// </summary>
        /// <param name="obj"></param>
        public static void LogDebug(object obj)
        {
            BepInExLoader.log.LogDebug($"[{BepInExLoader.GUID}] {obj.ToString()}");
        }
        public static void LogInfo(object obj)
        {
            BepInExLoader.log.LogInfo($"[{BepInExLoader.GUID}] {obj.ToString()}");
        }
        public static void LogMessage(object obj)
        {
            BepInExLoader.log.LogMessage($"[{BepInExLoader.GUID}] {obj.ToString()}");
        }
        public static void LogError(object obj)
        {
            BepInExLoader.log.LogError($"[{BepInExLoader.GUID}] {obj.ToString()}");
        }
        public static void LogWarning(object obj)
        {
            BepInExLoader.log.LogWarning($"[{BepInExLoader.GUID}] {obj.ToString()}");
        }
    }
}
