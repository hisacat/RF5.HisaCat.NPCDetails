using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using RF5.HisaCat.NPCDetails.Utils;

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
            public const string Tips_TodayTalked = "RF5ContentsArea/TipsArea/Tips_TodayTalked";
            public const string Tips_TodayTalked_Text = "Text";
            public const string Tips_TodayTalked_Checked = "Checkbox/checked";

            public const string Tips_WasPresent = "RF5ContentsArea/TipsArea/Tips_WasPresent";
            public const string Tips_WasPresent_Text = "Text";
            public const string Tips_WasPresent_CheckedNormal = "Checkbox_Normal/checked";
            public const string Tips_WasPresent_CheckedLikes = "Checkbox_Likes/checked";
            public const string Tips_WasPresent_CheckedLoves = "Checkbox_Loves/checked";

            public const string Tips_BirthdayLeft = "RF5ContentsArea/TipsArea/Tips_BirthdayLeft";
            public const string Tips_BirthdayLeft_Text = "Text";

            public const string Tips_Alert_BirthdayToday = "RF5ContentsArea/TipsArea/Tips_Alert_BirthdayToday";
            public const string Tips_Alert_BirthdayToday_Text = "Text";
        }

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
                GameObject parent;
                if (this.TryFindGameObject(TransformPaths.Tips_TodayTalked, out parent) == false) return false;
                this.m_Tips_TodayTalked_GO = parent;
                if (parent.TryFindComponent<Text>(TransformPaths.Tips_TodayTalked_Text, out this.m_Tips_TodayTalked_Text) == false) return false;
                if (parent.TryFindGameObject(TransformPaths.Tips_TodayTalked_Checked, out this.m_Tips_TodayTalked_Checked) == false) return false;
            }

            {
                GameObject parent;
                if (this.TryFindGameObject(TransformPaths.Tips_WasPresent, out parent) == false) return false;
                this.m_Tips_WasPresent_GO = parent;
                if (parent.TryFindComponent<Text>(TransformPaths.Tips_WasPresent_Text, out this.m_Tips_WasPresent_Text) == false) return false;
                if (parent.TryFindGameObject(TransformPaths.Tips_WasPresent_CheckedNormal, out this.m_Tips_WasPresent_CheckedNormal) == false) return false;
                if (parent.TryFindGameObject(TransformPaths.Tips_WasPresent_CheckedLikes, out this.m_Tips_WasPresent_CheckedLikes) == false) return false;
                if (parent.TryFindGameObject(TransformPaths.Tips_WasPresent_CheckedLoves, out this.m_Tips_WasPresent_CheckedLoves) == false) return false;
            }

            {
                GameObject parent;
                if (this.TryFindGameObject(TransformPaths.Tips_BirthdayLeft, out parent) == false) return false;
                this.m_Tips_BirthdayLeft_GO = parent;
                if (parent.TryFindComponent<Text>(TransformPaths.Tips_BirthdayLeft_Text, out this.m_Tips_BirthdayLeft_Text) == false) return false;
            }
            {
                GameObject parent;
                if (this.TryFindGameObject(TransformPaths.Tips_Alert_BirthdayToday, out parent) == false) return false;
                this.m_Tips_Alert_BirthdayToday_GO = parent;
                if (parent.TryFindComponent<Text>(TransformPaths.Tips_Alert_BirthdayToday_Text, out this.m_Tips_Alert_BirthdayToday_Text) == false) return false;
            }

            return true;
        }

        private FriendPageStatusDisp friendPageStatusDisp = null;
        public bool Init(FriendPageStatusDisp friendPageStatusDisp)
        {
            this.friendPageStatusDisp = friendPageStatusDisp;
            return PreloadPathes();
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
