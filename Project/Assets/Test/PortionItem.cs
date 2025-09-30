using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Backend.Util.Data;
using Backend.Util.Item.Base;
using UnityEngine;

namespace Backend.Util.Item
{
    /// <summary> 수량 아이템 - 포션 아이템 </summary>
    [System.Serializable]
    public class PortionItem : CountableItem, IUsableItem
    {
        public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) { }

        public bool Use()
        {
            // 임시 : 개수 하나 감소
            Amount--;

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, amount);
        }
    }
}

