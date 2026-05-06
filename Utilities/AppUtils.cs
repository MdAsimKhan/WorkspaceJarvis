using System.Diagnostics;
using System.Text.RegularExpressions;
namespace WorkspaceJarvis.UI.Utilities;

public static class AppUtils
{
    public static string GetFriendlyName(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return "Unknown Item";

        // 1. Handle Web URLs
        if (path.StartsWith("http://") || path.StartsWith("https://") || path.Contains("www."))
        {
            return CleanUrl(path);
        }

        // 2. Handle Files
        try
        {
            if (File.Exists(path))
            {
                var info = FileVersionInfo.GetVersionInfo(path);
                if (!string.IsNullOrWhiteSpace(info.ProductName))
                    return info.ProductName;

                return Path.GetFileNameWithoutExtension(path);
            }

            // Handle folders
            return Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));
        }
        catch
        {
            return "Invalid Path";
        }
    }

    private static string CleanUrl(string url)
    {
        try
        {
            // Remove protocol and www.
            string clean = Regex.Replace(url, @"^(https?://)?(www\.)?", "", RegexOptions.IgnoreCase);

            // Split by '/' to ignore paths (e.g., google.com/search -> google.com)
            clean = clean.Split('/')[0];

            // Remove Top Level Domains (.com, .in, .org, .co.uk, etc.)
            // This Regex removes the last dot and everything after it
            clean = Regex.Replace(clean, @"\.[a-z]{2,}(\.[a-z]{2,})?$", "", RegexOptions.IgnoreCase);

            // Capitalize first letter
            if (clean.Length > 0)
                clean = char.ToUpper(clean[0]) + clean.Substring(1);

            return clean;
        }
        catch
        {
            return url; // Fallback to raw URL if regex fails
        }
    }

    public static string GetIconBase64(string path)
    {
        try
        {
            if (path.StartsWith("http") || path.Contains("www."))
            {
                return $"https://www.google.com/s2/favicons?domain={path}&sz=32";
            }

            if (File.Exists(path))
            {
                // Use the full name System.Drawing.Icon to avoid collision
                using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(path))
                {
                    if (icon == null) return null;

                    using (var ms = new MemoryStream())
                    {
                        // Convert to Bitmap and save as PNG
                        using (var bitmap = icon.ToBitmap())
                        {
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Icon error: {ex.Message}");
        }
        return null;
    }
}