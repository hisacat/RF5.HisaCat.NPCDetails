using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using RF5.HisaCat.NPCDetails.Utils;
using RF5.HisaCat.NPCDetails.Localization;
using BepInEx;

namespace RF5.HisaCat.NPCDetails
{
    internal class NPCDetailWindow : MonoBehaviour
    {
        private Transform window = null;
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
                NPCDetailWindow.Instance.detailText = windowInstanceGO.FindComponent<Text>("Window/TextArea/Mask/Text");
                if (NPCDetailWindow.Instance.detailText == null)
                {
                    BepInExLog.LogError("[NPCDetailWindow] InstantiateAndAttach: Cannot find detailText");
                    Destroy(NPCDetailWindow.Instance.gameObject);
                    return false;
                }
            }

            //Check "/StartUI/UIMainManager/UIMainCanvas(Clone)/MainWindowCanvas/CampUI/CampMenuObject/CenterMenu/FriendlyMenu/NPCPage(Clone)/StatusObj/FriendsStatus/DiscriptionBoard" for support controllers

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
        public void SetNPCData(NpcData npcData)
        {
            this.detailText.text = GetDetailText(npcData);

            //bool wasTodayTalked = npcData.TodayTalkCount > 0;

            //var presentItemTypesArray = npcData.PresentItemTypes.ToArray();
            //bool wasPresentVeryFavorite = presentItemTypesArray.Any(x => x == LovePointManager.FavoriteType.VeryFavorite);
            //bool wasPresentFavorite = presentItemTypesArray.Any(x => x == LovePointManager.FavoriteType.Favorite);
            //bool wasPresentNormal = presentItemTypesArray.Any(x => x == LovePointManager.FavoriteType.Normal);
            //bool wasPresentNotFavorite = presentItemTypesArray.Any(x => x == LovePointManager.FavoriteType.NotFavorite);
            //bool wasPresentNotFavoriteBadly = presentItemTypesArray.Any(x => x == LovePointManager.FavoriteType.NotFavoriteBadly);

            //BepInExLog.Log($"wasPresentVeryFavorite: {wasTodayTalked}");
            //BepInExLog.Log($"wasPresentVeryFavorite: {wasPresentVeryFavorite}");
            //BepInExLog.Log($"wasPresentFavorite: {wasPresentFavorite}");
            //BepInExLog.Log($"wasPresentNormal: {wasPresentNormal}");
            //BepInExLog.Log($"wasPresentNotFavorite: {wasPresentNotFavorite}");
            //BepInExLog.Log($"wasPresentNotFavoriteBadly: {wasPresentNotFavoriteBadly}\r\n");
        }

        private static Dictionary<Define.ActorID, string> detailTextDic = null;
        private static string GetDetailText(NpcData npcData)
        {
            if (detailTextDic == null)
                detailTextDic = new Dictionary<Define.ActorID, string>();

            if (detailTextDic.ContainsKey(npcData.actorId) == false)
            {
                var text_Birthday = LocalizationManager.Load("TEXT_BIRTHDAY");
                var text_Loves = LocalizationManager.Load("TEXT_LOVES");
                var text_Likes = LocalizationManager.Load("TEXT_LIKES");
                var text_Dislikes = LocalizationManager.Load("TEXT_DISLIKES");
                var text_Hates = LocalizationManager.Load("TEXT_HATES");

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

                //https://www.nexusmods.com/runefactory5/mods/34?tab=bugs
                //Hotfix for 'Text cut off' bugs
                text += "\r\n";

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
