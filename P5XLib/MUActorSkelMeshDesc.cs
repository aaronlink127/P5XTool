using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{
    public partial class MUActorSkelMeshDesc : UnityAssetLib.Object
    {
        // Fields
        public int Lod;
        public string FileName = "";
        public string RootBoneName = "";
        public string[] BoneNames = [];
        public Material[] Materials = [];

    }
}
