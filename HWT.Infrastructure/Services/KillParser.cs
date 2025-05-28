using System.Text.RegularExpressions;
using HWT.Domain.Entities;
using Serilog;

namespace HWT.Infrastructure.Services;

/// <summary>
/// Parses kill messages from the game log to extract relevant information.
/// This class provides methods to identify the attacker, target, weapon used,
/// and the type of kill (FPS, Air, etc.).
/// </summary>
public static class KillParser
{
    private static readonly ILogger _log = Log.ForContext(typeof(KillParser));

    private static bool IsRealPlayer(string name)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               !name.Contains("NPC", StringComparison.OrdinalIgnoreCase) &&
               !name.Contains("Enemy", StringComparison.OrdinalIgnoreCase) &&
               !name.Contains("_");
    }

    private static string ClassifyKill(string weaponClass)
    {
        var wc = weaponClass.ToLowerInvariant();
        if (wc.Contains("rifle") || wc.Contains("pistol") || wc.Contains("sniper") ||
            wc.Contains("laser") || wc.Contains("ballistic") || wc.Contains("knife"))
            return "FPS";

        if (wc.Contains("turret") || wc.Contains("gatling") || wc.Contains("gimbal") ||
            wc.Contains("missile") || wc.Contains("ship") || wc.Contains("cannon"))
            return "Air";

        return "Unknown";
    }

    public static KillEntry? ExtractKill(string line)
    {
        if (!line.Contains("<Actor Death>"))
            return null;

        try
        {
            var attacker = Regex.Match(line, @"killed by\s+'([^']+)'").Groups[1].Value.Trim();
            var target = Regex.Match(line, @"CActor::Kill:\s+'([^']+)'").Groups[1].Value.Trim();
            var weaponMatch = Regex.Match(line, @"using\s+'([^']+)'\s+\[Class\s+([^\]]+)\]");
            var weaponClass = weaponMatch.Success ? weaponMatch.Groups[2].Value.Trim() : "Unknown";
            var timestamp = Regex.Match(line, @"<(\d{4}-\d{2}-\d{2}T[^>]+)>").Groups[1].Value;

            if (string.IsNullOrEmpty(attacker) || string.IsNullOrEmpty(target))
            {
                _log.Debug("Skipping line due to missing attacker or target: {Line}", line);
                return null;
            }

            if (!IsRealPlayer(target))
            {
                _log.Debug("Skipping non-player target: {Target}", target);
                return null;
            }

            var type = ClassifyKill(weaponClass);

            return new KillEntry
            {
                Timestamp = !string.IsNullOrWhiteSpace(timestamp) ? timestamp : DateTime.Now.ToString("s"),
                Attacker = attacker,
                Target = target,
                Weapon = weaponClass,
                Type = type,
                Summary = $"[{timestamp}] {type} – {attacker} → {target} ({weaponClass})"
            };
        }
        catch (Exception ex)
        {
            _log.Warning(ex, "Failed to parse kill line: {Line}", line);
            return null;
        }
    }
}
