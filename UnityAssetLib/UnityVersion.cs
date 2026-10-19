using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public class UnityVersion : IComparable, IEquatable<UnityVersion>
    {
        public int Major;
        public int Minor;
        public int Patch;
        public string Build;

        // ex: "2021.3.5f1" "2020.3.41f1c1"
        public UnityVersion(string version)
        {
            var mainParts = version.Split(['.'], 3);
            Major = int.Parse(mainParts[0]);
            Minor = int.Parse(mainParts[1]);
            var patchAndBuild = mainParts[2];
            var patchStr = new string([.. patchAndBuild.TakeWhile(char.IsDigit)]);
            Patch = int.Parse(patchStr);
            Build = patchAndBuild[patchStr.Length..];
        }
        public UnityVersion(int major)
        {
            Major = major;
            Minor = 0;
            Patch = 0;
            Build = "";
        }
        public UnityVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Build = "";
        }
        public UnityVersion(int major, int minor, int patch, string build)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Build = build;
        }
        public int CompareTo(UnityVersion obj)
        {
            var res = Major.CompareTo(obj.Major);
            if (res != 0) return res;
            res = Minor.CompareTo(obj.Minor);
            if (res != 0) return res;
            res = Patch.CompareTo(obj.Patch);
            if (res != 0) return res;
            return 0;
        }
        public int CompareTo(object? obj)
        {
            if (obj is UnityVersion uv)
            {
                return CompareTo(uv);
            }
            throw new ArgumentException("Object is not a UnityVersion");
        }
        public bool Equals(UnityVersion? other)
        {
            if (other == null) return false;
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }
        public override bool Equals(object? o)
        {
            if (o == null)
                return false;
            if (ReferenceEquals(this, o))
                return true;
            if (o is UnityVersion uv)
                return Equals(uv);
            return false;
        }
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}{Build}";
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch);
        }
        public static bool operator>(UnityVersion a, UnityVersion b) => a.CompareTo(b) > 0;
        public static bool operator<(UnityVersion a, UnityVersion b) => a.CompareTo(b) < 0;
        public static bool operator >=(UnityVersion a, UnityVersion b) => a.CompareTo(b) >= 0;
        public static bool operator <=(UnityVersion a, UnityVersion b) => a.CompareTo(b) <= 0;
    }
}
