using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RF5.HisaCat.NPCDetails.Utils
{
    internal static class RF5DataExtension
    {
        public static string GetItemName(this ItemDataTable dataTable)
        {
            return SV.UIRes.GetText(SysTextGroup.ItemUINameData, dataTable.ItemIndex);
            //return dataTable.ScreenName; //same
        }
        public static string GetNpcName(this NpcData data)
        {
            return SV.UIRes.GetText(SysTextGroup.NPCNameData, data.NpcId);
            //return data.statusData.FocusName; //It returnes japanese name.
        }
        public static string GetNpcDiscript(this NpcData data)
        {
            return SV.UIRes.GetText(SysTextGroup.NPCDiscriptData, data.NpcId);
            //return data.statusData.FocusName; //It returnes japanese name.
        }

        public static List<ItemDataTable> ItemIdArrayToItemDataTables(IEnumerable<int> itemIds)
        {
            var items = new List<ItemDataTable>();
            if (itemIds == null) return items;
            foreach (var itemIdInt in itemIds)
            {
                var itemData = ItemDataTable.GetDataTable((ItemID)itemIdInt);
                if (itemData == null) continue;
                items.Add(itemData);
            }
            return items;
        }
        public static List<ItemDataTable> RemoveTrashItems(List<ItemDataTable> items)
        {
            return items.Where(x => x.ItemType != ItemType.Trash).ToList();
        }
        public static List<ItemDataTable> GetVeryFavoriteItemDataTables(this NpcData npcData)
        {
            //Loves
            if (npcData.statusData == null) return new List<ItemDataTable>();
            return ItemIdArrayToItemDataTables(npcData.statusData.VeryFavoriteItem);
        }
        public static List<ItemDataTable> GetFavoriteItemDataTables(this NpcData npcData)
        {
            //Likes
            if (npcData.statusData == null) return new List<ItemDataTable>();
            return ItemIdArrayToItemDataTables(npcData.statusData.FavoriteItem);
        }
        public static List<ItemDataTable> GetNotFavoriteItemDataTables(this NpcData npcData, bool exceptAlmostHates = false)
        {
            //Dislikes
            if (npcData.statusData == null) return new List<ItemDataTable>();

            if (exceptAlmostHates)
            {
                return ItemIdArrayToItemDataTables(npcData.statusData.NotFavoriteItem).Where(x =>
                {
                    var itemID = ItemDataTable.GetItemID(x.ItemIndex);
                    switch (itemID)
                    {
                        case ItemID.Item_Buttaix: //물체 X(2011)
                        case ItemID.Item_Kuzutetsu: //고철(2151)
                        case ItemID.Item_Zasso: //잡초(195)
                        case ItemID.Item_Karekusa: //마른 풀(196)
                        case ItemID.Item_Shippaisaku: //실패작(200)
                        case ItemID.Item_Choshippaisaku: //완전 실패작(201)
                        case ItemID.Item_Ishi: //돌(1500)
                        case ItemID.Item_Eda: //나뭇가지(1501)
                        case ItemID.Item_Akikan: //빈 캔(1900)
                        case ItemID.Item_Nagagutsu: //장화(1901)
                        case ItemID.Item_Reanaakikan: //희귀한 빈 캔(1902)
                        default:
                            return true;
                    }
                }).ToList();
            }
            else
                return ItemIdArrayToItemDataTables(npcData.statusData.NotFavoriteItem);
        }
        public static List<ItemDataTable> GetNotFavoriteBadlyItemDataTables(this NpcData npcData)
        {
            //Hates
            if (npcData.statusData == null) return new List<ItemDataTable>();
            return ItemIdArrayToItemDataTables(npcData.statusData.NotFavoriteBadlyItem);
        }
        //public static string GetPlaceName(Define.Place place)
        //{
        //    //if (place == Define.Place.None || place == Define.Place.MAX)
        //    //    return string.Empty;
        //    if (AreaManager.PlaceToFieldPlaceId.ContainsKey(place) == false)
        //        return string.Empty;

        //    return null;
        //}
    }
}
