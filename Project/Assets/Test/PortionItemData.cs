using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Data.Base;
using Backend.Util.Item;
using UnityEngine;

// 날짜 : 2021-03-28 PM 10:42:48
// 작성자 : Rito

namespace Backend.Util.Data
{
    /// <summary> 소비 아이템 정보 </summary>
    [CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
    public class PortionItemData : CountableItemData
    {
        /// <summary> 효과량(회복량 등) </summary>
        public float Value => _value;
        [SerializeField] private float _value;
        public override Item.Base.Item CreateItem()
        {
            return new PortionItem(this);
        }
    }
}
