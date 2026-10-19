using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XLib
{

    // Namespace: MUEngine
    public enum EFacePart // TypeDefIndex: 5205
    {
        NotFacePart = -1,
        Eyeball = 1,
        SpEyeball = 2,
        Eyelid = 3,
        Beard = 4,
        Tear = 5,
        Cheek = 6
    }
    public partial class MUActorMeshExportInfo : MonoBehaviour
    {
        // Fields
        public bool IsFaceMesh;
        [AutoOptional]
        public EFacePart FacePart;
        public string mHighRootBoneName = "";
        public string mLodRootBoneName = "";
        [AutoOptional]
        public string mShadowRootBoneName = "";
        public string[] mHighMeshBoneNames = [];
        public string[] mLodMeshBoneNames = [];
        [AutoOptional]
        public string[] mShadowMeshBoneNames = [];
        public List<PPtr<Material>> mHighMeshMaterials = [];
        public PPtr<Mesh> mMesh = new();
        public PPtr<Mesh> mLODMesh = new();
        [AutoOptional]
        public PPtr<Mesh> mShadowMesh = new();
        public string mHighMeshName = "";
        public string mLODMeshName = "";
        [AutoOptional]
        public string mShadowMeshName = "";
        public List<PPtr<Material>> mLodMaterials = [];
        [AutoOptional]
        public List<PPtr<Material>> mShadowMaterials = [];
        public bool mIsSkinnedMeshRender;
        public string mMakeFaceTemplateName = "";
        public string mMakeFaceTexTemplateName = "";
        public List<PPtr<Material>> mFaceMaterials = [];
    }
}
