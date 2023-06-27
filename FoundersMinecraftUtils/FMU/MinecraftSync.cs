using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FMU
{
    public static class MinecraftSync
    {
        #region Variables
        public static readonly string MinecraftPackageDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe");
        public static string DefaultMcpeDirectory = Path.Combine(MinecraftPackageDirectory, @"LocalState\games\com.mojang\minecraftpe\");
        public static readonly string OptionsFile = Path.Combine(DefaultMcpeDirectory, "options.txt");
        #endregion

        #region Getting Values
        /// <summary>
        /// Returns the value of an option from the options.txt file
        /// </summary>
        public static string GetOptionValue(string key)
        {
            if (!File.Exists(OptionsFile))
                return "N/A";

            string[] lines = File.ReadAllLines(OptionsFile);
            foreach (string line in lines)
            {
                if (line.StartsWith(key))
                {
                    string[] parts = line.Split(':');
                    return parts.Length > 1 ? parts[1] : "N/A";
                }
            }

            return "N/A";
        }

        /// <summary>
        /// Returns the entire line from a file in the minecraftpe folder
        /// </summary>
        public static string GetValueFromFile(string fileName, int line = 0)
        {
            string filePath = Path.Combine(DefaultMcpeDirectory, fileName);

            if (!File.Exists(filePath))
                return "N/A";

            string[] lines = File.ReadAllLines(filePath);
            return lines.Length > line ? lines[line] : "N/A";
        }

        /// <summary>
        /// Returns the player's (not signed into xbox live) username
        /// </summary>
        public static string GetMPUsername() => GetOptionValue("mp_username");

        /// <summary>
        /// Returns the player's language setting
        /// </summary>
        public static string GetLanguage() => GetOptionValue("game_language");

        /// <summary>
        /// Returns the player's ClientID
        /// </summary>
        public static string GetCID() => GetValueFromFile("clientId.txt");

        /// <summary>
        /// Returns the player's DeviceID
        /// </summary>
        public static string GetDID() => GetValueFromFile("hs", 1);
        #endregion

        #region Settings Values
        /// <summary>
        /// Sets the value of an option from the options.txt file
        /// </summary>
        public static bool SetOptionValue(string key, string value)
        {
            try
            {
                if (!File.Exists(OptionsFile))
                    return false;

                string[] lines = File.ReadAllLines(OptionsFile);
                bool keyFound = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(key))
                    {
                        lines[i] = $"{key}:{value}";
                        keyFound = true;
                        break;
                    }
                }

                if (!keyFound)
                {
                    Array.Resize(ref lines, lines.Length + 1);
                    lines[lines.Length - 1] = $"{key}:{value}";
                }

                File.WriteAllLines(OptionsFile, lines);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Changes some game settings (like disabling vsync) to unlock the player's FPS
        /// </summary>
        public static bool UnlockFPS()
        {
            if (!File.Exists(OptionsFile))
                return false;

            try
            {
                bool success = true;

                success &= SetOptionValue("gfx_multithreaded_renderer", "1");
                success &= SetOptionValue("gfx_vsync", "0");
                success &= SetOptionValue("gfx_max_framerate", "0");

                return success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Clears cached files to clear up storage space
        /// </summary>
        public static bool ClearCache(bool skinCache = true, bool deleteFolders = true)
        {
            if (!Directory.Exists(MinecraftPackageDirectory))
                return false;

            string localState = Path.Combine(MinecraftPackageDirectory, "LocalState");

            try
            {
                string premiumCachePath = Path.Combine(localState, "premium_cache");

                if (skinCache)
                {
                    if (deleteFolders)
                    {
                        if (Directory.Exists(premiumCachePath))
                            Directory.Delete(premiumCachePath, true);
                    }
                    else
                    {
                        if (Directory.Exists(premiumCachePath))
                            foreach (string file in Directory.EnumerateFiles(premiumCachePath, "*.*", SearchOption.AllDirectories))
                                File.Delete(file);
                    }

                    if (Directory.Exists(localState))
                        foreach (string file in Directory.EnumerateFiles(localState, "*.png"))
                            File.Delete(file);
                }

                if (Directory.Exists(localState))
                    foreach (string file in Directory.EnumerateFiles(localState, "*.*").Where(f => f.EndsWith(".ent") || f.EndsWith(".cache")))
                        File.Delete(file);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
