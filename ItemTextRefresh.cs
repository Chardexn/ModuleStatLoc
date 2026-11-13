using System.Collections.Generic;

namespace ModuleStatLoc
{
    /// <summary>
    /// 读档后刷新已生成物品上的模组名和说明书。这些字段在生成时拷贝，不会再查本地化表。
    /// </summary>
    internal static class ItemTextRefresh
    {
        private static readonly Dictionary<string, string> ModuleNameKeys = new Dictionary<string, string>
        {
            ["system_module_performance"] = "item_system_module_performance_name",
            ["system_module_efficiancy"] = "item_system_module_efficiancy_name",
            ["system_module_quality"] = "item_system_module_quality_name"
        };

        private static readonly string[][] FlavorReplacements =
        {
            new[] { "质量影响适用物品的打印质量", "品质影响适用物品的打印品质" },
            new[] { "质量影响纯度上限", "品质影响纯度上限" },
            new[] { "质量影响吸引力", "品质影响吸引力" },
            new[] { "质量影响纯度", "品质影响纯度" },
            new[] { "质量：每125质量额外陈化1天", "品质：每125品质额外陈化1天" },
            new[] { "质量影响蒸气捕获量", "品质影响蒸气捕获量" },
            new[] { "质量影响产量上限", "品质影响产量上限" },
            new[] { "质量影响成功率", "品质影响成功率" },
            new[] { "质量无影响", "品质无影响" },
            new[] { "性能影响防盗率加成", "速度影响防盗率加成" },
            new[] { "性能影响每晚去除量", "速度影响每晚去除量" },
            new[] { "性能影响每晚蒸发量", "速度影响每晚蒸发量" },
            new[] { "性能影响操作速度", "速度影响操作速度" },
            new[] { "性能影响打印速度", "速度影响打印速度" },
            new[] { "性能影响生长速度", "速度影响生长速度" },
            new[] { "性能影响吸引力", "速度影响吸引力" },
            new[] { "性能影响产量", "速度影响产量" },
            new[] { "性能影响功耗", "速度影响功耗" },
            new[] { "性能影响速度", "速度影响处理速度" },
            new[] { "性能无影响", "速度无影响" },
            new[] { "效率影响功耗", "节能影响功耗" }
        };

        public static void Run()
        {
            string code = LocHelper.GetCurrentLocaleCode();
            if (string.IsNullOrEmpty(code) || !code.StartsWith("zh", System.StringComparison.Ordinal))
            {
                return;
            }

            if (EmporiumEntry.Instance == null)
            {
                return;
            }

            foreach (GameItem item in EmporiumEntry.Instance.GetAllOwnedItems())
            {
                Refresh(item);
            }
        }

        private static void Refresh(GameItem item)
        {
            if (item == null)
            {
                return;
            }

            if (ModuleNameKeys.TryGetValue(item.identifier, out string locKey))
            {
                item.name = LocHelper.GetLocalizedItem(locKey);
            }

            if (!string.IsNullOrEmpty(item.flavorText) && item.flavorText.IndexOf("[模组影响]") >= 0)
            {
                item.flavorText = RewriteFlavor(item.flavorText);
            }
        }

        private static string RewriteFlavor(string text)
        {
            foreach (string[] pair in FlavorReplacements)
            {
                text = text.Replace(pair[0], pair[1]);
            }

            return text;
        }
    }
}
