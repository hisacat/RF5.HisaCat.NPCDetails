using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RF5.HisaCat.NPCDetails.Utils
{
    internal static class RF5FontHelper
    {
        public static Font MainFont { get; private set; }

        [HarmonyPatch]
        public class FontLoader
        {
            private static bool Initialized = false;

            [HarmonyPatch(typeof(SV), nameof(SV.CreateUIRes))]
            [HarmonyPostfix]
            public static void CreateUIResPostfix(SV __instance)
            {
                if (Initialized) return;
                Initialized = true;

                //Check font asset from: Assets/AddressableAssets/{LANG}/Prefab/UIObject/Loader/UIMovieRoom.prefab
                //Chs: DFGB_H5
                //Cht: DFT_B5
                //Eng: ATILN___
                //Fre: ATILN___
                //Ger: ATILN___
                //Jpn: DF-ChuButoMaruGothic-W7
                //Kor: KoreanSMI-L
                //Kor(Assets/AddressableAssets/Kor/Prefab/LanguageOnly/UI/Craft/UI_Solowork_Success.prefab): KoreanPOR-L

                var enumId = Loader.ID.Prefab.UIOBJECT_LOADER_UIMOVIEROOM;
                var id = (int)enumId;

                //[NOTE] Remove from HandleList if id already exist for get other languages with same id in same frame
                if (Loader.AssetHandle.HandleList.ContainsKey(id))
                    Loader.AssetHandle.HandleList.Remove(id);

                Loader.AssetManager.Entry<UnityEngine.GameObject>(id, new System.Action<Loader.AssetHandle<UnityEngine.GameObject>>((handler) =>
                {
                    if (handler != null && handler.IsDone && handler.Result != null)
                    {
                        //[NOTE] Remove from HandleList for get other languages with same id later and avoid give effects main game logic.
                        Loader.AssetHandle.HandleList.Remove(id);
                        var sText = handler.Result.GetComponentInChildren<SText>();
                        if (sText == null)
                        {
                            BepInExLog.LogError($"FontLoader: Cannot find SText");
                        }
                        else
                        {
                            MainFont = sText.font;
                        }
                    }
                    else
                    {
                        BepInExLog.LogError($"FontLoader: AssetManager.Entry {enumId} failed");
                    }
                }), -1, false);
            }
        }
        public static void SetFontGlobal(GameObject root)
        {
            if (MainFont == null)
            {
                BepInExLog.LogError("SetFontGlobal: font was not ready");
                return;
            }
            var texts = root.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
                text.font = MainFont;
        }
    }
}
