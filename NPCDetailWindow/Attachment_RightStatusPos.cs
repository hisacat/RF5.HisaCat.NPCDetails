using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using RF5.HisaCat.NPCDetails.Utils;
using RF5.HisaCat.NPCDetails.Localization;

namespace RF5.HisaCat.NPCDetails.NPCDetailWindow
{
    /// <summary>
    /// Attachments for RightStatusPos
    /// </summary>
    internal class Attachment_RightStatusPos : MonoBehaviour
    {
        internal static Attachment_RightStatusPos Instance { get; private set; }

        public const string PrefabPathFromBundle = "[RF5.HisaCat.NPCDetails]RightStatusPos";

        public const string AttachPathBasedFriendPageStatusDisp = "StatusObj/FriendsStatus/RightStatusPos";
        public const string EquipMenuItemDetailWindowPath = "StatusObj/FriendsStatus/EquipMenuItemDetail/OnOffWindows";

        public static class TransformPaths
        {
            public const string Window = "Window";
            public const string Window_NPCDetailText = "TextArea/Mask/Text";
        }

        private GameObject m_Window_GO = null;
        private Text m_NPCDetailText = null;
        public bool PreloadPathes()
        {
            {
                GameObject parent;
                if (this.TryFindGameObject(TransformPaths.Window, out this.m_Window_GO) == false) return false;
                parent = this.m_Window_GO;
                if (parent.TryFindComponent<Text>(TransformPaths.Window_NPCDetailText, out this.m_NPCDetailText) == false) return false;
            }

            return true;
        }

        private FriendPageStatusDisp friendPageStatusDisp = null;
        private UIOnOffAnimate equipMenuItemDetail = null;
        public bool Init(FriendPageStatusDisp friendPageStatusDisp, UIOnOffAnimate equipMenuItemDetail)
        {
            this.friendPageStatusDisp = friendPageStatusDisp;
            this.equipMenuItemDetail = equipMenuItemDetail;
            return PreloadPathes();
        }

        public static bool InstantiateAndAttach(FriendPageStatusDisp friendPageStatusDisp)
        {
            if (Instance != null)
            {
                BepInExLog.LogDebug("[Attachment_RightStatusPos] InstantiateAndAttach: instance already exist");
                return true;
            }

            var attachTarget = friendPageStatusDisp.transform.Find(AttachPathBasedFriendPageStatusDisp);
            if (attachTarget == null)
            {
                BepInExLog.LogError("[Attachment_RightStatusPos] InstantiateAndAttach: Cannot find attachTarget");
                return false;
            }

            var equipMenuItemDetail = friendPageStatusDisp.FindComponent<UIOnOffAnimate>(EquipMenuItemDetailWindowPath);
            if (equipMenuItemDetail == null)
            {
                BepInExLog.LogError("[Attachment_RightStatusPos] InstantiateAndAttach: Cannot find equipMenuItemDetailWindow");
                return false;
            }

            var prefab = BundleLoader.MainBundle.LoadIL2CPP<GameObject>(PrefabPathFromBundle);
            if (prefab == null)
            {
                BepInExLog.LogError("[Attachment_RightStatusPos] InstantiateAndAttach: Cannot load prefab");
                return false;
            }

            var InstanceGO = GameObject.Instantiate(prefab, attachTarget.transform);
            if (InstanceGO == null)
            {
                BepInExLog.LogError("[Attachment_RightStatusPos] InstantiateAndAttach: Cannot instantiate window");
                return false;
            }
            RF5FontHelper.SetFontGlobal(InstanceGO);

            Instance = InstanceGO.AddComponent<Attachment_RightStatusPos>();
            if (Instance.Init(friendPageStatusDisp, equipMenuItemDetail) == false)
            {
                BepInExLog.LogError("[Attachment_RightStatusPos] InstantiateAndAttach: Initialize failed");
                Instance = null; Destroy(InstanceGO);
                return false;
            }

            BepInExLog.LogDebug("[Attachment_RightStatusPos] Attached");
            return true;
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
            this.m_NPCDetailText.text = GetDetailText(npcData);
        }

        private static Dictionary<Define.ActorID, string> detailTextDic = null;
        private static string GetDetailText(NpcData npcData)
        {
            if (detailTextDic == null)
                detailTextDic = new Dictionary<Define.ActorID, string>();

            if (detailTextDic.ContainsKey(npcData.actorId) == false)
            {
                var text_Birthday = LocalizationManager.Load("detail.title.birthday");
                var text_Loves = LocalizationManager.Load("detail.title.loves");
                var text_Likes = LocalizationManager.Load("detail.title.likes");
                var text_Dislikes = LocalizationManager.Load("detail.title.dislikes");
                var text_Hates = LocalizationManager.Load("detail.title.hates");

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
        public void SetMonsterData(int pageId, MonsterDataTable monsterData)
        {

        }

        private void Update()
        {
            bool equipMenuItemDetailOpened = equipMenuItemDetail.gameObject.activeInHierarchy && equipMenuItemDetail.isOpen;
            if (!equipMenuItemDetailOpened != this.m_Window_GO.gameObject.activeSelf)
                this.m_Window_GO.gameObject.SetActive(!equipMenuItemDetailOpened);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                BepInExLog.LogDebug("[Attachment_RightStatusPos] Destroyed");
                Instance = null;
                return;
            }
        }
    }
}
