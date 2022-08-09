using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using RF5.HisaCat.NPCDetails.Utils;

namespace RF5.HisaCat.NPCDetails
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    internal class BepInExLoader : BepInEx.IL2CPP.BasePlugin
    {
        public const string
            MODNAME = "NPCDetails",
            AUTHOR = "HisaCat",
            GUID = "RF5." + AUTHOR + "." + MODNAME,
            VERSION = "1.2.0";

        public static BepInEx.Logging.ManualLogSource log;

        public BepInExLoader()
        {
            log = Log;
        }

        public override void Load()
        {
            try
            {
                //Register components
                ClassInjector.RegisterTypeInIl2Cpp<NPCDetailWindow.Attachment_LeftStatusPos>();
                ClassInjector.RegisterTypeInIl2Cpp<NPCDetailWindow.Attachment_RightStatusPos>();
            }
            catch (System.Exception e)
            {
                BepInExLog.LogError($"Harmony - FAILED to Register Il2Cpp Types! {e}");
            }

            try
            {
                //Patches
                Harmony.CreateAndPatchAll(typeof(RF5FontHelper.FontLoader));
                Harmony.CreateAndPatchAll(typeof(SVPatcher));
                Harmony.CreateAndPatchAll(typeof(UIPatcher));
                //Harmony.CreateAndPatchAll(typeof(CalcStatusPatcher)); //for test
            }
            catch (System.Exception e)
            {
                BepInExLog.LogError($"Harmony - FAILED to Apply Patch's! {e}");
            }
        }


        [HarmonyPatch]
        internal class SVPatcher
        {
            [HarmonyPatch(typeof(SV), nameof(SV.CreateUIRes))]
            [HarmonyPostfix]
            private static void CreateUIResPostfix(SV __instance)
            {
                //Localization.LocalizedString.CreateTemplate();
                if (BundleLoader.MainBundle == null)
                    BundleLoader.LoadBundle();
            }
        }

        //[HarmonyPatch]
        //[Obsolete("This is TEST code for tame always success")]
        //internal static class CalcStatusPatcher
        //{
        //    //For test, tame always success
        //    [HarmonyPatch(typeof(Calc.CalcStatus), nameof(Calc.CalcStatus.CalcTame))]
        //    [HarmonyPrefix]
        //    private static bool CalcTamePrefix(ref bool __result, MonsterControllerBase monster)
        //    {
        //        __result = true;
        //        return false; //return false to skip
        //    }
        //}

        [HarmonyPatch]
        internal class UIPatcher
        {
            [HarmonyPatch(typeof(CampMenuMain), nameof(CampMenuMain.Update))]
            [HarmonyPostfix]
            private static void UpdatePostfix(CampMenuMain __instance)
            {
                //FOR DEBUG
                //if (BepInEx.IL2CPP.UnityEngine.Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.F1) && UnityEngine.Event.current.type == UnityEngine.EventType.KeyDown)
                //{
                //    //TimeManager.Instance.ChangeTimeNextDay(12, 30);
                //    //ItemStorageManager.GetStorage(Define.StorageType.Rucksack).PushItemIn(ItemData.Instantiate(ItemID.Item_Tendon, 1));
                //    //ItemStorageManager.GetStorage(Define.StorageType.Rucksack).PushItemIn(ItemData.Instantiate(ItemID.Item_Yakionigiri, 1));
                //    //ItemStorageManager.GetStorage(Define.StorageType.Rucksack).PushItemIn(ItemData.Instantiate(ItemID.Item_Yakiimo, 1));
                //}
            }

            private static string[] LAGACY_FILES_PATH = {
                System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, "texts.ini"),
                System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, "Localized", "chs.json"),
                System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, "Localized", "cht.json"),
                System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, "Localized", "de.json"),
                System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, "Localized", "en.json"),
                System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, "Localized", "fr.json"),
                System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, "Localized", "ja.json"),
                System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID, "Localized", "ko.json"),
            };

            [HarmonyPatch(typeof(CampMenuMain), nameof(CampMenuMain.StartCamp))]
            [HarmonyPostfix]
            private static void StartCampPostfix(CampMenuMain __instance)
            {
                //It called when camp window is newly opened.
                //In normal case, first page equals with active page on window opened newly.
                var activePage = (CampPage)__instance.campPageSwitcher.nowPage;
                var firstPage = __instance.firstPage;
                //BepInExLog.Log($"StartCampPostfix. activePage:{activePage} firstPage: {firstPage}");
            }

            public delegate void OnCampPageChangedDelegate(CampPage page);
            public static OnCampPageChangedDelegate OnCampPageChanged = null;

            //[HarmonyPatch(typeof(CampPageSwitcher), nameof(CampPageSwitcher.OpenPage), new Type[] { typeof(int) })]
            [HarmonyPatch(typeof(CampPageSwitcher), nameof(CampPageSwitcher.OpenPage), new Type[] { typeof(int) })]
            [HarmonyPostfix]
            //private static void OpenPagePostfix_int(CampMenuMain __instance, int nextPage)
            private static void OpenPagePostfix_CampPage(CampMenuMain __instance, CampPage nextPage)
            {
                //It called when camp window page(tabs on tops) changed (and also window newly opened)
                //Also in this step, activePage always 'Max' (seems like dose not initialized with nextPage yet)
                //* OpenPage(int nextPage), OpenPage(CampPage nextPage) is called in same time.
                var activePage = (CampPage)__instance.campPageSwitcher.nowPage;
                //BepInExLog.Log($"OpenPagePostfix_CampPage. nextPage: {nextPage} activePage: {activePage}");

                OnCampPageChanged?.Invoke(nextPage);
            }

            [HarmonyPatch(typeof(GenerateFriendlistButton), nameof(GenerateFriendlistButton.GenerateFriendData))]
            [HarmonyPostfix]
            private static void GenerateFriendDataPostfix(GenerateFriendlistButton __instance)
            {
                //It called on friendly page opened, and friendly insided tab (Friends, Monsters) changed.
                //BepInExLog.Log($"GenerateFriendDataPostfix. friendType: {__instance.friendType}");
            }
            [HarmonyPatch(typeof(FriendPageStatusDisp), nameof(FriendPageStatusDisp.SetStatusNPC))]
            [HarmonyPostfix]
            private static void SetStatusNPCPostfix(FriendPageStatusDisp __instance, int pageId, GenerateFriendlistButton _generateFriendlistButton)
            {
                //It called on friend detail opened and switched to another firend.
                //and '__instance.actorId' is equals with '_generateFriendlistButton.GetActorID(pageId)'
                //and 'pageId' is index of friend list button (starts with 0)
                //BepInExLog.Log($"SetStatusNPCPostfix. pageId: {pageId}, actorId: {__instance.actorId}");

                NPCDetailWindow.NPCDetailWindowManager.TryAttachIfNotExist(__instance);

                NPCDetailWindow.NPCDetailWindowManager.TrySetShown(true);
                var npcData = NpcDataManager.Instance.GetNpcData(__instance.actorId);
                NPCDetailWindow.NPCDetailWindowManager.TrySetNPCData(npcData);

                #region DEBUG
                //BepInExLog.Log($" - CurrentPlace: {npcData.CurrentPlace}"); //Print current npc's place.
                //BepInExLog.Log($" - TargetPlace: {npcData.TargetPlace}"); //Display npc's destination if it exist.
                //BepInExLog.Log($" - Loves: {string.Join(", ", npcData.GetVeryFavoriteItemDataTables().Select(x => $"{x.GetItemName()}({(int)ItemDataTable.GetItemID(x.ItemIndex)})"))}");
                //BepInExLog.Log($" - Likes: {string.Join(", ", npcData.GetFavoriteItemDataTables().Select(x => $"{x.GetItemName()}({(int)ItemDataTable.GetItemID(x.ItemIndex)})"))}");
                //BepInExLog.Log($" - Dislikes: {string.Join(", ", npcData.GetNotFavoriteItemDataTables().Select(x => $"{x.GetItemName()}({(int)ItemDataTable.GetItemID(x.ItemIndex)})"))}");
                //BepInExLog.Log($" - Hates: {string.Join(", ", npcData.GetNotFavoriteBadlyItemDataTables().Select(x => $"{x.GetItemName()}({(int)ItemDataTable.GetItemID(x.ItemIndex)})"))}");
                //BepInExLog.Log("");

                //Print all npc's detials
                //foreach (Define.ActorID actorId in typeof(Define.ActorID).GetEnumValues())
                //{
                //    var npcData = NpcDataManager.Instance.GetNpcData(actorId);
                //    if (npcData == null) continue;
                //    BepInExLog.Log($"{npcData.GetNpcName()} ({actorId.ToString()})");
                //    BepInExLog.Log($"{npcData.GetNpcDiscript()}\r\n");
                //    BepInExLog.Log($" - Loves: {string.Join(", ", npcData.GetVeryFavoriteItemDataTables().OrderBy(x => x.ItemIndex).Select(x => $"{x.GetItemName()}({(int)ItemDataTable.GetItemID(x.ItemIndex)})"))}");
                //    BepInExLog.Log($" - Likes: {string.Join(", ", npcData.GetFavoriteItemDataTables().OrderBy(x => x.ItemIndex).Select(x => $"{x.GetItemName()}({(int)ItemDataTable.GetItemID(x.ItemIndex)})"))}");
                //    BepInExLog.Log($" - Dislikes: {string.Join(", ", npcData.GetNotFavoriteItemDataTables().OrderBy(x => x.ItemIndex).Select(x => $"{x.GetItemName()}({(int)ItemDataTable.GetItemID(x.ItemIndex)})"))}");
                //    BepInExLog.Log($" - Hates: {string.Join(", ", npcData.GetNotFavoriteBadlyItemDataTables().OrderBy(x => x.ItemIndex).Select(x => $"{x.GetItemName()}({(int)ItemDataTable.GetItemID(x.ItemIndex)})"))}");
                //    BepInExLog.Log("");
                //}
                #endregion DEBUG
            }
            [HarmonyPatch(typeof(FriendPageStatusDisp), nameof(FriendPageStatusDisp.SetStatusMonster))]
            [HarmonyPostfix]
            private static void SetStatusMonsterPostfix(FriendPageStatusDisp __instance, int pageId, GenerateFriendlistButton _generateFriendlistButton)
            {
                //Same with 'SetStatusNPCPostfix' but in moster case
                //But in this case, trust 'monsterDataID' instead 'actorId'
                //BepInExLog.Log($"SetStatusMonsterPostfix. pageId: {pageId}, monsterDataID: {__instance.monsterDataID}");
                NPCDetailWindow.NPCDetailWindowManager.TryAttachIfNotExist(__instance);

                //NPCDetailWindow.NPCDetailWindowManager.TrySetShown(false);
                NPCDetailWindow.NPCDetailWindowManager.TrySetShown(true);
                var monsterData = MonsterDataTable.GetDataTable(__instance.monsterDataID);

                //_generateFriendlistButton.MonsterStatusDataIds ?
                //__instance.monsterDataID ?
                NPCDetailWindow.NPCDetailWindowManager.TrySetMonsterData(pageId, monsterData);
            }
        }
    }
}
