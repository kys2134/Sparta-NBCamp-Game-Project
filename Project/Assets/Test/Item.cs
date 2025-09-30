using System.Collections;
using System.Collections.Generic;
using Backend.Util.Data.Base;
using UnityEngine;

namespace Backend.Util.Item.Base
{
    /*
        [상속 구조]
        Item : 기본 아이템
            - EquipmentItem : 장비 아이템
            - CountableItem : 수량이 존재하는 아이템
    */
    public abstract class Item
    {
        public ItemData Data { get; private set; }
        public bool IsEquipped = false;

        public Item(ItemData data) => Data = data;
    }
}

