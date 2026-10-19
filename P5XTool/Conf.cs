using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5XTool
{
    public class DialogAnimation
    {
        public const string _ExcelName = "Dialog";
        public const string _FileName = "ConfDialogAnimation";
        [PrimaryKey]
        public string Sn { get; set; } = "";
        public int Type { get; set; }
        public string FrameImage { get; set; } = "";
        public string StartSequence { get; set; } = "";
        public string LoopSequence { get; set; } = "";
        public string EndSequence { get; set; } = "";
        public float Interval { get; set; }
    }

    public class DialogHead2D
    {
        public const string _ExcelName = "DialogHead2D";
        public const string _FileName = "ConfDialogHead2D";
        [PrimaryKey, Column("sn")]
        public string Sn { get; set; } = "";
        [Column("character")]
        public string Character { get; set; } = "";
        [Column("headImage")]
        public string HeadImage { get; set; } = "";
        [Column("blackBorderImage")]
        public string BlackBorderImage { get; set; } = "";
        [Column("headMaskImage")]
        public string HeadMaskImage { get; set; } = "";
        [Column("faceAnimationSn")]
        public int FaceAnimationSn { get; set; }
        [Column("Scale")]
        public float Scale { get; set; }
        [Column("headImageOffset")]
        public string HeadImageOffset { get; set; } = "";
        [Column("headImageOffset19")]
        public string HeadImageOffset19 { get; set; } = "";
        [Column("headImageOffset5")]
        public string HeadImageOffset5 { get; set; } = "";
        [Column("headImageOffset6")]
        public string HeadImageOffset6 { get; set; } = "";
        [Column("headImageOffsetStr4")]
        public string HeadImageOffsetStr4 { get; set; } = "";
        [Column("AVGImage")]
        public string AVGImage { get; set; } = "";
        [Column("AVGImageOffset")]
        public string AVGImageOffset { get; set; } = "";
        [Column("AVGImageShadow")]
        public string AVGImageShadow { get; set; } = "";
        [Column("headImageOffsetStr3")]
        public string HeadImageOffsetStr3 { get; set; } = "";
        [Column("adornment1")]
        public string Adornment1 { get; set; } = "";
        [Column("adornment2")]
        public string Adornment2 { get; set; } = "";
        [Column("adornment3")]
        public string Adornment3 { get; set; } = "";
        [Column("adornment4")]
        public string Adornment4 { get; set; } = "";
        [Column("adornment5")]
        public string Adornment5 { get; set; } = "";
        [Column("adornment6")]
        public string Adornment6 { get; set; } = "";
        [Column("materialParamSn")]
        public string MaterialParamSn { get; set; } = "";
        [Column("eyeFrameAnimation")]
        public string EyeFrameAnimation { get; set; } = "";
        [Column("mouthFrameAnimation")]
        public string MouthFrameAnimation { get; set; } = "";
        [Column("mouthFrameNoTalkAnimation")]
        public string MouthFrameNoTalkAnimation { get; set; } = "";
        [Column("expressionFrameAnimation")]
        public string ExpressionFrameAnimation { get; set; } = "";
        [Column("expressionFrameAnimationTime")]
        public float ExpressionFrameAnimationTime { get; set; }
        [Column("turn")]
        public bool Turn { get; set; }
        [Column("turnMonopoly")]
        public bool TurnMonopoly { get; set; }
        //[Column("isShowFukaEffect")]
        //public bool IsShowFukaEffect { get; set; }
    }

    public class Param
    {
        public const string _ExcelName = "Param";
        public const string _FileName = "ConfParam";
        [PrimaryKey]
        public string Sn { get; set; } = "";
        public string Value { get; set; } = "";
    }
    public class FashionPartModel
    {
        public const string _ExcelName = "Fashion";
        public const string _FileName = "ConfFashionPartModel";

        [PrimaryKey, Column("sn")]
        public int Sn { get; set; }
        [Column("name")]
        public string Name { get; set; } = "";
        [Column("icon")]
        public string Icon { get; set; } = "";
        [Column("subtype")]
        public int Subtype { get; set; }
        [Column("materialColor")]
        public string MaterialColor { get; set; } = "";
        [Column("prefab")]
        public string Prefab { get; set; } = "";
        [Column("hangPoint")]
        public string HangPoint { get; set; } = "";
        [Column("inOutEffect")]
        public int InOutEffect { get; set; }
        [Column("weaponEffect")]
        public string WeaponEffect { get; set; } = "";
        [Column("weaponEffectPoint")]
        public string WeaponEffectPoint { get; set; } = "";
        [Column("isMagicaCloth")]
        public bool IsMagicaCloth { get; set; }
    }
    public class ThiefModelChange
    {
        public const string _ExcelName = "Fashion";
        public const string _FileName = "ConfThiefModelChange";
        [PrimaryKey, Column("sn")]
        public int Sn { get; set; }
        [Column("modelSn")]
        public string ModelSn { get; set; } = "";
        [Column("itemSn")]
        public int ItemSn { get; set; }
        [Column("isShow")]
        public bool IsShow { get; set; }
        [Column("channel")]
        public string Channel { get; set; } = "";
        [Column("order")]
        public int Order { get; set; }
        [Column("thiefSn")]
        public int ThiefSn { get; set; }
        [Column("label")]
        public string Label { get; set; } = "";
        [Column("bgPic")]
        public string BgPic { get; set; } = "";
        [Column("color")]
        public string Color { get; set; } = "";
        [Column("teamPic")]
        public string TeamPic { get; set; } = "";
        [Column("thiefPic")]
        public string ThiefPic { get; set; } = "";
        [Column("effectClose")]
        public string EffectClose { get; set; } = "";
        [Column("light")]
        public string Light { get; set; } = "";
        [Column("CharacterPic")]
        public string CharacterPic { get; set; } = "";
        [Column("CharacterPicPosition")]
        public string CharacterPicPosition { get; set; } = "";
        [Column("CharacterPicScale")]
        public float CharacterPicScale { get; set; }
        [Column("actionTrack")]
        public string ActionTrack { get; set; } = "";
        [Column("showSound")]
        public string ShowSound { get; set; } = "";
        [Column("thiefback_sound")]
        public string Thiefback_sound { get; set; } = "";
        [Column("thiefbattle_sound")]
        public string Thiefbattle_sound { get; set; } = "";
        [Column("defaultFashion")]
        public bool DefaultFashion { get; set; }
        [Column("previewTimeline")]
        public string PreviewTimeline { get; set; } = "";
        [Column("teamPicS")]
        public string TeamPicS { get; set; } = "";
        [Column("flashCardEffectTeamCard")]
        public string FlashCardEffectTeamCard { get; set; } = "";
        [Column("flashCardEffectTeamHead")]
        public string FlashCardEffectTeamHead { get; set; } = "";
        [Column("CharacterCardPic")]
        public string CharacterCardPic { get; set; } = "";
        [Column("thiefBigPicPos")]
        public string ThiefBigPicPos { get; set; } = "";
        [Column("thiefBigPicScal")]
        public float ThiefBigPicScal { get; set; }
        [Column("teamPersonaPic")]
        public string TeamPersonaPic { get; set; } = "";
        [Column("DispatchOne")]
        public string DispatchOne { get; set; } = "";
        [Column("DispatchBegin")]
        public string DispatchBegin { get; set; } = "";
        [Column("DispatchBack")]
        public string DispatchBack { get; set; } = "";
        [Column("DispatchAward")]
        public string DispatchAward { get; set; } = "";
    }
}
