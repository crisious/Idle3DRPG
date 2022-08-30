﻿using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.ITEM_REFINE_FILE, menuName = GameDataMenuConsts.ITEM_REFINE_MENU, order = GameDataMenuConsts.ITEM_REFINE_ORDER)]
    public partial class ItemRefine : BaseGameData
    {
        [Category("Item Refine Settings")]
        [SerializeField]
        private Color titleColor = Color.clear;
        public Color TitleColor { get { return titleColor; } }

        [SerializeField]
        [Tooltip("This is refine level, each level have difference success rate, required items, required gold")]
        private ItemRefineLevel[] levels = new ItemRefineLevel[0];
        public ItemRefineLevel[] Levels { get { return levels; } }

        [SerializeField]
        [Tooltip("This is repair prices, should order from high to low durability rate")]
        private ItemRepairPrice[] repairPrices = new ItemRepairPrice[0];
        public ItemRepairPrice[] RepairPrices { get { return repairPrices; } }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (Levels != null && Levels.Length > 0)
            {
                foreach (ItemRefineLevel entry in Levels)
                {
                    GameInstance.AddItems(entry.RequireItems);
                    GameInstance.AddCurrencies(entry.RequireCurrencies);
                }
            }
            if (RepairPrices != null && RepairPrices.Length > 0)
            {
                foreach (ItemRepairPrice entry in RepairPrices)
                {
                    GameInstance.AddItems(entry.RequireItems);
                    GameInstance.AddCurrencies(entry.RequireCurrencies);
                }
            }
        }
    }

    [System.Serializable]
    public partial struct ItemRefineLevel
    {
        [Range(0.01f, 1f)]
        [SerializeField]
        private float successRate;
        public float SuccessRate { get { return successRate; } }

        [SerializeField]
        [ArrayElementTitle("item")]
        private ItemAmount[] requireItems;
        public ItemAmount[] RequireItems { get { return requireItems; } }

        [SerializeField]
        [ArrayElementTitle("currency")]
        private CurrencyAmount[] requireCurrencies;
        public CurrencyAmount[] RequireCurrencies { get { return requireCurrencies; } }

        [SerializeField]
        private int requireGold;
        public int RequireGold { get { return requireGold; } }

        [Tooltip("How many levels it will be decreased if refining failed")]
        [SerializeField]
        private short refineFailDecreaseLevels;
        public short RefineFailDecreaseLevels { get { return refineFailDecreaseLevels; } }

        [Tooltip("It will be destroyed if this value is TRUE and refining failed")]
        [SerializeField]
        private bool refineFailDestroyItem;
        public bool RefineFailDestroyItem { get { return refineFailDestroyItem; } }

        public ItemRefineLevel(
            float successRate,
            ItemAmount[] requireItems,
            CurrencyAmount[] requireCurrencies,
            int requireGold,
            short refineFailDecreaseLevels,
            bool refineFailDestroyItem)
        {
            this.successRate = successRate;
            this.requireItems = requireItems;
            this.requireCurrencies = requireCurrencies;
            this.requireGold = requireGold;
            this.refineFailDecreaseLevels = refineFailDecreaseLevels;
            this.refineFailDestroyItem = refineFailDestroyItem;
        }

        public bool CanRefine(IPlayerCharacterData character)
        {
            return CanRefine(character, out _);
        }

        public bool CanRefine(IPlayerCharacterData character, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            if (!GameInstance.Singleton.GameplayRule.CurrenciesEnoughToRefineItem(character, this))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                return false;
            }
            if (requireItems == null || requireItems.Length == 0)
                return true;
            // Count required items
            foreach (ItemAmount requireItem in requireItems)
            {
                if (requireItem.item != null && character.CountNonEquipItems(requireItem.item.DataId) < requireItem.amount)
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }
            }
            return true;
        }
    }

    [System.Serializable]
    public partial struct ItemRepairPrice
    {
        [Range(0.01f, 1f)]
        [SerializeField]
        private float durabilityRate;
        public float DurabilityRate { get { return durabilityRate; } }

        [SerializeField]
        [ArrayElementTitle("item")]
        private ItemAmount[] requireItems;
        public ItemAmount[] RequireItems { get { return requireItems; } }

        [SerializeField]
        [ArrayElementTitle("currency")]
        private CurrencyAmount[] requireCurrencies;
        public CurrencyAmount[] RequireCurrencies { get { return requireCurrencies; } }

        [SerializeField]
        private int requireGold;
        public int RequireGold { get { return requireGold; } }

        public ItemRepairPrice(
            float durabilityRate,
            ItemAmount[] requireItems,
            CurrencyAmount[] requireCurrencies,
            int requireGold)
        {
            this.durabilityRate = durabilityRate;
            this.requireItems = requireItems;
            this.requireCurrencies = requireCurrencies;
            this.requireGold = requireGold;
        }

        public bool CanRepair(IPlayerCharacterData character)
        {
            return CanRepair(character, out _);
        }

        public bool CanRepair(IPlayerCharacterData character, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            if (!GameInstance.Singleton.GameplayRule.CurrenciesEnoughToRepairItem(character, this))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                return false;
            }
            if (requireItems == null || requireItems.Length == 0)
                return true;
            // Count required items
            foreach (ItemAmount requireItem in requireItems)
            {
                if (requireItem.item != null && character.CountNonEquipItems(requireItem.item.DataId) < requireItem.amount)
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }
            }
            return true;
        }
    }
}
