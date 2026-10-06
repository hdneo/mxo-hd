using System;
using System.IO;

namespace hds.Core
{
    /// <summary>
    /// Resolves runtime folders. The game data lives in the repository (hds/data) and is not copied
    /// into the build output; it is searched next to the binary first and then upwards from it.
    /// </summary>
    public static class ServerPaths
    {
        private static string dataDirectory;

        public static string DataDirectory
        {
            get
            {
                if (dataDirectory != null) return dataDirectory;
                dataDirectory = Locate("data", "goschema.txt")
                                ?? throw new DirectoryNotFoundException("Could not find the data folder (expected hds/data with goschema.txt)");
                return dataDirectory;
            }
            set => dataDirectory = value;
        }

        /// <summary>Path of a file inside the data folder, e.g. Data("missions") or Data("abilityIDs.csv").</summary>
        public static string Data(params string[] parts) => Path.Combine(DataDirectory, Path.Combine(parts));

        private static string Locate(string folder, string markerFile)
        {
            foreach (var start in new[] {Directory.GetCurrentDirectory(), AppContext.BaseDirectory})
            {
                var dir = new DirectoryInfo(start);
                while (dir != null)
                {
                    var candidate = Path.Combine(dir.FullName, folder);
                    if (File.Exists(Path.Combine(candidate, markerFile))) return candidate;
                    dir = dir.Parent;
                }
            }
            return null;
        }
    }
}
