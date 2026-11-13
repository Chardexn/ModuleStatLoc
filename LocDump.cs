using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace ModuleStatLoc
{
    /// <summary>
    /// F9 调试：把 Mechanic / Item 表里含「性能/效率/质量」的中文条目打到日志。
    /// </summary>
    internal static class LocDump
    {
        private static readonly string[] Tables = { "Mechanic", "Item" };
        private static readonly string[] Needles = { "性能", "效率", "质量" };

        public static void Run()
        {
            ModLog log = ModEntry.Instance.Log;
            string locale = LocHelper.GetCurrentLocaleCode();
            log.Log($"dump start (locale {locale})");

            int count = 0;
            foreach (string tableName in Tables)
            {
                StringTable table = LocalizationSettings.StringDatabase.GetTable(tableName);
                if (table == null)
                {
                    log.Log($"table {tableName} is null");
                    continue;
                }

                foreach (StringTableEntry entry in table.Values)
                {
                    string value = entry.Value;
                    if (string.IsNullOrEmpty(value) || !ContainsNeedle(value))
                    {
                        continue;
                    }

                    log.Log($"{tableName}.{entry.Key} = {value}");
                    count++;
                }
            }

            log.Log($"dump done, {count} entries");
        }

        private static bool ContainsNeedle(string value)
        {
            foreach (string needle in Needles)
            {
                if (value.IndexOf(needle) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
