using WindowsFirewallHelper;
using WindowsFirewallHelper.FirewallRules;

namespace Netrix.Utils;

public static class Firewall
{
    private const string Netrix = "Netrix";

    /// <summary>
    ///     Add firewall rules for Netrix's bundled programs
    /// </summary>
    public static void AddNetrixFwRules()
    {
        if (!FirewallWAS.IsLocallySupported)
        {
            Log.Warning("Windows Firewall Locally Unsupported");
            return;
        }

        try
        {
            var rule = FirewallManager.Instance.Rules.FirstOrDefault(r => r.Name == Netrix);
            if (rule != null)
            {
                if (rule.ApplicationName.StartsWith(Global.NetrixDir))
                    return;

                RemoveNetrixFwRules();
            }

            foreach (var path in Directory.GetFiles(Global.NetrixDir, "*.exe", SearchOption.AllDirectories))
                AddFwRule(Netrix, path);
        }
        catch (Exception e)
        {
            Log.Warning(e, "Create Netrix Firewall rules error");
        }
    }

    /// <summary>
    ///     Remove the firewall rules (Netrix's bundled programs)
    /// </summary>
    public static void RemoveNetrixFwRules()
    {
        if (!FirewallWAS.IsLocallySupported)
            return;

        try
        {
            foreach (var rule in FirewallManager.Instance.Rules.Where(r
                         => r.ApplicationName?.StartsWith(Global.NetrixDir, StringComparison.OrdinalIgnoreCase) ?? r.Name == Netrix))
                FirewallManager.Instance.Rules.Remove(rule);
        }
        catch (Exception e)
        {
            Log.Warning(e, "Remove Netrix Firewall rules error");
        }
    }

    #region Wrappers

    private static void AddFwRule(string ruleName, string exeFullPath)
    {
        var rule = new FirewallWASRule(ruleName,
            exeFullPath,
            FirewallAction.Allow,
            FirewallDirection.Inbound,
            FirewallProfiles.Private | FirewallProfiles.Public | FirewallProfiles.Domain);

        FirewallManager.Instance.Rules.Add(rule);
    }

    #endregion
}