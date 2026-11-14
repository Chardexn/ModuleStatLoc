using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace ModuleStatLoc
{
    /// <summary>
    /// 读档后刷新已生成物品上的模组名和说明书。这些字段在生成时拷贝，不会再查本地化表。
    /// 必须挂 OnGameLoadedLate：Normal 触发时存档物品尚未 Decode。
    /// 说明书标题从 Item 表扫描 `*_instruction_name`，游戏加新机器或改正文时会跟官方词条走。
    /// </summary>
    internal static class ItemTextRefresh
    {
        private const string ItemTable = "Item";
        private const string InstructionNameSuffix = "_instruction_name";
        private const string NameSuffix = "_name";

        private static readonly Dictionary<string, string> ModuleNameKeys = new Dictionary<string, string>
        {
            ["system_module_performance"] = "item_system_module_performance_name",
            ["system_module_efficiancy"] = "item_system_module_efficiancy_name",
            ["system_module_quality"] = "item_system_module_quality_name"
        };

        private static readonly string[] FallbackInstructionPrefixes =
        {
            "item_alarm_instruction",
            "item_desequencer_instruction",
            "item_furnace_instruction",
            "item_projector_instruction",
            "item_moisture_farm_instruction",
            "item_water_purifier_instruction",
            "item_age_well_instruction",
            "item_evaporator_instruction",
            "item_3d_printer_instruction",
            "item_hydroponic_instruction",
            "item_dna_manipulator_instruction"
        };

        private static Dictionary<string, string> prefixByName;
        private static string cachedLocale;

        public static void Run()
        {
            if (!StatWords.IsChineseLocale())
            {
                return;
            }

            if (EmporiumEntry.Instance == null)
            {
                return;
            }

            HashSet<GameItem> seen = new HashSet<GameItem>();
            RefreshList(EmporiumEntry.Instance.GetAllOwnedItems(), seen);
            RefreshList(EmporiumEntry.Instance.GetAllItems(), seen);
        }

        public static void RunAfterNewGame(string _)
        {
            Run();
        }

        #region MARK: 遍历
        private static void RefreshList(List<GameItem> items, HashSet<GameItem> seen)
        {
            if (items == null)
            {
                return;
            }

            foreach (GameItem item in items)
            {
                if (item == null || !seen.Add(item))
                {
                    continue;
                }

                Refresh(item);
            }
        }
        #endregion

        #region MARK: 单件
        private static void Refresh(GameItem item)
        {
            if (ModuleNameKeys.TryGetValue(item.identifier, out string locKey))
            {
                item.name = LocHelper.GetLocalizedItem(locKey);
                return;
            }

            if (!IsManualCandidate(item))
            {
                return;
            }

            if (TryRefreshByInstructionName(item))
            {
                return;
            }

            if (StatWords.LooksLikeModuleImpact(item.flavorText))
            {
                item.flavorText = StatWords.Rewrite(item.flavorText);
            }
        }

        private static bool IsManualCandidate(GameItem item)
        {
            if (item.IsTag(MachineHelper.MANUAL_TAG))
            {
                return true;
            }

            if (item.identifier == "postit")
            {
                return true;
            }

            return StatWords.LooksLikeModuleImpact(item.flavorText);
        }

        private static bool TryRefreshByInstructionName(GameItem item)
        {
            if (string.IsNullOrEmpty(item.name))
            {
                return false;
            }

            if (!PrefixByName().TryGetValue(item.name, out string prefix))
            {
                return false;
            }

            item.flavorText = LocHelper.GetLocalizedItem(prefix + "_flavor");
            TryRefreshShort(item, prefix + "_short");
            return true;
        }

        private static void TryRefreshShort(GameItem item, string key)
        {
            if (string.IsNullOrEmpty(item.shortDescription))
            {
                return;
            }

            string text = LocHelper.GetLocalizedItem(key);
            if (string.IsNullOrEmpty(text) || text == key)
            {
                return;
            }

            if (LocHelper.TryParseMissingTranslation(text, out _, out _))
            {
                return;
            }

            item.shortDescription = text;
        }
        #endregion

        #region MARK: 说明书词条
        private static Dictionary<string, string> PrefixByName()
        {
            string locale = LocHelper.GetCurrentLocaleCode();
            if (prefixByName != null && cachedLocale == locale)
            {
                return prefixByName;
            }

            cachedLocale = locale;
            prefixByName = new Dictionary<string, string>();
            if (!TryScanInstructionNames(prefixByName))
            {
                AddFallbackPrefixes(prefixByName);
            }

            return prefixByName;
        }

        private static bool TryScanInstructionNames(Dictionary<string, string> map)
        {
            StringTable table = LocalizationSettings.StringDatabase.GetTable(ItemTable);
            if (table == null)
            {
                return false;
            }

            int found = 0;
            foreach (StringTableEntry entry in table.Values)
            {
                string key = EntryKey(entry);
                if (string.IsNullOrEmpty(key) || !key.EndsWith(InstructionNameSuffix, StringComparison.Ordinal))
                {
                    continue;
                }

                string name = LocHelper.GetLocalizedItem(key);
                if (string.IsNullOrEmpty(name)
                    || name == key
                    || LocHelper.TryParseMissingTranslation(name, out _, out _))
                {
                    continue;
                }

                string prefix = key.Substring(0, key.Length - NameSuffix.Length);
                map[name] = prefix;
                found++;
            }

            return found > 0;
        }

        private static void AddFallbackPrefixes(Dictionary<string, string> map)
        {
            foreach (string prefix in FallbackInstructionPrefixes)
            {
                string name = LocHelper.GetLocalizedItem(prefix + NameSuffix);
                if (string.IsNullOrEmpty(name) || name == prefix + NameSuffix)
                {
                    continue;
                }

                map[name] = prefix;
            }
        }

        private static string EntryKey(StringTableEntry entry)
        {
            if (entry?.SharedEntry != null && !string.IsNullOrEmpty(entry.SharedEntry.Key))
            {
                return entry.SharedEntry.Key;
            }

            return Convert.ToString(entry?.Key);
        }
        #endregion
    }
}
