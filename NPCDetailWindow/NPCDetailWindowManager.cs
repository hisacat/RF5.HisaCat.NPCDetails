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

namespace RF5.HisaCat.NPCDetails.NPCDetailWindow
{
    internal class NPCDetailWindowManager
    {
        private static NPCDetailWindowManager Instance;
        public NPCDetailWindowManager()
        {
            Instance = this;
        }

        private static UIOnOffAnimate equipMenuItemDetail = null;
        public static void TryAttachIfNotExist(FriendPageStatusDisp friendPageStatusDisp)
        {
            if (Attachment_LeftStatusPos.Instance == null)
                Attachment_LeftStatusPos.InstantiateAndAttach(friendPageStatusDisp);

            if (Attachment_RightStatusPos.Instance == null)
                Attachment_RightStatusPos.InstantiateAndAttach(friendPageStatusDisp);
        }
        public static void TrySetNPCData(NpcData npcData)
        {
            Attachment_LeftStatusPos.Instance?.SetNPCData(npcData);
            Attachment_RightStatusPos.Instance?.SetNPCData(npcData);
        }
        public static void TrySetMonsterData(int pageId, MonsterDataTable monsterData)
        {
            Attachment_LeftStatusPos.Instance?.SetMonsterData(pageId, monsterData);
            Attachment_RightStatusPos.Instance?.SetMonsterData(pageId, monsterData);
        }
        public static void TrySetShown(bool isShown)
        {
            Attachment_LeftStatusPos.Instance?.SetShown(isShown);
            Attachment_RightStatusPos.Instance?.SetShown(isShown);
        }
    }
}
