using SALT.Utils;
using System;

namespace SALT
{
    public class ModInfo
    {
        public ModInfo(
          string modid,
          string name,
          string author,
          ModInfo.ModVersion version,
          string description)
        {
            this.Id = modid;
            this.Name = name;
            this.Author = author;
            this.Version = version;
            this.Description = description;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Author { get; private set; }

        public string Description { get; private set; }

        public ModInfo.ModVersion Version { get; private set; }

        public static ModInfo GetCurrentInfo() => ModLoader.GetModForAssembly(ReflectionUtils.GetRelevantAssembly()).ModInfo;

        public struct ModVersion : IComparable<ModInfo.ModVersion>
        {
            public int Major;
            public int Minor;
            public int Revision;
            public static readonly ModInfo.ModVersion DEFAULT = new ModInfo.ModVersion(1, 0);

            public ModVersion(int major, int minor, int revision = 0)
            {
                this.Major = major;
                this.Minor = minor;
                this.Revision = revision;
            }

            public override string ToString() => string.Format("{0}.{1}.{2}", (object)this.Major, (object)this.Minor, (object)this.Revision);

            public static ModInfo.ModVersion Parse(string s)
            {
                string[] strArray = s.Split('.');
                int result1;
                int result2;
                if (strArray.Length >= 2 && strArray.Length <= 3 && (int.TryParse(strArray[0], out result1) && int.TryParse(strArray[1], out result2)))
                {
                    int result3 = 0;
                    if (strArray.Length != 3 || int.TryParse(strArray[2], out result3))
                        return new ModInfo.ModVersion(result1, result2, result3);
                }
                throw new Exception("Invalid Version String: " + s);
            }

            public int CompareTo(ModInfo.ModVersion other)
            {
                if (this.Major > other.Major)
                    return -1;
                if (this.Major < other.Major)
                    return 1;
                if (this.Minor > other.Minor)
                    return -1;
                if (this.Minor < other.Minor)
                    return 1;
                if (this.Revision > other.Revision)
                    return -1;
                return this.Revision < other.Revision ? 1 : 0;
            }
        }
    }
}
