using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public class BundleFile
    {
        [Flags]
        public enum BlockFlags : ushort
        {
            CompressionTypeMask = 0x3f,
            Streamed = 0x40
        }
        [Flags]
        public enum ArchiveFlags : int
        {
            CompressionTypeMask = 0x3f,
            BlocksAndDirectoryInfoCombined = 0x40,
            BlocksInfoAtTheEnd = 0x80,
            OldWebPluginCompatibility = 0x100,
            BlockInfoNeedPaddingAtStart = 0x200
        }
        struct BlockInfo {
            public uint UncompressedSize;
            public uint CompressedSize;
            public BlockFlags Flags;
            public readonly CompressionType CompressionType => (CompressionType)(Flags & BlockFlags.CompressionTypeMask);
        }
        struct Node
        {
            public long offset;
            public long size;
            public uint flags;
            public string path;
        }
        public enum CompressionType
        {
            Auto = -1,
            None,
            Lzma,
            Lz4,
            Lz4HC,
            Lzham
        }
        public struct UnityFile
        {
            public string path;
            public byte[] data;
        }
        public List<UnityFile> files;
        public BundleFile(Stream stream)
        {
            var br = new BinaryReaderBE(stream);
            var sig = Utils.ReadNullTerminatedString(br);
            if (sig != "UnityFS")
            {
                throw new Exception("File not UnityFS?");
            }
            var version = br.ReadInt32();
            var unityVersion = Utils.ReadNullTerminatedString(br);
            var unityRevision = Utils.ReadNullTerminatedString(br);

            var size = br.ReadInt64();
            var compressedBlocksInfoSize = br.ReadInt32();
            var uncompressedBlocksInfoSize = br.ReadInt32();
            var flags = (ArchiveFlags)br.ReadInt32();
            var compressionType = (CompressionType)((int)flags & (int)ArchiveFlags.CompressionTypeMask);
            Utils.AlignStream(stream, 16);
            //if (compressionType == CompressionType.None)
            //{
            //    compressedBlocksInfoSize = uncompressedBlocksInfoSize;
            //}
            var compressedBlockInfoBytes = new byte[compressedBlocksInfoSize];
            if ((flags & ArchiveFlags.BlocksInfoAtTheEnd) != 0)
            {
                var currentPos = stream.Position;
                stream.Seek(size - compressedBlocksInfoSize, SeekOrigin.Begin);
                stream.ReadExactly(compressedBlockInfoBytes);
                stream.Seek(currentPos, SeekOrigin.Begin);
            }
            else
            {
                stream.ReadExactly(compressedBlockInfoBytes);
            }
            var blockInfoBytes = compressedBlockInfoBytes;
            if (compressionType == CompressionType.Lz4 || compressionType == CompressionType.Lz4HC)
            {
                blockInfoBytes = new byte[uncompressedBlocksInfoSize];
                LZ4Codec.Decode(compressedBlockInfoBytes, 0, compressedBlockInfoBytes.Length, blockInfoBytes, 0, blockInfoBytes.Length);
            }
            var blocksInfoUncompressedStream = new MemoryStream(blockInfoBytes);
            var brBlockInfo = new BinaryReaderBE(blocksInfoUncompressedStream);
            var uncompressedDataHash = brBlockInfo.ReadBytes(16);
            var blocksInfoCount = brBlockInfo.ReadInt32();
            var blocks = new List<BlockInfo>();
            for (int i = 0; i < blocksInfoCount; i++)
            {
                blocks.Add(new BlockInfo()
                {
                    UncompressedSize = brBlockInfo.ReadUInt32(),
                    CompressedSize = brBlockInfo.ReadUInt32(),
                    Flags = (BlockFlags)brBlockInfo.ReadUInt16()
                });
            }
            var nodesCount = brBlockInfo.ReadInt32();
            var nodes = new List<Node>();
            for (int i = 0; i < nodesCount; i++)
            {
                nodes.Add(new Node()
                {
                    offset = brBlockInfo.ReadInt64(),
                    size = brBlockInfo.ReadInt64(),
                    flags = brBlockInfo.ReadUInt32(),
                    path = Utils.ReadNullTerminatedString(brBlockInfo)
                });
            }
            if ((flags & ArchiveFlags.BlockInfoNeedPaddingAtStart) != 0)
            {
                Utils.AlignStream(stream, 16);
            }
            MemoryStream combinedBlockStream = new();
            foreach (var block in blocks)
            {
                byte[] compressedBytes = br.ReadBytes((int)block.CompressedSize);
                var uncompressedBytes = compressedBytes;
                if (block.CompressionType == CompressionType.Lz4 || block.CompressionType == CompressionType.Lz4HC)
                {
                    uncompressedBytes = new byte[block.UncompressedSize];
                    LZ4Codec.Decode(compressedBytes, 0, compressedBytes.Length, uncompressedBytes, 0, uncompressedBytes.Length);
                }
                combinedBlockStream.Write(uncompressedBytes);
            }
            files = [];
            foreach (var node in nodes)
            {
                combinedBlockStream.Seek(node.offset, SeekOrigin.Begin);
                byte[] fileBytes = new byte[node.size];
                combinedBlockStream.Read(fileBytes);
                files.Add(new UnityFile()
                {
                    path = node.path,
                    data = fileBytes
                });
            }
            if (files.Count == 0)
            {
                throw new Exception("Failed to read any files?");
            }
        }
    }
}
