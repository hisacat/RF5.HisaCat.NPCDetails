using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using RF5.HisaCat.NPCDetails.Utils;
using BepInEx;

namespace RF5.HisaCat.NPCDetails
{
    internal class NPCDetailWindow : MonoBehaviour
    {
        private Transform window = null;
        private ScrollRect scrollRect = null;
        private Text detailText = null;

        private static UIOnOffAnimate equipMenuItemDetail = null;
        public static NPCDetailWindow Instance { get; private set; }
        public static bool InstantiateAndAttach(FriendPageStatusDisp friendPageStatusDisp)
        {
            if (Instance != null)
            {
                BepInExLog.Log("[NPCDetailWindow] InstantiateAndAttach: instance already exist");
                return true;
            }

            var attachTarget = friendPageStatusDisp.transform.Find("StatusObj/FriendsStatus/RightStatusPos");
            if (attachTarget == null)
            {
                BepInExLog.LogError("[NPCDetailWindow] InstantiateAndAttach: Cannot find attachTarget");
                return false;
            }

            equipMenuItemDetail = friendPageStatusDisp.FindComponent<UIOnOffAnimate>("StatusObj/FriendsStatus/EquipMenuItemDetail/OnOffWindows");
            if (equipMenuItemDetail == null)
            {
                BepInExLog.LogError("[NPCDetailWindow] InstantiateAndAttach: Cannot find equipMenuItemDetailWindow");
                return false;
            }

            var windowPrefab = BundleLoader.MainBundle.LoadIL2CPP<GameObject>("[RF5.HisaCat.NPCDetails]RightStatusPos");
            if (windowPrefab == null)
            {
                BepInExLog.LogError("[NPCDetailWindow] InstantiateAndAttach: Cannot load window prefab");
                return false;
            }

            var windowInstanceGO = GameObject.Instantiate(windowPrefab, attachTarget.transform);
            if (windowInstanceGO == null)
            {
                BepInExLog.LogError("[NPCDetailWindow] InstantiateAndAttach: Cannot instantiate window");
                return false;
            }

            RF5FontHelper.SetFontGlobal(windowInstanceGO);

            NPCDetailWindow.Instance = windowInstanceGO.AddComponent<NPCDetailWindow>();
            {
                NPCDetailWindow.Instance.window = windowInstanceGO.Find("Window");
                if (NPCDetailWindow.Instance.window == null)
                {
                    BepInExLog.LogError("[NPCDetailWindow] InstantiateAndAttach: Cannot find window");
                    Destroy(NPCDetailWindow.Instance.gameObject);
                    return false;
                }
                NPCDetailWindow.Instance.scrollRect = windowInstanceGO.FindComponent<ScrollRect>("Window/Scroll View");
                if (NPCDetailWindow.Instance.scrollRect == null)
                {
                    BepInExLog.LogError("[NPCDetailWindow] InstantiateAndAttach: Cannot find scrollRect");
                    Destroy(NPCDetailWindow.Instance.gameObject);
                    return false;
                }
                NPCDetailWindow.Instance.detailText = windowInstanceGO.FindComponent<Text>("Window/Scroll View/Viewport/Content/Text");
                if (NPCDetailWindow.Instance.detailText == null)
                {
                    BepInExLog.LogError("[NPCDetailWindow] InstantiateAndAttach: Cannot find detailText");
                    Destroy(NPCDetailWindow.Instance.gameObject);
                    return false;
                }

            }

            return true;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                //BepInExLog.LogWarning("[NPCDetailWindow] Instance was destroyed.");
                Instance = null;
                return;
            }
        }

        public void SetShown(bool isShown)
        {
            this.gameObject.SetActive(isShown);
        }
        public bool GetShown()
        {
            return this.gameObject.activeSelf;
        }

        public void ResetScroll()
        {
            this.scrollRect.verticalNormalizedPosition = 1;
        }
        public void SetNPCData(NpcData npcData)
        {
            this.detailText.text = GetDetailText(npcData);
        }

        private static Dictionary<Define.ActorID, string> detailTextDic = null;
        private static IniFile textsIni = null;
        private static string GetDetailText(NpcData npcData)
        {
            if (detailTextDic == null)
                detailTextDic = new Dictionary<Define.ActorID, string>();

            if (detailTextDic.ContainsKey(npcData.actorId) == false)
            {
                var text_Birthday = "Birthday";
                var text_Loves = "Loves";
                var text_Likes = "Likes";
                var text_Dislikes = "Dislikes";
                var text_Hates = "Hates";

                if (textsIni == null)
                {
                    textsIni = new IniFile();
                    var textsIniPath = System.IO.Path.Combine(System.IO.Path.Combine(Paths.PluginPath, BepInExLoader.GUID), "texts.ini");
                    if (System.IO.File.Exists(textsIniPath) == false)
                    {
                        BepInExLog.LogWarning($"Cannot find Texts INI file at {textsIniPath}. use en default");
                    }
                    else
                    {
                        textsIni.Load(textsIniPath);
                    }
                }

                var lang = BootSystem.OptionData.SystemLanguage.ToString();
                IniSection iniSection = null;
                if (textsIni.TryGetSection(lang, out iniSection))
                {
                    IniValue iniValue = default;
                    if (iniSection.TryGetValue("TEXT_BIRTHDAY", out iniValue))
                        text_Birthday = iniValue.GetString();
                    if (iniSection.TryGetValue("TEXT_LOVES", out iniValue))
                        text_Loves = iniValue.GetString();
                    if (iniSection.TryGetValue("TEXT_LIKES", out iniValue))
                        text_Likes = iniValue.GetString();
                    if (iniSection.TryGetValue("TEXT_DISLIKES", out iniValue))
                        text_Dislikes = iniValue.GetString();
                    if (iniSection.TryGetValue("TEXT_HATES", out iniValue))
                        text_Hates = iniValue.GetString();
                }

                var text = string.Empty;

                Define.Season birthday_season;
                int birthday_day;
                if (npcData.TryFindNPCBirthday(out birthday_season, out birthday_day))
                {
                    var birthdayText = string.Empty;
                    switch (birthday_season)
                    {
                        case Define.Season.Spring:
                            birthdayText = $"{SV.UIRes.GetSystemText(UITextDic.DICID.HUDCLOCK_SPRING)} {birthday_day}";
                            break;
                        case Define.Season.Summer:
                            birthdayText = $"{SV.UIRes.GetSystemText(UITextDic.DICID.HUDCLOCK_SUMMER)} {birthday_day}";
                            break;
                        case Define.Season.Autumn:
                            birthdayText = $"{SV.UIRes.GetSystemText(UITextDic.DICID.HUDCLOCK_AUTUMN)} {birthday_day}";
                            break;
                        case Define.Season.Winter:
                            birthdayText = $"{SV.UIRes.GetSystemText(UITextDic.DICID.HUDCLOCK_WINTER)} {birthday_day}";
                            break;
                    }
                    text += $"<size=25>{text_Birthday}:</size> {birthdayText}\r\n\r\n";
                }

                text += $"<size=25>{text_Loves}</size>\r\n{string.Join(", ", npcData.GetVeryFavoriteItemDataTables().Select(x => $"{x.GetItemName()}"))}\r\n\r\n";
                text += $"<size=25>{text_Likes}</size>\r\n{string.Join(", ", npcData.GetFavoriteItemDataTables().Select(x => $"{x.GetItemName()}"))}\r\n\r\n";
                text += $"<size=25>{text_Dislikes}</size>\r\n{string.Join(", ", npcData.GetNotFavoriteItemDataTables().Select(x => $"{x.GetItemName()}"))}\r\n\r\n";
                text += $"<size=25>{text_Hates}</size>\r\n{string.Join(", ", npcData.GetNotFavoriteBadlyItemDataTables().Select(x => $"{x.GetItemName()}"))}";

                detailTextDic.Add(npcData.actorId, text);
            }
            return detailTextDic[npcData.actorId];
        }

        private void Update()
        {
            bool equipMenuItemDetailOpened = equipMenuItemDetail.gameObject.activeInHierarchy && equipMenuItemDetail.isOpen;
            if (!equipMenuItemDetailOpened != this.window.gameObject.activeSelf)
                this.window.gameObject.SetActive(!equipMenuItemDetailOpened);
        }
    }
}
