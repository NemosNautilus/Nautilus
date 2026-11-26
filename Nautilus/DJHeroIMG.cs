using Nautilus.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using static Nautilus.FSARFile;

namespace Nautilus
{
    public class DJHeroIMGTools
    {
        private byte[] SwapBytes(byte[] textureData)
        {
            var swappedData = new byte[textureData.Length];
            for (int i = 0; i < textureData.Length; i += 4)
            {
                if (i + 4 <= textureData.Length)
                {
                    swappedData[i + 0] = textureData[i + 1];
                    swappedData[i + 1] = textureData[i + 0];
                    swappedData[i + 2] = textureData[i + 3];
                    swappedData[i + 3] = textureData[i + 2];
                }
            }
            return swappedData;
        }

        public bool ImgToDDS(string img)
        {
            string tempFile = Path.GetDirectoryName(img) + "\\temp.dds";
            string finalFile = img.Replace(".img", ".dds");

            // Read the .img file and extract the texture data (skipping the 20-byte header)
            byte[] imgData = File.ReadAllBytes(img);
            byte[] textureData = imgData.Skip(20).ToArray();

            // Apply byte-swapping
            textureData = SwapBytes(textureData);

            // Flip texture data vertically
            int width = 512;  // Extracted from header
            int height = 512; // Extracted from header                        

            // Generate DDS header
            byte[] ddsHeader = GenerateDDSHeader(width, height, 1);

            // Combine the header and processed texture data
            byte[] ddsData = new byte[ddsHeader.Length + textureData.Length];
            Array.Copy(ddsHeader, 0, ddsData, 0, ddsHeader.Length);
            Array.Copy(textureData, 0, ddsData, ddsHeader.Length, textureData.Length);

            // Save the DDS file
            File.WriteAllBytes(tempFile, ddsData);

            var arg = " -i \"" + tempFile + "\" -o \"" + finalFile + "\" -f BC1 -flip y";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = Application.StartupPath + "\\bin\\PVRTexToolCLI.exe",
                Arguments = arg,
                WorkingDirectory = Application.StartupPath + "\\bin\\"
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();

            return File.Exists(finalFile);
        }

        private byte[] GenerateDDSHeader(int width, int height, int mipmapCount)
        {
            // Create the DDS header
            byte[] header = new byte[128];

            // Magic number
            Array.Copy(new byte[] { 0x44, 0x44, 0x53, 0x20 }, 0, header, 0, 4);

            // Header size
            BitConverter.GetBytes(124).CopyTo(header, 4);

            // Flags
            BitConverter.GetBytes(0x00021007).CopyTo(header, 8);

            // Dimensions
            BitConverter.GetBytes(height).CopyTo(header, 12); // Height
            BitConverter.GetBytes(width).CopyTo(header, 16);  // Width

            // Linear size (pitch)
            int linearSize = (width / 4) * (height / 4) * 8; // Total size for BC1
            BitConverter.GetBytes(linearSize).CopyTo(header, 20);

            // Depth (unused)
            BitConverter.GetBytes(0).CopyTo(header, 24);

            // Mipmaps
            BitConverter.GetBytes(mipmapCount).CopyTo(header, 28);

            // Reserved fields (44 bytes)
            // Already zero-initialized

            // Pixel format
            BitConverter.GetBytes(32).CopyTo(header, 76);  // Size of pixel format
            BitConverter.GetBytes(0x00000004).CopyTo(header, 80);  // Flags (compressed)
            Array.Copy(new byte[] { 0x44, 0x58, 0x54, 0x31 }, 0, header, 84, 4);  // FourCC ('DXT1')

            // Caps
            BitConverter.GetBytes(0x00100008).CopyTo(header, 108);  // Caps1 (texture + mipmaps)

            // Explicitly set bytes 6D (109) and 6E (110)
            header[109] = 0x10;
            header[110] = 0x00;

            // Remaining fields are zero-initialized
            return header;
        }




    }

    //code by Maxton, RIP
    public class FSARPackage : AbstractPackage
    {
        const uint kFSAR = 0x46534152;
        const uint kFSGC = 0x46534743;
        public static PackageTestResult IsFSAR(IFile f)
        {
            using (Stream s = f.GetStream())
            {
                s.Position = 0;
                switch (s.ReadUInt32BE())
                {
                    case kFSAR:
                        return PackageTestResult.YES;
                    case kFSGC:
                        return PackageTestResult.MAYBE;
                    default:
                        return PackageTestResult.NO;
                }
            }
        }

        public static FSARPackage FromFile(IFile f)
        {
            return new FSARPackage(f);
        }

        public override string FileName { get; }

        public override IDirectory RootDirectory => root;
        public override long Size => filestream.Length;

        public override bool Writeable => false;

        public override Type FileType => typeof(FSARFile);

        private Stream filestream;
        private FSARDirectory root;
        private static byte[] fsgcKey = new byte[]
        {
      0x47, 0x3F, 0x2A, 0xD8, 0xCA, 0x3B, 0xBC, 0xF7,
      0xAD, 0x71, 0x5D, 0xE7, 0x90, 0x96, 0x2E, 0xFE
        };
        private static byte[] fsgcCtr = new byte[]
        {
      0xE0, 0xAC, 0x52, 0x9C, 0x1B, 0x97, 0x3B, 0x27,
      0x65, 0x89, 0x78, 0x46, 0x30, 0x82, 0x58, 0x3E,
        };

        /// <summary>
        /// Open the .far archive which is the given file.
        /// </summary>
        /// <param name="path"></param>
        public FSARPackage(IFile f)
        {
            FileName = f.Name;
            root = new FSARDirectory(null, ROOT_DIR);
            filestream = f.GetStream();
            uint magic = filestream.ReadUInt32BE();
            if (magic == kFSGC) // "FSGC"
            {
                filestream = new AesCtrStream(filestream, fsgcKey, fsgcCtr, 8, true);
                magic = filestream.ReadUInt32BE();
            }
            if (magic != kFSAR) // "FSAR"
            {
                throw new InvalidDataException("File does not have a valid FSAR header.");
            }
            filestream.Position = 8;
            uint file_base = filestream.ReadUInt32BE();
            uint num_files = filestream.ReadUInt32BE();
            filestream.Position = 0x20;
            for (int i = 0; i < num_files; i++)
            {
                filestream.Position = 0x20 + 0x120 * i;
                string fpath = filestream.ReadASCIINullTerminated();
                filestream.Position = 0x20 + 0x120 * i + 0x100;
                long size = filestream.ReadInt64BE();
                long zsize = filestream.ReadInt64BE();
                long offset = filestream.ReadInt64BE();
                uint zipped = filestream.ReadUInt32BE();
                FSARDirectory dir = makeOrGetDir(fpath);
                string filename = fpath.Split('\\').Last();
                dir.AddFile(new FSARFile(filename, dir, size, zipped != 1, zsize, offset + file_base, filestream));
            }
        }

        /// <summary>
        /// Get the directory at the end of this path, or make it (and all
        /// intermediate dirs) if it doesn't exist.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private FSARDirectory makeOrGetDir(string path)
        {
            string[] breadcrumbs = path.Split('\\');
            IDirectory last = root;
            IDirectory current;
            if (breadcrumbs.Length == 1)
            {
                return root;
            }

            for (var idx = 0; idx < breadcrumbs.Length - 1; idx++)
            {
                if (!last.TryGetDirectory(breadcrumbs[idx], out current))
                {
                    current = new FSARDirectory(last, breadcrumbs[idx]);
                    (last as FSARDirectory).AddDir(current as FSARDirectory);
                }
                last = current;
            }
            return last as FSARDirectory;
        }

        public override void Dispose()
        {
            filestream.Close();
            filestream.Dispose();
        }
    }

    public abstract class AbstractPackage : IDisposable
    {
        /// <summary>
        /// The name of this package.
        /// </summary>
        public abstract string FileName { get; }

        /// <summary>
        /// The root directory of this filesystem.
        /// </summary>
        public abstract IDirectory RootDirectory { get; }

        /// <summary>
        /// The size of this package's data files. For packages with unified header and data,
        /// this is just the size of the package file.
        /// </summary>
        public abstract long Size { get; }

        /// <summary>
        /// Indicates whether this package can be modified.
        /// </summary>
        public abstract bool Writeable { get; }

        /// <summary>
        /// Implementation of the IDisposable interface.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Separates elements in a file path.
        /// </summary>
        public const char PATH_SEPARATOR = '/';

        /// <summary>
        /// The name of the root directory. Never used in paths, though.
        /// </summary>
        public const string ROOT_DIR = "/";

        /// <summary>
        /// The .NET type of the file objects in this package.
        /// </summary>
        public virtual Type FileType => typeof(IFile);

        /// <summary>
        /// Get the file at the given path. Path separator is '/'.
        /// Files in the root directory have no path separator.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The file at the given path.</returns>
        public IFile GetFile(string path) => RootDirectory.GetFileAtPath(path);

        /// <summary>
        /// Returns a list containing all the logical files of the specified type in this archive.
        /// </summary>
        /// <returns></returns>
        public virtual List<F> GetAllFiles<F>() where F : class, IFile
        {
            var files = new List<F>();
            Action<IDirectory> readDirectory = null;
            readDirectory = (d) =>
            {
                foreach (var f in d.Files) if (f as F != null) files.Add(f as F);
                foreach (var dir in d.Dirs) readDirectory(dir);
            };

            readDirectory(RootDirectory);
            return files;
        }

        /// <summary>
        /// Returns a list containing all the logical files in this archive.
        /// </summary>
        /// <returns></returns>
        public List<IFile> GetAllFiles()
        {
            return GetAllFiles<IFile>();
        }
    }

    public enum PackageTestResult
    {
        /// <summary>
        /// Definitely not an instance of the package type.
        /// </summary>
        NO,
        /// <summary>
        /// Possibly an instance of the package type, but a more in-depth analysis would be needed.
        /// </summary>
        MAYBE,
        /// <summary>
        /// Definitely an instance of the package type.
        /// </summary>
        YES
    };

    public interface IFile : IFSNode
    {
        /// <summary>
        /// The size of this file.
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Indicates whether this file is compressed in the archive.
        /// </summary>
        bool Compressed { get; }

        /// <summary>
        /// The size of this file, as it is in the archive.
        /// </summary>
        long CompressedSize { get; }

        /// <summary>
        /// A collection of extended information about the file. The values in the collection
        /// depend on the type of package the file is from. Modifying this dictionary results in
        /// undefined behavior.
        /// </summary>
        IDictionary<string, object> ExtendedInfo { get; }

        /// <summary>
        /// Get a byte-array in memory containing all the data of this file.
        /// </summary>
        /// <returns></returns>
        byte[] GetBytes();

        ///<summary>
        /// Gets a stream that allows access to this file.
        ///</summary>
        Stream Stream { get; }

        /// <summary>
        /// Get a stream (either memory-backed or disk-based) that allows access to this file.
        /// </summary>
        /// <returns></returns>
        Stream GetStream();
    }

    /// <summary>
    /// Represents a directory within some file system.
    /// </summary>
    public interface IDirectory : IFSNode
    {
        /// <summary>
        /// Tries to get the named file. If it is not found, returns false.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        bool TryGetFile(string name, out IFile file);

        /// <summary>
        /// Get the file in this directory with the given name. Throws exception if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Thrown when the file could not be found.</exception>  
        IFile GetFile(string name);

        /// <summary>
        /// Tries to get the named directory. If it is not found, returns false.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        bool TryGetDirectory(string name, out IDirectory dir);

        /// <summary>
        /// Get the directory in this directory with the given name. Throws exception if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when the directory could not be found.</exception>
        IDirectory GetDirectory(string name);

        /// <summary>
        /// Tries to get the file at the given path, which is relative to this directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException">Thrown when a directory in the path could not be found.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file could not be found.</exception>
        IFile GetFileAtPath(string path);

        /// <summary>
        /// A collection of all files in this directory.
        /// </summary>
        ICollection<IFile> Files { get; }

        /// <summary>
        /// A collection of all the directories in this directory.
        /// </summary>
        ICollection<IDirectory> Dirs { get; }
    }

    public class FSARFile : IFile
    {
        public string Name { get; }
        public IDirectory Parent { get; }
        public long Size { get; }
        public bool Compressed { get; }
        public long CompressedSize { get; }
        public IDictionary<string, object> ExtendedInfo { get; }
        public Stream Stream => GetStream();

        private long offset;
        private Stream archive;

        public FSARFile(string n, IDirectory p, long size, bool compressed,
                        long zsize, long offset, Stream archive)
        {
            Name = n;
            Parent = p;
            Size = size;
            Compressed = compressed;
            CompressedSize = zsize;
            this.offset = offset;
            this.archive = archive;
            ExtendedInfo = new Dictionary<string, object>();
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Size];
            if (Size > Int32.MaxValue)
            {
                throw new NotSupportedException("Can't read bytes for file larger than int32 max, yet.");
            }
            using (var stream = this.GetStream())
            {
                stream.Read(bytes, 0, (int)Size);
            }
            return bytes;
        }

        public Stream GetStream()
        {
            if (!Compressed)
                return new OffsetStream(archive, offset, Size);
            else
                return new DeflateStream(new OffsetStream(archive, offset + 2, CompressedSize - 2),
                                         CompressionMode.Decompress);
        }

        public class DefaultDirectory : IDirectory
        {
            protected SortedDictionary<string, IFile> files;
            protected SortedDictionary<string, IDirectory> dirs;

            public string Name { get; }

            public IDirectory Parent { get; }

            public virtual ICollection<IDirectory> Dirs => dirs.Values;
            public virtual ICollection<IFile> Files => files.Values;

            public virtual bool TryGetDirectory(string name, out IDirectory dir)
            {
                return dirs.TryGetValue(name.ToLower(), out dir);
            }

            public IDirectory GetDirectory(string name)
            {
                IDirectory ret;
                if (TryGetDirectory(name, out ret))
                    return ret;
                throw new System.IO.DirectoryNotFoundException("Unable to find the directory " + name);
            }

            public virtual bool TryGetFile(string name, out IFile file)
            {
                return files.TryGetValue(name.ToLower(), out file);
            }

            public IFile GetFile(string name)
            {
                IFile ret;
                if (TryGetFile(name, out ret))
                    return ret;
                throw new System.IO.FileNotFoundException("Unable to find the file " + name);
            }

            public IFile GetFileAtPath(string path)
            {
                if (path[0] == AbstractPackage.PATH_SEPARATOR)
                    path = path.Substring(1);
                string[] breadcrumbs = path.Split('/');
                if (breadcrumbs.Length == 1)
                {
                    return GetFile(breadcrumbs[0]);
                }
                string newPath = string.Join(AbstractPackage.PATH_SEPARATOR.ToString(), breadcrumbs, 1, breadcrumbs.Length - 1);
                return GetDirectory(breadcrumbs[0]).GetFileAtPath(newPath);
            }

            internal void AddFile(IFile f)
            {
                if (!files.ContainsKey(f.Name.ToLower()))
                {
                    files.Add(f.Name.ToLower(), f);
                }
            }

            internal void AddDir(IDirectory d)
            {
                if (!dirs.ContainsKey(d.Name.ToLower()))
                {
                    dirs.Add(d.Name.ToLower(), d);
                }
            }

            internal void GetAllNodes()
            {

            }

            internal DefaultDirectory(IDirectory parent, string name)
            {
                Parent = parent;
                Name = name;
                files = new SortedDictionary<string, IFile>();
                dirs = new SortedDictionary<string, IDirectory>();
            }
        }
    }

    public interface IFSNode
    {
        /// <summary>
        /// The name of this node.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The folder where this node resides.
        /// For the root directory, this is null.
        /// </summary>
        IDirectory Parent { get; }
    }

    /// <summary>
    /// Represents a single file in a filesystem.
    /// </summary>

    public class FSARDirectory : DefaultDirectory
    {
        public FSARDirectory(IDirectory parent, string name) : base(parent, name)
        {

        }
    }

    public class AesCtrStream : Stream
    {
        Stream s;
        long offset;
        long position;
        AesManaged aes;
        byte[] initialIv;
        byte[] counter;
        byte[] cryptedCounter = new byte[16];
        bool closeStream;
        public AesCtrStream(Stream input, byte[] key, byte[] iv, long offset = 0, bool shouldClose = false)
        {
            s = input;
            initialIv = (byte[])iv.Clone();
            counter = (byte[])iv.Clone();
            this.offset = offset;
            closeStream = shouldClose;
            aes = new AesManaged()
            {
                Mode = CipherMode.ECB,
                BlockSize = 128,
                KeySize = 128,
                Padding = PaddingMode.None,
                Key = key,
            };
        }

        private void resetCounter()
        {
            // TODO: optimize this
            Buffer.BlockCopy(initialIv, 0, counter, 0, 16);
            var block = position / 16;
            counterBlock = 0;
            for (long i = 0; i < block; i++)
            {
                IncrementCounter();
            }
        }

        private long counterBlock = 0;
        private void IncrementCounter()
        {
            counterBlock++;
            for (int j = 0; j < 16; j++)
            {
                counter[j]++;
                if (counter[j] != 0)
                    break;
            }
        }
        public override void Close()
        {
            if (closeStream)
                s.Close();
            base.Close();
        }
        public override bool CanRead => position < Length;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => s.Length - offset;

        public override long Position { get => position; set { position = value; resetCounter(); } }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int bufOffset, int count)
        {
            if (position + count > Length)
            {
                count = (int)(Length - position);
            }

            s.Position = position + offset;
            int bytesRead = s.Read(buffer, bufOffset, count);

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = aes.CreateEncryptor();
            int counterLoc = (int)(position % 16);
            encryptor.TransformBlock(counter, 0, counter.Length, cryptedCounter, 0);
            for (int i = 0; i < bytesRead; i++)
            {
                if (position / 16 != counterBlock)
                {
                    IncrementCounter();
                    counterLoc = 0;
                    encryptor.TransformBlock(counter, 0, counter.Length, cryptedCounter, 0);
                }
                buffer[bufOffset++] ^= cryptedCounter[counterLoc++]; //decrypt one byte
                position++;
            }

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    break;
            }
            return position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    public class OffsetStream : Stream
    {
        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length { get; }

        private Stream pkg;
        private long data_offset;

        private long _position;
        public override long Position
        {
            get
            {
                return _position;
            }

            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Constructs a new offset stream on the given base stream with the given offset and length.
        /// </summary>
        /// <param name="package">The base stream</param>
        /// <param name="offset">Offset into the base stream where this stream starts</param>
        /// <param name="length">Number of bytes in this stream</param>
        public OffsetStream(Stream package, long offset, long length)
        {
            this.pkg = package;
            this.data_offset = offset;
            Length = length;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            pkg.Seek(data_offset + Position, SeekOrigin.Begin);
            if (count + Position > Length)
            {
                count = (int)(Length - Position);
            }
            int bytes_read = pkg.Read(buffer, offset, count);
            _position += bytes_read;
            return bytes_read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    break;
                case SeekOrigin.Current:
                    offset += _position;
                    break;
                case SeekOrigin.End:
                    offset += Length;
                    break;
            }
            if (offset > Length)
            {
                offset = Length;
            }
            else if (offset < 0)
            {
                offset = 0;
            }
            _position = offset;
            return _position;
        }

        #region Not Supported
        public override void Flush()
        {
            throw new NotSupportedException();
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    public class LocalFile : IFile
    {
        public bool Compressed => false;
        public long CompressedSize => Size;
        public long Size { get; }
        public string Name { get; }
        public IDirectory Parent { get; }
        public Stream Stream => GetStream();
        public IDictionary<string, object> ExtendedInfo { get; }

        private string path;

        internal LocalFile(IDirectory parent, string path)
        {
            Parent = parent;
            this.path = path;
            Size = new FileInfo(path).Length;
            this.Name = Path.GetFileName(path);
            ExtendedInfo = new Dictionary<string, object>();
        }

        public byte[] GetBytes()
        {
            return File.ReadAllBytes(path);
        }
        public Stream GetStream()
        {
            try
            {
                return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception)
            {
                return File.OpenRead(path);
            }
        }
    }
}
