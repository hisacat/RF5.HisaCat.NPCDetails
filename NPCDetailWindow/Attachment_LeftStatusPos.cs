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
    /// Attachments for LeftStatusPos
    /// </summary>
    internal class Attachment_LeftStatusPos : MonoBehaviour
    {
        internal static Attachment_LeftStatusPos Instance { get; private set; }

        public const string PrefabPathFromBundle = "[RF5.HisaCat.NPCDetails]LeftStatusPos";

        public const string AttachPathBasedFriendPageStatusDisp = "StatusObj/FriendsStatus/LeftStatusPos";
        public static class TransformPaths
        {
            public const string Status_NPC = "RF5ContentsArea/Status_NPC";
            public const string NPC_Tips_TodayTalked = "TipsArea/Tips_TodayTalked";
            public const string NPC_Tips_TodayTalked_Text = "Text";
            public const string NPC_Tips_TodayTalked_Checked = "Checkbox/checked";

            public const string NPC_Tips_WasPresent = "TipsArea/Tips_WasPresent";
            public const string NPC_Tips_WasPresent_Text = "Text";
            public const string NPC_Tips_WasPresent_CheckedNormal = "Checkbox_Normal/checked";
            public const string NPC_Tips_WasPresent_CheckedLikes = "Checkbox_Likes/checked";
            public const string NPC_Tips_WasPresent_CheckedLoves = "Checkbox_Loves/checked";

            public const string NPC_Tips_BirthdayLeft = "TipsArea/Tips_BirthdayLeft";
            public const string NPC_Tips_BirthdayLeft_Text = "Text";

            public const string NPC_Tips_Alert_BirthdayToday = "TipsArea/Tips_Alert_BirthdayToday";
            public const string NPC_Tips_Alert_BirthdayToday_Text = "Text";

            public const string Status_Monster = "RF5ContentsArea/Status_NPC";
        }


        public GameObject m_Status_NPC_GO = null;
        public GameObject m_Status_Monster_GO = null;

        public GameObject m_Tips_TodayTalked_GO = null;
        public Text m_Tips_TodayTalked_Text = null;
        public GameObject m_Tips_TodayTalked_Checked = null;

        public GameObject m_Tips_WasPresent_GO = null;
        public Text m_Tips_WasPresent_Text = null;
        public GameObject m_Tips_WasPresent_CheckedNormal;
        public GameObject m_Tips_WasPresent_CheckedLikes;
        public GameObject m_Tips_WasPresent_CheckedLoves;

        public GameObject m_Tips_BirthdayLeft_GO = null;
        public Text m_Tips_BirthdayLeft_Text = null;

        public GameObject m_Tips_Alert_BirthdayToday_GO = null;
        public Text m_Tips_Alert_BirthdayToday_Text = null;
        public bool PreloadPathes()
        {
            {
                GameObject root;
                if (this.TryFindGameObject(TransformPaths.Status_NPC, out root) == false) return false;
                this.m_Status_NPC_GO = root;
                {
                    GameObject parent;
                    if (root.TryFindGameObject(TransformPaths.NPC_Tips_TodayTalked, out parent) == false) return false;
                    this.m_Tips_TodayTalked_GO = parent;
                    if (parent.TryFindComponent<Text>(TransformPaths.NPC_Tips_TodayTalked_Text, out this.m_Tips_TodayTalked_Text) == false) return false;
                    if (parent.TryFindGameObject(TransformPaths.NPC_Tips_TodayTalked_Checked, out this.m_Tips_TodayTalked_Checked) == false) return false;
                }

                {
                    GameObject parent;
                    if (root.TryFindGameObject(TransformPaths.NPC_Tips_WasPresent, out parent) == false) return false;
                    this.m_Tips_WasPresent_GO = parent;
                    if (parent.TryFindComponent<Text>(TransformPaths.NPC_Tips_WasPresent_Text, out this.m_Tips_WasPresent_Text) == false) return false;
                    if (parent.TryFindGameObject(TransformPaths.NPC_Tips_WasPresent_CheckedNormal, out this.m_Tips_WasPresent_CheckedNormal) == false) return false;
                    if (parent.TryFindGameObject(TransformPaths.NPC_Tips_WasPresent_CheckedLikes, out this.m_Tips_WasPresent_CheckedLikes) == false) return false;
                    if (parent.TryFindGameObject(TransformPaths.NPC_Tips_WasPresent_CheckedLoves, out this.m_Tips_WasPresent_CheckedLoves) == false) return false;
                }

                {
                    GameObject parent;
                    if (root.TryFindGameObject(TransformPaths.NPC_Tips_BirthdayLeft, out parent) == false) return false;
                    this.m_Tips_BirthdayLeft_GO = parent;
                    if (parent.TryFindComponent<Text>(TransformPaths.NPC_Tips_BirthdayLeft_Text, out this.m_Tips_BirthdayLeft_Text) == false) return false;
                }
                {
                    GameObject parent;
                    if (root.TryFindGameObject(TransformPaths.NPC_Tips_Alert_BirthdayToday, out parent) == false) return false;
                    this.m_Tips_Alert_BirthdayToday_GO = parent;
                    if (parent.TryFindComponent<Text>(TransformPaths.NPC_Tips_Alert_BirthdayToday_Text, out this.m_Tips_Alert_BirthdayToday_Text) == false) return false;
                }
            }
            {
                GameObject root;
                if (this.TryFindGameObject(TransformPaths.Status_Monster, out root) == false) return false;
                this.m_Status_Monster_GO = root;
            }
            return true;
        }

        private FriendPageStatusDisp friendPageStatusDisp = null;
        private string tips_BirthdayLeft_Text_Format = string.Empty;
        public bool Init(FriendPageStatusDisp friendPageStatusDisp)
        {
            this.friendPageStatusDisp = friendPageStatusDisp;
            if (PreloadPathes() == false)
                return false;

            this.m_Tips_TodayTalked_Text.text = LocalizationManager.Load("tips.title.talk_today");
            this.m_Tips_WasPresent_Text.text = LocalizationManager.Load("tips.title.was_present");
            this.tips_BirthdayLeft_Text_Format = LocalizationManager.Load("tips.title.birthday_left");
            this.m_Tips_Alert_BirthdayToday_Text.text = LocalizationManager.Load("tips.title.birthday_today");

            return true;
        }

        public static bool InstantiateAndAttach(FriendPageStatusDisp friendPageStatusDisp)
        {
            if (Instance != null)
            {
                BepInExLog.LogDebug("[Attachment_LeftStatusPos] InstantiateAndAttach: instance already exist");
                return true;
            }

            var attachTarget = friendPageStatusDisp.transform.Find(AttachPathBasedFriendPageStatusDisp);
            if (attachTarget == null)
            {
                BepInExLog.LogError("[Attachment_LeftStatusPos] InstantiateAndAttach: Cannot find attachTarget");
                return false;
            }

            var prefab = BundleLoader.MainBundle.LoadIL2CPP<GameObject>(PrefabPathFromBundle);
            if (prefab == null)
            {
                BepInExLog.LogError("[Attachment_LeftStatusPos] InstantiateAndAttach: Cannot load prefab");
                return false;
            }

            var InstanceGO = GameObject.Instantiate(prefab, attachTarget.transform);
            if (InstanceGO == null)
            {
                BepInExLog.LogError("[Attachment_LeftStatusPos] InstantiateAndAttach: Cannot instantiate window");
                return false;
            }
            RF5FontHelper.SetFontGlobal(InstanceGO);

            Instance = InstanceGO.AddComponent<Attachment_LeftStatusPos>();
            if (Instance.Init(friendPageStatusDisp) == false)
            {
                BepInExLog.LogError("[Attachment_LeftStatusPos] InstantiateAndAttach: PreloadPathes failed");
                Instance = null; Destroy(InstanceGO);
                return false;
            }

            BepInExLog.LogDebug("[Attachment_LeftStatusPos] Attached");
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
            this.m_Status_NPC_GO.SetActive(true);
            this.m_Status_Monster_GO.SetActive(false);

            this.m_Tips_TodayTalked_GO.SetActive(false);
            this.m_Tips_WasPresent_GO.SetActive(false);
            this.m_Tips_BirthdayLeft_GO.SetActive(false);
            this.m_Tips_Alert_BirthdayToday_GO.SetActive(false);
            {
                this.m_Tips_TodayTalked_GO.SetActive(true);

                bool wasTodayTalked = npcData.TodayTalkCount > 0;
                this.m_Tips_TodayTalked_Checked.SetActive(wasTodayTalked);
            }
            {
                this.m_Tips_WasPresent_GO.SetActive(true);

                var presentItemTypesArray = npcData.PresentItemTypes.ToArray();
                bool wasPresentNormal = presentItemTypesArray.Any(x => x == LovePointManager.FavoriteType.Normal);
                bool wasPresentFavorite = presentItemTypesArray.Any(x => x == LovePointManager.FavoriteType.Favorite);
                bool wasPresentVeryFavorite = presentItemTypesArray.Any(x => x == LovePointManager.FavoriteType.VeryFavorite);

                this.m_Tips_WasPresent_CheckedNormal.SetActive(wasPresentNormal);
                this.m_Tips_WasPresent_CheckedLikes.SetActive(wasPresentFavorite);
                this.m_Tips_WasPresent_CheckedLoves.SetActive(wasPresentVeryFavorite);
            }

            var isBirthday = NpcDataManager.Instance.LovePointManager.IsBirthDay(npcData.NpcId);
            {
                if (isBirthday)
                {
                    this.m_Tips_BirthdayLeft_GO.SetActive(false);
                }
                else
                {
                    this.m_Tips_BirthdayLeft_GO.SetActive(true);

                    Define.Season birthday_season;
                    int birthday_day;
                    if (npcData.TryFindNPCBirthday(out birthday_season, out birthday_day))
                    {
                        var local_Today = ((int)TimeManager.Instance.Season * 30) + TimeManager.Instance.Day;
                        var local_Birthday = ((int)birthday_season * 30) + birthday_day;
                        int leftDay = 0;
                        if (local_Today < local_Birthday) //Birthday not spend yet
                            leftDay = local_Birthday - local_Today;
                        else
                            leftDay = ((4 * 30) - local_Today) + local_Birthday;
                        this.m_Tips_BirthdayLeft_Text.text = string.Format(this.tips_BirthdayLeft_Text_Format, leftDay);
                    }
                    else
                    {
                        this.m_Tips_BirthdayLeft_GO.SetActive(false);
                        BepInExLog.LogError($"[Attachment_LeftStatusPos] Cannot find {npcData.actorId}'s birthday");
                    }
                }
            }
            {
                if (isBirthday)
                {
                    this.m_Tips_Alert_BirthdayToday_GO.SetActive(true);
                }
                else
                {
                    this.m_Tips_Alert_BirthdayToday_GO.SetActive(false);
                }
            }
        }

        public void SetMonsterData(int pageId, MonsterDataTable monsterData)
        {
            this.m_Status_NPC_GO.SetActive(false);
            this.m_Status_Monster_GO.SetActive(true);

        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                BepInExLog.LogDebug("[Attachment_LeftStatusPos] Destroyed");
                Instance = null;
                return;
            }
        }
    }
}
