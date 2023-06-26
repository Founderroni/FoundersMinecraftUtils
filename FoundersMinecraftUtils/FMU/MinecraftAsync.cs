using System;
using System.IO;
using System.Threading.Tasks;

namespace FMU
{
    public static class MinecraftAsync
    {
        #region Variables
        public static readonly string DefaultMcpeDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftpe\");
        public static readonly string OptionsFile = Path.Combine(DefaultMcpeDirectory, "options.txt");
        #endregion

        #region Getting Values
        /// <summary>
        /// Returns the value of an option from the options.txt file
        /// </summary>
        public static async Task<string> GetOptionValue(string key)
        {
            if (!File.Exists(OptionsFile))
                return "N/A";

            using (var sr = new StreamReader(OptionsFile))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    if (line.StartsWith(key))
                    {
                        string[] parts = line.Split(':');
                        return parts.Length > 1 ? parts[1] : "N/A";
                    }
                }
            }

            return "N/A";
        }

        /// <summary>
        /// Returns the entire line from a file in the minecraftpe folder
        /// </summary>
        public static async Task<string> GetValueFromFile(string fileName, int line = 0)
        {
            string filePath = Path.Combine(DefaultMcpeDirectory, fileName);

            if (!File.Exists(filePath))
                return "N/A";

            using (var sr = new StreamReader(filePath))
            {
                string result;
                int count = 0;
                do
                {
                    result = await sr.ReadLineAsync();
                    count++;
                } while (result != null && count <= line);

                return result != null ? result : "N/A";
            }
        }

        /// <summary>
        /// Returns the player's (not signed into xbox live) username
        /// </summary>
        public static Task<string> GetMPUsername() => GetOptionValue("mp_username");

        /// <summary>
        /// Returns the player's language setting
        /// </summary>
        public static Task<string> GetLanguage() => GetOptionValue("game_language");

        /// <summary>
        /// Returns the player's ClientID
        /// </summary>
        public static Task<string> GetCID() => GetValueFromFile("clientId.txt");

        /// <summary>
        /// Returns the player's DeviceID
        /// </summary>
        public static Task<string> GetDID() => GetValueFromFile("hs", 1);
        #endregion

        #region Settings Values
        /// <summary>
        /// Sets the value of an option from the options.txt file
        /// </summary>
        public static async Task<bool> SetOptionValue(string key, string value)
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

                using (var sw = new StreamWriter(OptionsFile))
                {
                    foreach (var line in lines)
                    {
                        await sw.WriteLineAsync(line);
                    }
                }

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
        public static async Task<bool> UnlockFPS()
        {
            if (!File.Exists(OptionsFile))
                return false;

            try
            {
                bool success = true;

                success &= await SetOptionValue("gfx_multithreaded_renderer", "1");
                success &= await SetOptionValue("gfx_vsync", "0");
                success &= await SetOptionValue("gfx_max_framerate", "0");

                return success;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
