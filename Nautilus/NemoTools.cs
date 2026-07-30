using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using Nautilus.x360;
using NAudio.Midi;
using Encoder = System.Drawing.Imaging.Encoder;
using Path = System.IO.Path;
using NautilusFREE;
using static Nautilus.YARGSongFileStream;
using Nautilus.Sng;
using System.Threading.Tasks;


namespace Nautilus
{
    public static unsafe partial class TheMethod3
    {
        private const string __DllName = "themethod3.dll";

        static TheMethod3()
        {
            // Get the full path of the /bin/ directory
            string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");

            // Add the /bin/ directory to the DLL search path
            SetDllDirectory(binPath);
        }

        [DllImport(__DllName, EntryPoint = "decrypt_mogg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool decrypt_mogg(byte* data, uint len);


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);
    }

    public class NemoTools
    {
        #region Wii Stuff

        // // // // // // // // // // // // // // // 
        //WII STUFF WII STUFF WII STUFF WII STUFF // 
        // // // // // // // // // // // // // // // 

        /// <summary>
        /// Converts png_wii files to usable format
        /// </summary>
        /// <param name="wii_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="output_path">Full path you'd like to save the converted image</param>
        /// <param name="format">Allowed formats: BMP | JPG | PNG (default)</param>
        /// <param name="delete_original">True: delete | False: keep (default)</param>
        /// <returns></returns>
        public bool ConvertWiiImage(string wii_image, string output_path, string format, bool delete_original)
        {
            var tplfile = Path.GetDirectoryName(wii_image) + "\\" + Path.GetFileNameWithoutExtension(wii_image) + ".tpl";
            if (!Directory.Exists(Path.GetDirectoryName(wii_image) + "\\"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(wii_image) + "\\");
            }
            var pngfile = tplfile.Replace(".tpl", ".png");
            var Headers = new ImageHeaders();

            isVerticalTexture = false;
            isHorizontalTexture = false;
            TextureSize = 128;

            DeleteFile(pngfile);

            try
            {
                if (tplfile != wii_image)
                {
                    DeleteFile(tplfile);

                    var binaryReader = new BinaryReader(File.OpenRead(wii_image));
                    var binaryWriter = new BinaryWriter(new FileStream(tplfile, FileMode.Create));

                    var wii_header = new byte[32];
                    binaryReader.Read(wii_header, 0, 32);

                    byte[] tpl_header;
                    if (wii_header.SequenceEqual(Headers.wii_32x64_pma))
                    {
                        tpl_header = Headers.tpl_32x64;
                        isHorizontalTexture = true;
                        TextureSize = 64;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_32x128_pma))
                    {
                        tpl_header = Headers.tpl_32x128;
                        isHorizontalTexture = true;
                        TextureDivider = 4;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_64x128_pma))
                    {
                        tpl_header = Headers.tpl_64x128;
                        isVerticalTexture = true;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_128x32_pma))
                    {
                        tpl_header = Headers.tpl_128x32;
                        isHorizontalTexture = true;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_128x64_pma))
                    {
                        tpl_header = Headers.tpl_128x64;
                        isHorizontalTexture = true;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_128x256) ||
                        wii_header.SequenceEqual(Headers.wii_128x256_unknown))
                    {
                        tpl_header = Headers.tpl_128x256;
                        isVerticalTexture = true;
                        TextureSize = 256;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_128x128_rgba32))
                    {
                        tpl_header = Headers.tpl_128x128_rgba32;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_256x256) ||
                             wii_header.SequenceEqual(Headers.wii_256x256_B) ||
                             wii_header.SequenceEqual(Headers.wii_256x256_c8)||
                             wii_header.SequenceEqual(Headers.wii_256x256_unknown))
                    {
                        tpl_header = Headers.tpl_256x256;
                        TextureSize = 256;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_128x128) ||
                            wii_header.SequenceEqual(Headers.wii_128x128_pma) ||
                            wii_header.SequenceEqual(Headers.wii_128x128_unknown))
                    {
                        tpl_header = Headers.tpl_128x128;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_64x64) ||
                            wii_header.SequenceEqual(Headers.wii_64x64_pma) ||
                            wii_header.SequenceEqual(Headers.wii_64x64_unknown))
                    {
                        TextureSize = 64;
                        tpl_header = Headers.tpl_64x64;
                    }
                    else
                    {
                        MessageBox.Show("File " + Path.GetFileName(wii_image) +
                            " has a header I don't recognize, so I can't convert it",
                            "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                    binaryWriter.Write(tpl_header);

                    var buffer = new byte[64];
                    int num;
                    do
                    {
                        num = binaryReader.Read(buffer, 0, 64);
                        if (num > 0)
                            binaryWriter.Write(buffer);
                    } while (num > 0);
                    binaryWriter.Dispose();
                    binaryReader.Dispose();
                }

                //this is so image quality is higher than the default
                var myEncoder = Encoder.Quality;
                var myEncoderParameters = new EncoderParameters(1);
                var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;

                var img = TPL.ConvertFromTPL(tplfile);
                img.Save(pngfile, GetEncoderInfo("image/png"), myEncoderParameters);
                img.Dispose();

                if (!File.Exists(pngfile))
                {
                    if (tplfile != wii_image)
                    {
                        DeleteFile(tplfile);
                    }
                    return false;
                }
                if (!format.ToLowerInvariant().Contains("png"))
                {
                    var image = NemoLoadImage(pngfile);
                    if (!ResizeImage(pngfile, image.Width, format, output_path))
                    {
                        image.Dispose();
                        DeleteFile(pngfile);
                        return false;
                    }
                    image.Dispose();
                }

                if (tplfile != wii_image && !KeepDDS)
                {
                    DeleteFile(tplfile);
                }
                if (!format.ToLowerInvariant().Contains("png"))
                {
                    DeleteFile(pngfile);
                }
                if (delete_original)
                {
                    SendtoTrash(wii_image);
                }
                return true;
            }
            catch (Exception)
            {
                if (tplfile != wii_image)
                {
                    DeleteFile(tplfile);
                }
                DeleteFile(pngfile);
                return false;
            }
        }

        /// <summary>
        /// Converts png_wii files to usable format
        /// </summary>
        /// <param name="wii_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="output_path">Full path you'd like to save the converted image</param>
        /// <param name="format">Allowed formats: BMP | JPG | PNG (default)</param>
        /// <returns></returns>
        public bool ConvertWiiImage(string wii_image, string output_path, string format)
        {
            return ConvertWiiImage(wii_image, output_path, format, false);
        }

        /// <summary>
        /// Converts png_wii files to png format
        /// </summary>
        /// <param name="wii_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="output_path">Full path you'd like to save the converted image</param>
        /// <returns></returns>
        public bool ConvertWiiImage(string wii_image, string output_path)
        {
            return ConvertWiiImage(wii_image, output_path, "png", false);
        }

        /// <summary>
        /// Converts png_wii files to png format
        /// </summary>
        /// <param name="wii_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <returns></returns>
        public bool ConvertWiiImage(string wii_image)
        {
            return ConvertWiiImage(wii_image, wii_image, "png", false);
        }

        /// <summary>
        /// Converts png_wii files to png format
        /// </summary>
        /// <param name="wii_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="delete_original">True - delete | False - keep (default)</param>
        /// <returns></returns>
        public bool ConvertWiiImage(string wii_image, bool delete_original)
        {
            return ConvertWiiImage(wii_image, wii_image, "png", delete_original);
        }

        /// <summary>
        /// Converts images to png_wii format
        /// </summary>
        /// <param name="wimgt_path">Full path to wimgt.exe (REQUIRED)</param>
        /// <param name="image_path">Full path of image to be converted</param>
        /// <param name="output_path">Full path of output image</param>
        /// <param name="delete_original">True: Delete | False: Keep (default)</param>
        /// <returns></returns>
        public bool ConvertImagetoWii(string wimgt_path, string image_path, string output_path, bool delete_original)
        {
            var pngfile = Path.GetDirectoryName(image_path) + "\\" + Path.GetFileNameWithoutExtension(image_path) + ".png";
            if (!Directory.Exists(Path.GetDirectoryName(image_path) + "\\"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(image_path) + "\\");
            }
            var tplfile = pngfile.Replace(".png", ".tpl");
            //var tplfile = Path.GetDirectoryName(image_path) + "\\" + Path.GetFileNameWithoutExtension(image_path) + ".tpl";
            var origfile = image_path;
            var Headers = new ImageHeaders();

            try
            {
                var ext = Path.GetExtension(image_path);
                if (ext == ".png_xbox" || ext == ".png_ps3")
                {
                    if (!ConvertRBImage(image_path, pngfile, "png", false))
                    {
                        return false;
                    }
                    image_path = pngfile;
                }
                if (!ResizeImage(image_path, 256, "png", pngfile))
                {
                    return false;
                }

                if (File.Exists(wimgt_path))
                {
                    if (image_path != tplfile)
                    {
                        DeleteFile(tplfile);

                        try
                        {
                            var arg = "-d \"" + tplfile + "\" ENC -x TPL.CMPR \"" + pngfile + "\"";
                            var startInfo = new ProcessStartInfo
                            {
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                FileName = wimgt_path,
                                Arguments = arg,
                                WorkingDirectory = Application.StartupPath + "\\bin\\"
                            };
                            var process = Process.Start(startInfo);
                            do
                            {
                                //
                            } while (!process.HasExited);
                            process.Dispose();

                            if (!File.Exists(tplfile))
                            {
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("There was an error in converting the png_wii file\nThe error was caused by wimgt.exe\nThe error says:" +
                                ex.Message, "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            if (image_path != tplfile)
                            {
                                DeleteFile(tplfile);
                            }
                            if (image_path != pngfile)
                            {
                                DeleteFile(pngfile);
                            }
                        }
                    }

                    var wiifile = Path.GetDirectoryName(origfile) + "\\" + Path.GetFileNameWithoutExtension(origfile) + "_keep.png_wii";
                    wiifile = wiifile.Replace("_keep_keep", "_keep"); //in case of double _keep markers for whatever reason

                    DeleteFile(wiifile);
                    if (origfile != pngfile)
                    {
                        DeleteFile(pngfile);
                    }

                    var binaryReader = new BinaryReader(File.OpenRead(tplfile));
                    var binaryWriter = new BinaryWriter(new FileStream(wiifile, FileMode.Create));
                    binaryReader.BaseStream.Position = 64L;
                    binaryWriter.Write(Headers.wii_256x256);
                    var buffer = new byte[64];

                    int num;
                    do
                    {
                        num = binaryReader.Read(buffer, 0, 64);
                        if (num > 0)
                            binaryWriter.Write(buffer);
                    } while (num > 0);
                    binaryWriter.Dispose();
                    binaryReader.Dispose();

                    if (image_path != tplfile && !KeepDDS)
                    {
                        DeleteFile(tplfile);
                    }
                    if (delete_original)
                    {
                        DeleteFile(origfile);
                    }
                    return File.Exists(wiifile);
                }
                MessageBox.Show("Wimgt.exe is missing and is required\nNo png_wii album art was created", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                DeleteFile(pngfile);
                DeleteFile(tplfile);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Converts images to png_wii format
        /// </summary>
        /// <param name="wimgt_path">Full path to wimgt.exe (REQUIRED)</param>
        /// <param name="image_path">Full path of image to be converted</param>
        /// <param name="output_path">Full path of output image</param>
        /// <returns></returns>
        public bool ConvertImagetoWii(string wimgt_path, string image_path, string output_path)
        {
            return ConvertImagetoWii(wimgt_path, image_path, output_path, false);
        }

        /// <summary>
        /// Converts images to png_wii format
        /// </summary>
        /// <param name="wimgt_path">Full path to wimgt.exe (REQUIRED)</param>
        /// <param name="image_path">Full path of image to be converted</param>
        /// <returns></returns>
        public bool ConvertImagetoWii(string wimgt_path, string image_path)
        {
            return ConvertImagetoWii(wimgt_path, image_path, image_path, false);
        }

        public void ProcessBINFile(string file, string title)
        {
            try
            {
                var bin = File.ReadAllBytes(file);
                var header = new byte[] { 0x00, 0x00, 0x00, 0x70 };
                var head = new[] { bin[0], bin[1], bin[2], bin[3] };
                if (!head.SequenceEqual(header))
                {
                    MessageBox.Show("That is not a valid BIN file: incorrect header", title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var sfd = new SaveFileDialog
                {
                    Filter = "Text File (*.txt)|*.txt",
                    Title = "Where should I save the ng_id.txt file?",
                    FileName = "ng_id.txt",
                    InitialDirectory = CurrentFolder
                };

                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var idfile = sfd.FileName;
                if (!idfile.EndsWith(".txt", StringComparison.Ordinal))
                {
                    idfile = idfile + ".txt";
                }
                DeleteFile(idfile);

                var idbytes = new[] { bin[8], bin[9], bin[10], bin[11] };

                var sw = new StreamWriter(idfile, false);
                sw.Write((idbytes[0].ToString("X2") + idbytes[1].ToString("X2") + idbytes[2].ToString("X2") + idbytes[3].ToString("X2")).ToLowerInvariant());
                sw.Dispose();

                MessageBox.Show("Saved ng_id successfully", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing BIN file: \n" + ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        // // // // // // // // // // // // // // // 
        //WII STUFF WII STUFF WII STUFF WII STUFF // 
        // // // // // // // // // // // // // // // 
        #endregion             

        public bool isV17(string mogg)
        {
            return isV17(File.ReadAllBytes(mogg));
        }

        public bool isV17(byte[] mogg_data)
        {
            try
            {
                using (var stream = new MemoryStream(mogg_data))
                using (var reader = new BinaryReader(stream))
                {
                    byte version = reader.ReadByte();
                    return version == 0x11;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool ConvertBandFuse(string argument, string input, string output)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "songfuse.exe"))
            {
                MessageBox.Show("songfuse.exe is missing from the bin directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            DeleteFile(output);
            var arg = argument + " \"" + input + "\" \"" + output + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = path + "songfuse.exe",
                Arguments = arg,
                WorkingDirectory = path
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();
            return File.Exists(output);
        }

        public bool MakeMogg(string ogg, string mogg)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "makemogg.exe"))
            {
                MessageBox.Show("makemogg.exe is missing from the bin directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            DeleteFile(mogg);
            var arg = "\"" + ogg + "\" -m \"" + mogg + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = path + "makemogg.exe",
                Arguments = arg,
                WorkingDirectory = path
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();
            return File.Exists(mogg);
        }

        public bool DecompressXML(string XML)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "7z.exe"))
            {
                MessageBox.Show("7z.exe is missing from the bin directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            var arg = "x -o\"" +Path.GetDirectoryName(XML) + "\" \"" + XML + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = path + "7z.exe",
                Arguments = arg,
                WorkingDirectory = path
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();

            var extracted = XML.Replace(".xml", "");
            if (!File.Exists(extracted)) return false;
            MoveFile(extracted, XML);
            return File.Exists(XML);
        }

        public string ExtractWithOnyx(string inputFile, string outputFolder)
        {
            var path = Application.StartupPath + "\\bin\\onyx\\";
            if (!File.Exists(path + "onyx.exe"))
            {
                MessageBox.Show("onyx.exe is missing from the \\bin\\onyx\\ directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            
            var arg = "extract \"" + inputFile + "\" --crypt --to \"" + outputFolder + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = path + "onyx.exe",
                Arguments = arg,
                WorkingDirectory = path
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();

            var songs_folder = outputFolder + "\\Audio\\songs\\";
            var XML = Directory.GetFiles(songs_folder, "*.xml", SearchOption.AllDirectories);
            if (XML.Count() >= 1)
            {
                return XML[0];
            }
            else
            {
                return null;
            }
        }

        public bool XMASH(string xma)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "xmash.exe"))
            {
                MessageBox.Show("xmash.exe is missing from the bin directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var newPath = Path.GetDirectoryName(xma) + "\\xmash.exe";
            if (!File.Exists(newPath))
            {
                File.Copy(path + "xmash.exe", newPath, true);
            }

            var arg = " \"" + Path.GetFileName(xma) + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = newPath,
                Arguments = arg,
                WorkingDirectory = Path.GetDirectoryName(newPath)
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();

            var xmas = Directory.GetFiles(Path.GetDirectoryName(xma), "*.xma");
            return xmas.Count() > 1;
        }

        public bool toWAV(string xma)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "towav.exe"))
            {
                MessageBox.Show("towav.exe is missing from the bin directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var newPath = Path.GetDirectoryName(xma) + "\\towav.exe";
            if (!File.Exists(newPath))
            {
                File.Copy(path + "towav.exe", newPath, true);
            }

            var arg = " \"" + Path.GetFileName(xma) + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = newPath,
                Arguments = arg,
                WorkingDirectory = Path.GetDirectoryName(newPath)
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();
            var wav = xma.Substring(0, xma.Length - 3) + "wav";
            return File.Exists(wav);
        }

        #region Declarations

        public string CurrentFolder = ""; //used throughout the program to maintain the current working folder
        public string MIDI_ERROR_MESSAGE;
        public bool isHorizontalTexture;
        public bool isVerticalTexture;
        private int TextureDivider = 2; //default value
        public int TextureSize = 512; //default value
        public bool KeepDDS = false;
        public bool isSaveFileCharacter = false; //need a different header
        public bool isSaveFileArt = false; //need to disable mip maps
        public string SaveFileBandName;
        public List<string> SaveFileCharNames;
        private const int FO_DELETE = 0x0003;
        private const int FOF_ALLOWUNDO = 0x0040;           // Preserve undo information, if possible.
        private const int FOF_NOCONFIRMATION = 0x0010;      // Show no confirmation dialog box to the user
        public string DDS_Format;
        public bool useDXT5 = true;

        // Struct which contains information that the SHFileOperation function uses to perform file operations.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
        #endregion
            
        #region Image Stuff

        public ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            var encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private const string ClientId = "248d1718f9e6925"; //from imgur, specific to this program, do not use elsewhere
        public string UploadToImgur(string image)
        {
            var link = "";
            try
            {
                using (var w = new WebClient())
                {
                    var values = new NameValueCollection
                        {
                            {"image", Convert.ToBase64String(File.ReadAllBytes(image))}
                        };
                    w.Headers.Add("Authorization", "Client-ID " + ClientId);
                    var response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);
                    var sr = new StreamReader(new MemoryStream(response), Encoding.Default);
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (line == null || !line.Contains("link")) continue;
                        var linkStart = line.IndexOf("<link>", StringComparison.Ordinal) + 6;
                        var linkEnd = line.IndexOf("</link>", linkStart, StringComparison.Ordinal);
                        link = line.Substring(linkStart, linkEnd - linkStart);
                    }
                    sr.Dispose();
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                link = "";
                if (error.Contains("429"))
                {
                    error = "Error Code 429: Rate limiting\nThis most likely means users have uploaded too many images recently\nPlease wait a couple of hours and try again";
                }
                else if (error.Contains("500"))
                {
                    error = "Error Code 500: Unexpected internal error\nThis most likely means something is broken with the Imgur service\nPlease try again later";
                }
                if (MessageBox.Show("Sorry, there was an error uploading that image!\n\nThe error says:\n" + error + "\n\nTry again?", "Error",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    UploadToImgur(image);
                }
            }
            return link;
        }

        /// <summary>
        /// Loads image and unlocks file for uses elsewhere. USE THIS!
        /// </summary>
        /// <param name="file">Full path to the image file</param>
        /// <returns></returns>
        public Image NemoLoadImage(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }

            Image img;
            using (var bmpTemp = new Bitmap(file))
            {
                img = new Bitmap(bmpTemp);
            }

            return img;
        }

        /// <summary>
        /// Returns true after successful conversion to png_xbox format
        /// </summary>
        /// <param name="image_path">Full path to original image</param>
        /// <param name="output_path">Full path of output png_xbox file</param>
        /// <param name="delete_original">True to delete original image | False to keep original image</param>
        /// <param name="do_ps3">Set true to output png_ps3 image, false to output png_xbox (default)</param>
        /// <returns></returns>
        public bool ConvertImagetoRB(string image_path, string output_path, bool delete_original, bool do_ps3 = false)
        {
            var ddsfile = Path.GetDirectoryName(image_path) + "\\" + Path.GetFileNameWithoutExtension(image_path) + ".dds";
            var keep = isSaveFileArt || isSaveFileCharacter ? "" : "_keep";
            var outputfile = Path.GetDirectoryName(output_path) + "\\" + Path.GetFileNameWithoutExtension(output_path) + keep + ".png_" + (do_ps3 ? "ps3" : "xbox");
            outputfile = outputfile.Replace("_keep_keep", "_keep"); //in case it already had it and was added above
            var tgafile = Application.StartupPath + "\\bin\\temp.tga";
            var Headers = new ImageHeaders();

            var nv_tool = Application.StartupPath + "\\bin\\nvcompress.exe";
            if (!File.Exists(nv_tool))
            {
                MessageBox.Show("nvcompress.exe is missing and is required\nProcess aborted", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                if (ddsfile != image_path)
                {
                    DeleteFile(ddsfile);
                }
                DeleteFile(outputfile);
                DeleteFile(tgafile);

                if (!image_path.EndsWith(".dds", StringComparison.Ordinal)) //allow for .dds input image for superior quality
                {
                    if (!ResizeImage(image_path, TextureSize, "tga", tgafile))
                    {
                        return false;
                    }

                    try
                    {
                        var mip = isSaveFileArt || isSaveFileCharacter ? "-nomips " : ""; //need to disable mips, otherwise always enable
                        //save as 512x512 / 1024x1024 DXT5 textures - first time ever in RB3 customs @ Nemo 2014 bitches
                        var arg = mip + "-nocuda " + (useDXT5 ? "-bc3" : "-bc1") + " \"" + tgafile + "\" \"" + ddsfile + "\"";
                        var startInfo = new ProcessStartInfo
                        {
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            FileName = nv_tool,
                            Arguments = arg,
                            WorkingDirectory = Application.StartupPath + "\\bin\\"
                        };
                        var process = Process.Start(startInfo);
                        do
                        {
                            //
                        } while (!process.HasExited);
                        process.Dispose();

                        if (!File.Exists(ddsfile))
                        {
                            return false;
                        }
                        DeleteFile(tgafile);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                else
                {
                    ddsfile = image_path;
                }

                //read all raw bytes
                var ddsbytes = File.ReadAllBytes(ddsfile);
                var buffer = new byte[4];
                var swap = new byte[4];

                //default header
                var header = useDXT5 ? Headers.RB3_512x512_DXT5 : Headers.RB3_512x512_DXT1;
                if (isSaveFileCharacter)
                {
                    header = Headers.RB3_256x512_DXT5_NOMIP;
                }
                else if (isSaveFileArt)
                {
                    header = Headers.RB3_256x256_DXT5_NOMIP;
                }
                else
                {
                    switch (TextureSize)
                    {
                        case 256:
                            header = useDXT5 ? Headers.RB3_256x256_DXT5 : Headers.RB3_256x256_DXT1;
                            break;
                        case 1024:
                            header = useDXT5 ? Headers.NEMO_1024x1024_DXT5 : Headers.NEMO_1024x1024_DXT1;
                            break;
                        case 2048:
                            header = useDXT5 ? Headers.NEMO_2048x2048_DXT5 : Headers.NEMO_2048x2048_DXT1;
                            break;
                    }
                }

                //get filesize / 4 for number of times to loop
                //128 is size of dds header we have to skip
                var loop = (ddsbytes.Length - 128) / 4;

                //skip the first dds header
                var input = new MemoryStream(ddsbytes, 128, ddsbytes.Length - 128);
                var output = new FileStream(outputfile, FileMode.Create);

                //write HMX header
                output.Write(header, 0, header.Length);

                //here we go
                for (var x = 0; x <= loop; x++)
                {
                    input.Read(buffer, 0, 4);
                    //png_ps3 are not byte swapped, no need to change anything
                    if (do_ps3)
                    {
                        swap = buffer;
                    }
                    else
                    {
                        //png_xbox are byte swapped, so got to change it here
                        swap[0] = buffer[1];
                        swap[1] = buffer[0];
                        swap[2] = buffer[3];
                        swap[3] = buffer[2];
                    }
                    output.Write(swap, 0, 4);
                }
                input.Dispose();
                output.Dispose();

                //clean up temporary file silently
                if (!image_path.EndsWith(".dds", StringComparison.Ordinal) && !KeepDDS)
                {
                    DeleteFile(ddsfile);
                }
                if (delete_original)
                {
                    SendtoTrash(image_path);
                }
                if (File.Exists(outputfile))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                if (!image_path.EndsWith(".dds", StringComparison.Ordinal))
                {
                    DeleteFile(ddsfile);
                }
                DeleteFile(tgafile);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Returns true after successful conversion to png_xbox format
        /// </summary>
        /// <param name="image_path">Full path to original image</param>
        /// <param name="output_path">Full path of output png_xbox file</param>
        /// <returns></returns>
        public bool ConvertImagetoRB(string image_path, string output_path)
        {
            return ConvertImagetoRB(image_path, output_path, false);
        }

        /// <summary>
        /// Returns true after successful conversion to png_xbox format
        /// </summary>
        /// <param name="image_path">Full path to original image</param>
        /// <returns></returns>
        public bool ConvertImagetoRB(string image_path)
        {
            return ConvertImagetoRB(image_path, image_path, false);
        }

        /// <summary>
        /// Returns true after successful conversion to png_xbox format
        /// </summary>
        /// <param name="image_path">Full path to original image</param>
        /// <param name="delete_original">True - delete | False - keep (default)</param>
        /// <returns></returns>
        public bool ConvertImagetoRB(string image_path, bool delete_original)
        {
            return ConvertImagetoRB(image_path, image_path, delete_original);
        }

        /// <summary>
        /// Use to resize images up or down or convert across BMP/JPG/PNG/TIF
        /// </summary>
        /// <param name="image_path">Full file path to source image</param>
        /// <param name="image_size">Integer for image size, can be smaller or bigger than source image</param>
        /// <param name="format">Format to save the image in: BMP | JPG | TIF | PNG (default)</param>
        /// <param name="output_path">Full file path to output image</param>
        /// <returns></returns>
        public bool ResizeImage(string image_path, int image_size, string format, string output_path)
        {
            try
            {
                var newImage = Path.Combine(Path.GetDirectoryName(output_path), Path.GetFileNameWithoutExtension(output_path));

                Il.ilInit();
                Ilu.iluInit();

                var imageId = new int[1];
                Il.ilGenImages(1, imageId);
                Il.ilBindImage(imageId[0]);

                if (!Il.ilLoadImage(image_path))
                    return false;

                Il.ilEnable(Il.IL_FILE_OVERWRITE);

                var height = isHorizontalTexture ? image_size / TextureDivider : image_size;
                var width = isVerticalTexture ? image_size / TextureDivider : image_size;

                // Ensure we’re working in RGBA format
                Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE);

                // Step 1: Premultiply alpha
                int origWidth = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int origHeight = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bpp = Il.ilGetInteger(Il.IL_IMAGE_BPP); // should be 4 (RGBA)
                IntPtr data = Il.ilGetData();

                byte[] pixels = new byte[origWidth * origHeight * bpp];
                Marshal.Copy(data, pixels, 0, pixels.Length);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    float alpha = pixels[i + 3] / 255f;
                    pixels[i + 0] = (byte)(pixels[i + 0] * alpha); // R
                    pixels[i + 1] = (byte)(pixels[i + 1] * alpha); // G
                    pixels[i + 2] = (byte)(pixels[i + 2] * alpha); // B
                }

                Marshal.Copy(pixels, 0, data, pixels.Length);

                // Step 2: Resize with bilinear filter
                Ilu.iluImageParameter(Ilu.ILU_FILTER, Ilu.ILU_BILINEAR);
                Ilu.iluScale(width, height, 1);

                // Step 3: Un-premultiply alpha
                int scaledWidth = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int scaledHeight = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                bpp = Il.ilGetInteger(Il.IL_IMAGE_BPP);
                data = Il.ilGetData();
                pixels = new byte[scaledWidth * scaledHeight * bpp];
                Marshal.Copy(data, pixels, 0, pixels.Length);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    float alpha = pixels[i + 3] / 255f;
                    if (alpha > 0)
                    {
                        pixels[i + 0] = (byte)Math.Min(255, pixels[i + 0] / alpha); // R
                        pixels[i + 1] = (byte)Math.Min(255, pixels[i + 1] / alpha); // G
                        pixels[i + 2] = (byte)Math.Min(255, pixels[i + 2] / alpha); // B
                    }
                }

                Marshal.Copy(pixels, 0, data, pixels.Length);

                // Set output format
                if (format.ToLowerInvariant().Contains("bmp"))
                {
                    Il.ilSetInteger(Il.IL_BMP_RLE, 0);
                    newImage += ".bmp";
                }
                else if (format.ToLowerInvariant().Contains("jpg") || format.ToLowerInvariant().Contains("jpeg"))
                {
                    Il.ilSetInteger(Il.IL_JPG_QUALITY, 99);
                    newImage += ".jpg";
                }
                else if (format.ToLowerInvariant().Contains("tif"))
                {
                    newImage += ".tif";
                }
                else if (format.ToLowerInvariant().Contains("tga"))
                {
                    Il.ilSetInteger(Il.IL_TGA_RLE, 0);
                    newImage += ".tga";
                }
                else
                {
                    Il.ilSetInteger(Il.IL_PNG_INTERLACE, 0); // Fine to keep
                    newImage += ".png";
                }

                if (!Il.ilSaveImage(newImage))
                    return false;

                Il.ilDeleteImages(1, imageId);
                return File.Exists(newImage);
            }
            catch
            {
                return false;
            }
        }
        /*public bool ResizeImage(string image_path, int image_size, string format, string output_path)
        {
            try
            {
                var newImage = Path.GetDirectoryName(output_path) + "\\" + Path.GetFileNameWithoutExtension(output_path);

                Il.ilInit();
                Ilu.iluInit();

                var imageId = new int[10];

                // Generate the main image name to use
                Il.ilGenImages(1, imageId);

                // Bind this image name
                Il.ilBindImage(imageId[0]);

                // Loads the image into the imageId
                if (!Il.ilLoadImage(image_path))
                {
                    return false;
                }
                // Enable overwriting destination file
                Il.ilEnable(Il.IL_FILE_OVERWRITE);

                var height = isHorizontalTexture ? image_size / TextureDivider : image_size;
                var width = isVerticalTexture ? image_size / TextureDivider : image_size;

                //assume we're downscaling, this is better filter
                const int scaler = Ilu.ILU_BILINEAR;

                //resize image
                //Ilu.iluImageParameter(Ilu.ILU_FILTER, scaler);
                //Ilu.iluScale(width, height, 1);

                // Premultiply alpha
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bpp = Il.ilGetInteger(Il.IL_IMAGE_BPP); // should be 4
                IntPtr data = Il.ilGetData();
                byte[] pixels = new byte[width * height * bpp];
                System.Runtime.InteropServices.Marshal.Copy(data, pixels, 0, pixels.Length);

                // Premultiply alpha
                for (int i = 0; i < pixels.Length; i += 4)
                {
                    float alpha = pixels[i + 3] / 255f;
                    pixels[i + 0] = (byte)(pixels[i + 0] * alpha); // R
                    pixels[i + 1] = (byte)(pixels[i + 1] * alpha); // G
                    pixels[i + 2] = (byte)(pixels[i + 2] * alpha); // B
                }

                // Write modified data back to DevIL
                System.Runtime.InteropServices.Marshal.Copy(pixels, 0, data, pixels.Length);

                // Now do your iluScale
                Ilu.iluImageParameter(Ilu.ILU_FILTER, Ilu.ILU_BILINEAR);
                Ilu.iluScale(width, height, 1);

                // After scaling, un-premultiply alpha
                data = Il.ilGetData();
                pixels = new byte[width * height * bpp];
                System.Runtime.InteropServices.Marshal.Copy(data, pixels, 0, pixels.Length);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    float alpha = pixels[i + 3] / 255f;
                    if (alpha > 0)
                    {
                        pixels[i + 0] = (byte)Math.Min(255, pixels[i + 0] / alpha); // R
                        pixels[i + 1] = (byte)Math.Min(255, pixels[i + 1] / alpha); // G
                        pixels[i + 2] = (byte)Math.Min(255, pixels[i + 2] / alpha); // B
                    }
                }

                // Write back to DevIL
                System.Runtime.InteropServices.Marshal.Copy(pixels, 0, data, pixels.Length);

                if (format.ToLowerInvariant().Contains("bmp"))
                {
                    //disable compression
                    Il.ilSetInteger(Il.IL_BMP_RLE, 0);
                    newImage = newImage + ".bmp";
                }
                else if (format.ToLowerInvariant().Contains("jpg") || format.ToLowerInvariant().Contains("jpeg"))
                {
                    Il.ilSetInteger(Il.IL_JPG_QUALITY, 99);
                    newImage = newImage + ".jpg";
                }
                else if (format.ToLowerInvariant().Contains("tif"))
                {
                    newImage = newImage + ".tif";
                }
                else if (format.ToLowerInvariant().Contains("tga"))
                {
                    Il.ilSetInteger(Il.IL_TGA_RLE, 0);
                    newImage = newImage + ".tga";
                }
                else
                {
                    Il.ilSetInteger(Il.IL_PNG_INTERLACE, 0);
                    newImage = newImage + ".png";
                }

                if (!Il.ilSaveImage(newImage))
                {
                    return false;
                }

                // Done with the imageId, so let's delete it
                Il.ilDeleteImages(1, imageId);

                return File.Exists(newImage);
            }
            catch (Exception)
            {
                return false;
            }
        }*/

        /// <summary>
        /// Use to resize images up or down or convert across BMP/JPG/PNG
        /// </summary>
        /// <param name="image_path">Full file path to source image</param>
        /// <param name="image_size">Integer for image size, can be smaller or bigger than source image</param>
        /// <param name="format">Format to save the image in: BMP | JPG | PNG (default)</param>
        /// <returns></returns>
        public bool ResizeImage(string image_path, int image_size, string format)
        {
            return ResizeImage(image_path, image_size, format, image_path);
        }

        /// <summary>
        /// Use to resize images up or down or convert across BMP/JPG/PNG
        /// </summary>
        /// <param name="image_path">Full file path to source image</param>
        /// <param name="image_size">Integer for image size, can be smaller or bigger than source image</param>
        /// <returns></returns>
        public bool ResizeImage(string image_path, int image_size)
        {
            var img = NemoLoadImage(image_path);
            var format = img.RawFormat.ToString();
            img.Dispose();

            return ResizeImage(image_path, image_size, format);
        }

        /// <summary>
        /// Converts png_xbox image to png_ps3 image
        /// </summary>
        /// <param name="image">Full file path to the image to be converted</param>
        /// <param name="output_path">Full directory path where new image is to be saved</param>
        /// <param name="delete_original">Whether to delete the original image upon completion</param>
        /// <returns></returns>
        public bool ConvertXboxtoPS3(string image, string output_path, bool delete_original)
        {
            return FlipRBImageBytes(image, output_path, "ps3", delete_original);
        }

        /// <summary>
        /// Converts png_ps3 image to png_xbox image
        /// </summary>
        /// <param name="image">Full file path to the image to be converted</param>
        /// <param name="output_path">Full directory path where new image is to be saved</param>
        /// <param name="delete_original">Whether to delete the original image upon completion</param>
        /// <returns></returns>
        public bool ConvertPS3toXbox(string image, string output_path, bool delete_original)
        {
            return FlipRBImageBytes(image, output_path, "xbox", delete_original);
        }

        /// <summary>
        /// Converts png_xbox to/from png_ps3
        /// </summary>
        /// <param name="image">Full file path to the image to convert</param>
        /// <param name="output_path">Full directory path where new image is to be saved</param>
        /// <param name="extension">Either 'xbox' or 'ps3'</param>
        /// <param name="delete_original">Whether to delete the original</param>
        /// <returns></returns>
        private bool FlipRBImageBytes(string image, string output_path, string extension, bool delete_original)
        {
            try
            {
                var output_image = Path.GetDirectoryName(output_path) + "\\" + Path.GetFileNameWithoutExtension(output_path) + ".png_" + extension.ToLowerInvariant();

                if (output_image == image)
                {
                    return true; //why are you wasting my time?
                }

                //read all raw bytes
                var ddsbytes = File.ReadAllBytes(image);
                var buffer = new byte[4];
                var swap = new byte[4];

                //get filesize / 4 for number of times to loop
                //32 is size of HMX header
                var loop = (ddsbytes.Length - 32) / 4;

                //grab the header
                var header = new byte[32];
                var header_stream = new MemoryStream(ddsbytes, 0, 32);
                header_stream.Read(header, 0, 32);

                //skip the HMX header = leaves us with image bytes
                var input = new MemoryStream(ddsbytes, 32, ddsbytes.Length - 32);

                //create new image
                var output = new FileStream(output_image, FileMode.Create);

                //both png_xbox and png_ps3 files use the same header, put it back
                output.Write(header, 0, header.Length);

                //here we go
                for (var x = 0; x <= loop; x++)
                {
                    input.Read(buffer, 0, 4);

                    swap[0] = buffer[1];
                    swap[1] = buffer[0];
                    swap[2] = buffer[3];
                    swap[3] = buffer[2];

                    output.Write(swap, 0x00, 4);
                }
                input.Dispose();
                output.Dispose();

                if (delete_original)
                {
                    DeleteFile(image);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static byte[] BuildDDSHeader(string format, int width, int height)
        {
            var dds = new byte[] //512x512 DXT5 
                {
                    0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x0A, 0x00, 0x00, 0x02, 0x00, 0x00,
                    0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x4E, 0x45, 0x4D, 0x4F, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                    0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };

            switch (format.ToLowerInvariant())
            {
                case "dxt1":
                    dds[87] = 0x31;
                    break;
                case "dxt3":
                    dds[87] = 0x33;
                    break;
                case "normal":
                    dds[84] = 0x41;
                    dds[85] = 0x54;
                    dds[86] = 0x49;
                    dds[87] = 0x32;
                    break;
            }

            switch (height)
            {
                case 8:
                    dds[12] = 0x08;
                    dds[13] = 0x00;
                    break;
                case 16:
                    dds[12] = 0x10;
                    dds[13] = 0x00;
                    break;
                case 32:
                    dds[12] = 0x20;
                    dds[13] = 0x00;
                    break;
                case 64:
                    dds[12] = 0x40;
                    dds[13] = 0x00;
                    break;
                case 128:
                    dds[12] = 0x80;
                    dds[13] = 0x00;
                    break;
                case 256:
                    dds[13] = 0x01;
                    break;
                case 1024:
                    dds[13] = 0x04;
                    break;
                case 2048:
                    dds[13] = 0x08;
                    break;
            }

            switch (width)
            {
                case 8:
                    dds[16] = 0x08;
                    dds[17] = 0x00;
                    break;
                case 16:
                    dds[16] = 0x10;
                    dds[17] = 0x00;
                    break;
                case 32:
                    dds[16] = 0x20;
                    dds[17] = 0x00;
                    break;
                case 64:
                    dds[16] = 0x40;
                    dds[17] = 0x00;
                    break;
                case 128:
                    dds[16] = 0x80;
                    dds[17] = 0x00;
                    break;
                case 256:
                    dds[17] = 0x01;
                    break;
                case 1024:
                    dds[17] = 0x04;
                    break;
                case 2048:
                    dds[17] = 0x08;
                    break;
            }

            if (width == height)
            {
                switch (width)
                {
                    case 8:
                        dds[0x1C] = 0x00; //no mipmaps at this size
                        break;
                    case 16:
                        dds[0x1C] = 0x05;
                        break;
                    case 32:
                        dds[0x1C] = 0x06;
                        break;
                    case 64:
                        dds[0x1C] = 0x07;
                        break;
                    case 128:
                        dds[0x1C] = 0x08;
                        break;
                    case 256:
                        dds[0x1C] = 0x09;
                        break;
                    case 1024:
                        dds[0x1C] = 0x0B;
                        break;
                    case 2048:
                        dds[0x1C] = 0x0C;
                        break;
                }
            }
            return dds;
        }

        /// <summary>
        /// Figure out right DDS header to go with HMX texture
        /// </summary>
        /// <param name="full_header">First 16 bytes of the png_xbox/png_ps3 file</param>
        /// <param name="short_header">Bytes 5-16 of the png_xbox/png_ps3 file</param>
        /// <returns></returns>
        private byte[] GetDDSHeader(IEnumerable<byte> full_header, IEnumerable<byte> short_header)
        {
            //official album art header, most likely to be the one being requested
            var header = BuildDDSHeader("dxt1", 256, 256);

            var headers = Directory.GetFiles(Application.StartupPath + "\\bin\\headers\\", "*.header");
            DDS_Format = "UNKNOWN";
            foreach (var head_name in from head in headers let header_bytes = File.ReadAllBytes(head) where full_header.SequenceEqual(header_bytes) || short_header.SequenceEqual(header_bytes) select Path.GetFileNameWithoutExtension(head).ToLowerInvariant())
            {
                DDS_Format = "DXT5";
                if (head_name.Contains("dxt1"))
                {
                    DDS_Format = "DXT1";
                }
                else if (head_name.Contains("normal"))
                {
                    DDS_Format = "NORMAL_MAP";
                }

                var index1 = head_name.IndexOf("_", StringComparison.Ordinal) + 1;
                var index2 = head_name.IndexOf("x", StringComparison.Ordinal);
                var width = Convert.ToInt16(head_name.Substring(index1, index2 - index1));
                index1 = head_name.IndexOf("_", index2, StringComparison.Ordinal);
                index2++;
                var height = Convert.ToInt16(head_name.Substring(index2, index1 - index2));

                header = BuildDDSHeader(DDS_Format.ToLowerInvariant().Replace("_map", ""), width, height);
                break;
            }
            return header;
        }

        /// <summary>
        /// Converts png_xbox files to usable format
        /// </summary>
        /// <param name="rb_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="output_path">Full path you'd like to save the converted image</param>
        /// <param name="format">Allowed formats: BMP | JPG | PNG (default)</param>
        /// <param name="delete_original">True: delete | False: keep (default)</param>
        /// <returns></returns>
        public bool ConvertRBImage(string rb_image, string output_path, string format, bool delete_original)
        {
            var ddsfile = Path.GetDirectoryName(output_path) + "\\" + Path.GetFileNameWithoutExtension(output_path) + ".dds";
            var tgafile = ddsfile.Replace(".dds", ".tga");

            TextureSize = 256; //default size album art
            TextureDivider = 2; //default to divide larger size by, always multiples of 2
            isHorizontalTexture = false; //this is a rectangle wider than tall
            isVerticalTexture = false; //this is a rectangle taller than wide

            var nvTool = Application.StartupPath + "\\bin\\nvdecompress.exe";
            if (!File.Exists(nvTool))
            {
                MessageBox.Show("nvdecompress.exe is missing and is required\nProcess aborted", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                if (ddsfile != rb_image)
                {
                    DeleteFile(ddsfile);
                }
                DeleteFile(tgafile);

                //read raw file bytes
                var ddsbytes = File.ReadAllBytes(rb_image);

                if (!rb_image.EndsWith(".dds", StringComparison.Ordinal))
                {
                    var buffer = new byte[4];
                    var swap = new byte[4];

                    //get filesize / 4 for number of times to loop
                    //32 is the size of the HMX header to skip
                    var loop = (ddsbytes.Length - 32) / 4;

                    //skip the HMX header
                    var input = new MemoryStream(ddsbytes, 32, ddsbytes.Length - 32);

                    //grab HMX header to compare against known headers
                    var full_header = new byte[16];
                    var file_header = new MemoryStream(ddsbytes, 0, 16);
                    file_header.Read(full_header, 0, 16);
                    file_header.Dispose();

                    //some games have a bunch of headers for the same files, so let's skip the varying portion and just
                    //grab the part that tells us the dimensions and image format
                    var short_header = new byte[11];
                    file_header = new MemoryStream(ddsbytes, 5, 11);
                    file_header.Read(short_header, 0, 11);
                    file_header.Dispose();

                    //create dds file
                    var output = new FileStream(ddsfile, FileMode.Create);
                    var header = GetDDSHeader(full_header, short_header);
                    output.Write(header, 0, header.Length);

                    //here we go
                    for (var x = 0; x <= loop; x++)
                    {
                        input.Read(buffer, 0, 4);

                        //PS3 images are not byte swapped, just DDS images with HMX header on top
                        if (rb_image.EndsWith("_ps3", StringComparison.Ordinal))
                        {
                            swap = buffer;
                        }
                        else
                        {
                            //XBOX images are byte swapped, so we gotta return it
                            swap[0] = buffer[1];
                            swap[1] = buffer[0];
                            swap[2] = buffer[3];
                            swap[3] = buffer[2];
                        }
                        output.Write(swap, 0, 4);
                    }
                    input.Dispose();
                    output.Dispose();
                }
                else
                {
                    ddsfile = rb_image;
                    tgafile = ddsfile.Replace(".dds", ".tga");
                }

                //read raw dds bytes
                ddsbytes = File.ReadAllBytes(ddsfile);

                //grab relevant part of dds header
                var header_stream = new MemoryStream(ddsbytes, 0, 32);
                var size = new byte[32];
                header_stream.Read(size, 0, 32);
                header_stream.Dispose();

                //default to 256x256
                var width = 256;
                var height = 256;

                //get dds dimensions from header
                switch (size[17]) //width byte
                {
                    case 0x00:
                        switch (size[16])
                        {
                            case 0x08:
                                width = 8;
                                break;
                            case 0x10:
                                width = 16;
                                break;
                            case 0x20:
                                width = 32;
                                break;
                            case 0x40:
                                width = 64;
                                break;
                            case 0x80:
                                width = 128;
                                break;
                        }
                        break;
                    case 0x02:
                        width = 512;
                        break;
                    case 0x04:
                        width = 1024;
                        break;
                    case 0x08:
                        width = 2048;
                        break;
                }
                switch (size[13]) //height byte
                {
                    case 0x00:
                        switch (size[12])
                        {
                            case 0x08:
                                height = 8;
                                break;
                            case 0x10:
                                height = 16;
                                break;
                            case 0x20:
                                height = 32;
                                break;
                            case 0x40:
                                height = 64;
                                break;
                            case 0x80:
                                height = 128;
                                break;
                        }
                        break;
                    case 0x02:
                        height = 512;
                        break;
                    case 0x04:
                        height = 1024;
                        break;
                    case 0x08:
                        height = 2048;
                        break;
                }

                if (width > height)
                {
                    isHorizontalTexture = true;
                    TextureDivider = width / height;
                    TextureSize = width;
                }
                else if (height > width)
                {
                    isVerticalTexture = true;
                    TextureDivider = height / width;
                    TextureSize = height;
                }
                else
                {
                    TextureSize = width;
                }

                var arg = "\"" + ddsfile + "\"";
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = nvTool,
                    Arguments = arg,
                    WorkingDirectory = Application.StartupPath + "\\bin\\"
                };
                var process = Process.Start(startInfo);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();

                if (!ResizeImage(tgafile, TextureSize, format, output_path))
                {
                    DeleteFile(tgafile);
                    return false;
                }
                if (!rb_image.EndsWith(".dds", StringComparison.Ordinal) && !KeepDDS)
                {
                    DeleteFile(ddsfile);
                }
                if (!format.ToLowerInvariant().Contains("tga"))
                {
                    DeleteFile(tgafile);
                }
                if (delete_original)
                {
                    SendtoTrash(rb_image);
                }
                return true;
            }
            catch (Exception)
            {
                if (!rb_image.EndsWith(".dds", StringComparison.Ordinal))
                {
                    DeleteFile(ddsfile);
                }
                return false;
            }
        }

        /// <summary>
        /// Converts png_xbox files to usable format
        /// </summary>
        /// <param name="rb_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="output_path">Full path you'd like to save the converted image</param>
        /// <param name="format">Allowed formats: BMP | JPG | PNG (default)</param>
        /// <returns></returns>
        public bool ConvertRBImage(string rb_image, string output_path, string format)
        {
            return ConvertRBImage(rb_image, output_path, format, false);
        }

        /// <summary>
        /// Converts png_xbox files to usable format
        /// </summary>
        /// <param name="rb_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="output_path">Full path you'd like to save the converted image</param>
        /// <returns></returns>
        public bool ConvertRBImage(string rb_image, string output_path)
        {
            return ConvertRBImage(rb_image, output_path, "png", false);
        }

        /// <summary>
        /// Converts png_xbox files to usable format
        /// </summary>
        /// <param name="rb_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <returns></returns>
        public bool ConvertRBImage(string rb_image)
        {
            return ConvertRBImage(rb_image, rb_image, "png", false);
        }

        /// <summary>
        /// Converts png_xbox files to usable format
        /// </summary>
        /// <param name="rb_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="delete_original">True - delete | False - keep (default)</param>
        /// <returns></returns>
        public bool ConvertRBImage(string rb_image, bool delete_original)
        {
            return ConvertRBImage(rb_image, rb_image, "png", delete_original);
        }
        #endregion

        #region Misc Stuff

        public int GetDiffTag(Control instrument)
        {
            int diff;
            try
            {
                diff = Convert.ToInt16(instrument.Tag);
            }
            catch (Exception)
            {
                return 0;
            }
            return diff;
        }

        public void ReplaceSongID(string dta, string newID, string vocals = "", string existingID = "")
        {
            var songHasSongID = false;
            var Parser = new DTAParser();
            var encoding = Parser.DetectEncoding(dta);
            var sr = new StreamReader(dta, encoding);
            var lines = new List<string>();
            while (sr.Peek() >= 0)
            {
                var line = sr.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line);
                }
            }
            sr.Dispose();
            var doVocals = !string.IsNullOrWhiteSpace(vocals) && vocals != "0";
            var sw = new StreamWriter(dta, false, new UTF8Encoding(false, false));
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                /*if (!string.IsNullOrEmpty(existingID) && line.Contains(existingID) && !line.Contains("/"))
                {
                    sw.WriteLine(line.Replace(existingID, newID));
                    continue;
                }*/
                if (line.Contains("song_id"))
                {
                    songHasSongID = true;
                }
                if (line.Contains("song_id") && !line.Trim().StartsWith(";") && !line.Contains(newID)) //avoid adding extra line with duplicate ID
                {
                    sw.WriteLine(";" + line.Trim());
                    sw.WriteLine("   ('song_id' " + newID + ")");
                    continue;
                }
                if (line.Contains("latin1"))
                {
                    line = line.Replace("latin1", "utf8");
                    sw.WriteLine(line);
                    continue;
                }
                if (line.Contains("vocal_parts") && doVocals) continue;
                if ((line.Trim() == "(song" || line.Trim() == "'song'") && doVocals)
                {
                    sw.WriteLine(line);
                    sw.WriteLine("      (vocal_parts " + vocals + ")");
                    continue;
                }
                sw.WriteLine(line);
            }
            sw.Dispose();

            if (!songHasSongID) //do it again this time to manually insert song_ID that isn't there
            {
                sr = new StreamReader(dta, encoding);
                lines = new List<string>();
                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }
                sr.Dispose();
                sw = new StreamWriter(dta, false, new UTF8Encoding(false, false));
                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    if (line.Contains("(format") || line.Contains("'format'") && !songHasSongID)
                    {
                        sw.WriteLine("   ('song_id' " + newID + ")");
                        songHasSongID = true;
                    }
                    sw.WriteLine(line);
                }
                sw.Dispose();
            }
        }

        /// <summary>
        /// Returns line with featured artist normalized as 'ft.'
        /// </summary>
        /// <param name="line">Line to normalize</param>
        /// <returns></returns>
        public string FixFeaturedArtist(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return "";

            var adjusted = line;

            adjusted = adjusted.Replace("Featuring", "ft.");
            adjusted = adjusted.Replace("featuring", "ft.");
            adjusted = adjusted.Replace("feat.", "ft.");
            adjusted = adjusted.Replace("Feat.", "ft.");
            adjusted = adjusted.Replace(" ft ", " ft. ");
            adjusted = adjusted.Replace(" FT ", " ft. ");
            adjusted = adjusted.Replace("Ft. ", "ft. ");
            adjusted = adjusted.Replace("FEAT. ", "ft. ");
            adjusted = adjusted.Replace(" FEAT ", " ft. ");

            if (adjusted.StartsWith("ft ", StringComparison.Ordinal))
            {
                adjusted = "ft. " + adjusted.Substring(3, adjusted.Length - 3);
            }

            return FixBadChars(adjusted);
        }

        /// <summary>
        /// Loads and formats help file for display on the HelpForm
        /// </summary>
        /// <param name="file">Name of the file, path assumed to be \bin\help/</param>
        /// <returns></returns>
        public string ReadHelpFile(string file)
        {
            var message = string.Empty;
            var helpfile = Application.StartupPath + "\\bin\\help\\" + file;
            if (File.Exists(helpfile))
            {
                var sr = new StreamReader(helpfile);
                while (sr.Peek() > 0)
                {
                    var line = sr.ReadLine();
                    message = message == string.Empty ? line : message + "\r\n" + line;
                }
                sr.Dispose();
            }
            else
            {
                message = "Could not find help file, please redownload this program and DO NOT delete any files";
            }

            return message;
        }

        public Color LightenColor(Color color)
        {
            var correctionFactor = (float)0.20;

            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public Color DarkenColor(Color color)
        {
            var correctionFactor = (float)-0.25;

            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        /// <summary>
        /// Use to quickly grab value on right side of = in options/fix files
        /// </summary>
        /// <param name="raw_line">Raw line from the file</param>
        /// <returns></returns>
        public string GetConfigString(string raw_line)
        {
            if (string.IsNullOrWhiteSpace(raw_line)) return "";
            var line = raw_line;
            try
            {
                var index = line.IndexOf("=", StringComparison.Ordinal) + 1;
                line = line.Substring(index, line.Length - index);
            }
            catch (Exception)
            {
                line = "";
            }
            return line.Trim();
        }

        /// <summary>
        /// Will send files or folders to Recycle Bin rather than delete from hard drive
        /// </summary>
        /// <param name="path">Full file / folder path to be recycled</param>
        /// <param name="isFolder">Whether path is to a folder rather than a file</param>
        public void SendtoTrash(string path, bool isFolder = false)
        {
            if (isFolder)
            {
                if (!Directory.Exists(path)) return;
            }
            else
            {
                if (!File.Exists(path)) return;
            }

            try
            {
                var fileop = new SHFILEOPSTRUCT
                {
                    wFunc = FO_DELETE,
                    pFrom = path + '\0' + '\0',
                    fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION
                };
                SHFileOperation(ref fileop);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Will safely try to move, and if fails, copy/delete a file
        /// </summary>
        /// <param name="input">Full starting path of the file</param>
        /// <param name="output">Full destination path of the file</param>
        public bool MoveFile(string input, string output)
        {
            try
            {
                DeleteFile(output);
                File.Move(input, output);
            }
            catch (Exception)
            {
                try
                {
                    File.Copy(input, output);
                    DeleteFile(input);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return File.Exists(output);
        }

        /// <summary>
        /// Simple function to safely delete folders
        /// </summary>
        /// <param name="folder">Full path of folder to be deleted</param>
        /// <param name="delete_contents">Whether to delete folders that are not empty</param>
        public void DeleteFolder(string folder, bool delete_contents)
        {
            if (!Directory.Exists(folder)) return;
            try
            {
                if (delete_contents)
                {
                    Directory.Delete(folder, true);
                    return;
                }
                if (!Directory.GetFiles(folder).Any())
                {
                    Directory.Delete(folder);
                }
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Simple function to safely delete files
        /// </summary>
        /// <param name="file">Full path of file to be deleted</param>
        public void DeleteFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return;
            if (!File.Exists(file)) return;
            try
            {
                File.Delete(file);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Simple function to safely delete folders
        /// </summary>
        /// <param name="folder">Full path of folder to be deleted</param>
        public void DeleteFolder(string folder)
        {
            if (!Directory.Exists(folder)) return;
            DeleteFolder(folder, false);
        }

        /// <summary>
        /// Use to send mogg to Audacity for editing
        /// </summary>
        /// <param name="mogg">Full file path to audio file</param>
        /// <returns>String message with either success or failure messages for logging/displaying</returns>
        public string SendtoAudacity(string mogg)
        {
            var nautilus = new nTools();
            var mData = File.ReadAllBytes(mogg);
            if (mData[0] != 0x0A)
            {
                nautilus.WriteOutData(mData, mogg);
                return "Audio file " + Path.GetFileName(mogg) + " is encrypted and can't be sent to Audacity, sorry";
            }
            try
            {
                var audacity = "";
                if (File.Exists(Application.StartupPath + "\\bin\\audacity.nautilus"))
                {
                    var sr = new StreamReader(Application.StartupPath + "\\bin\\audacity.nautilus", Encoding.Default);
                    audacity = sr.ReadLine();
                    sr.Dispose();
                }
                if (string.IsNullOrWhiteSpace(audacity) || !File.Exists(audacity) || !audacity.EndsWith(".exe", StringComparison.Ordinal))
                {
                    if (File.Exists("C:\\Program Files (x86)\\Audacity\\audacity.exe"))
                    {
                        audacity = "C:\\Program Files (x86)\\Audacity\\audacity.exe";
                    }
                    else if (File.Exists("C:\\Program Files\\Audacity\\audacity.exe"))
                    {
                        audacity = "C:\\Program Files\\Audacity\\audacity.exe";
                    }
                    else
                    {
                        var openfile = new OpenFileDialog
                        {
                            Filter = "Windows Executable (*.exe)|*.exe",
                            InitialDirectory = Application.StartupPath,
                            Title = "Select Audacity executable",
                            FileName = "audacity"
                        };
                        openfile.ShowDialog();
                        if (openfile.FileName != "")
                        {
                            audacity = openfile.FileName;
                            var sw = new StreamWriter(Application.StartupPath + "\\audacity.nautilus", false, Encoding.Default);
                            sw.WriteLine(audacity);
                            sw.Dispose();
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(audacity) || !File.Exists(audacity) || !audacity.EndsWith(".exe", StringComparison.Ordinal))
                {
                    return "Could not find Audacity executable\nProcess aborted";
                }
                CurrentFolder = Path.GetDirectoryName(mogg) + "\\";
                var arg = "\"" + mogg + "\"";
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    FileName = audacity,
                    Arguments = arg,
                    WorkingDirectory = Path.GetDirectoryName(mogg) + "\\"
                };
                var process = Process.Start(startInfo);
                do
                {
                    //wait
                } while (!process.HasExited);
                process.Dispose();
                return "Audacity process completed successfully";
            }
            catch (Exception ex)
            {
                return "There was an error while sending to Audacity\nThe error says: " + ex.Message;
            }
        }

        /// <summary>
        /// Unlocks STFS package to show as a full song in game
        /// </summary>
        /// <param name="file_path">Full path to the CON or LIVE file</param>
        /// <returns></returns>
        public bool UnlockCON(string file_path)
        {
            //open and unlock CON/LIVE package
            try
            {
                var bw = new BinaryWriter(File.Open(file_path, FileMode.Open, FileAccess.ReadWrite));
                bw.BaseStream.Seek(567L, SeekOrigin.Begin);
                bw.Write(0x01);
                bw.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Signs STFS file as CON for use in retail Xboxes
        /// </summary>
        /// <param name="file_path">Full path to the STFS file to sign</param>
        /// <returns></returns>
        public bool SignCON(string file_path)
        {
            var xPackage = new STFSPackage(file_path);
            if (!xPackage.ParseSuccess)
            {
                return false;
            }
            try
            {
                var kv = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
                if (kv.Valid)
                {
                    xPackage.FlushPackage(kv);
                    xPackage.UpdateHeader(kv);
                    xPackage.CloseIO();
                    return true;
                }
                xPackage.CloseIO();
                return false;
            }
            catch
            {
                xPackage.CloseIO();
                return false;
            }
        }

        /// <summary>
        /// Returns string with correctly formatted characters
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <returns></returns>
        public string FixBadChars(string raw_line)
        {
            var line = raw_line.Replace("Ã¡", "á");
            line = line.Replace("Ã©", "é");
            line = line.Replace("Ã¨", "è");
            line = line.Replace("ÃŠ", "Ê");
            line = line.Replace("Ã¬", "ì");
            line = line.Replace("Ã­­­", "í");
            line = line.Replace("Ã¯", "ï");
            line = line.Replace("Ã–", "Ö");
            line = line.Replace("Ã¶", "ö");
            line = line.Replace("Ã³", "ó");
            line = line.Replace("Ã²", "ò");
            line = line.Replace("Ãœ", "Ü");
            line = line.Replace("Ã¼", "ü");
            line = line.Replace("Ã¹", "ù");
            line = line.Replace("Ãº", "ú");
            line = line.Replace("Ã¿", "ÿ");
            line = line.Replace("Ã±", "ñ");
            line = line.Replace("ï¿½", "");
            line = line.Replace("�", "");
            line = line.Replace("E½", "");
            return line;
        }

        /// <summary>
        /// Returns byte array in hex value
        /// </summary>
        /// <param name="xIn">String value to be converted to hex</param>
        /// <returns></returns>
        public byte[] ToHex(string xIn)
        {
            for (var i = 0; i < (xIn.Length % 2); i++)
                xIn = "0" + xIn;
            var xReturn = new List<byte>();
            for (var i = 0; i < (xIn.Length / 2); i++)
                xReturn.Add(Convert.ToByte(xIn.Substring(i * 2, 2), 16));
            return xReturn.ToArray();
        }

        /// <summary>
        /// Returns true if the package description suggests a pack
        /// </summary>
        /// <param name="desc">Package description</param>
        /// <param name="disp">Package display</param>
        /// <returns></returns>
        public bool DescribesPack(string desc, string disp)
        {
            var description = desc.ToLowerInvariant();
            var display = disp.ToLowerInvariant();

            return (display.Contains("pro upgrade") || description.Contains("pro upgrade") ||
                   description.Contains("(pro)") || description.Contains("(upgrade)") ||
                   display.Contains("(pro)") || display.Contains("(upgrade)") ||
                   display.Contains("album") || description.Contains("album") ||
                   display.Contains("export") || description.Contains("export")) &&
                   !description.Contains("depacked") && !display.Contains("depacked");
        }

        /// <summary>
        /// Returns cleaned string for file names, etc
        /// </summary>
        /// <param name="raw_string">Raw string from the songs.dta file</param>
        /// <param name="removeDash">Whether to remove dashes from the string</param>
        /// <param name="DashForSlash">Whether to replace slashes with dashes</param>
        /// <returns></returns>
        public string CleanString(string raw_string, bool removeDash, bool DashForSlash = false)
        {
            var myString = raw_string;

            //remove forbidden characters if present
            myString = myString.Replace("\"", "");
            myString = myString.Replace(">", " ");
            myString = myString.Replace("<", " ");
            myString = myString.Replace(":", " ");
            myString = myString.Replace("|", " ");
            myString = myString.Replace("?", " ");
            myString = myString.Replace("*", " ");
            myString = myString.Replace("&#8217;", "'"); //Don't Speak
            myString = myString.Replace("   ", " ");
            myString = myString.Replace("  ", " ");
            myString = FixBadChars(myString).Trim();

            if (removeDash)
            {
                if (myString.Substring(0, 1) == "-") //if starts with -
                {
                    myString = myString.Substring(1, myString.Length - 1);
                }
                if (myString.Substring(myString.Length - 1, 1) == "-") //if ends with -
                {
                    myString = myString.Substring(0, myString.Length - 1);
                }

                myString = myString.Trim();
            }

            while (myString.EndsWith(".", StringComparison.Ordinal))
            {
                myString = myString.Substring(0, myString.Length - 1);
            }

            myString = myString.Replace("\\", DashForSlash && myString != "AC/DC" ? "-" : (myString != "AC/DC" ? " " : ""));
            myString = myString.Replace("/", DashForSlash && myString != "AC/DC" ? "-" : (myString != "AC/DC" ? " " : ""));

            return myString;
        }

        /// <summary>
        /// Exports log to text file
        /// </summary>
        /// <param name="FormName">Name of the form calling the function, names the log tex file</param>
        /// <param name="logItems">Items in the lstLog listbox</param>
        public void ExportLog(string FormName, ListBox.ObjectCollection logItems)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var sfd = new SaveFileDialog
            {
                InitialDirectory = desktop,
                Title = "Export log file",
                FileName = FormName.Replace(" ", "").Replace("*", "") + "_log",
                Filter = "Text File (*.txt)|*.txt",
                OverwritePrompt = true
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            var sw = new StreamWriter(sfd.FileName, false, Encoding.Default);

            foreach (var line in logItems)
            {
                sw.WriteLine(line);
            }
            sw.Dispose();
        }

        /// <summary>
        /// Creates RAR archive with highest compression setting
        /// </summary>
        /// <param name="rar_path">Full path to the RAR.exe file</param>
        /// <param name="archive_name">Name for the RAR archive to be created</param>
        /// <param name="arguments">Arguments required by RAR.exe</param>
        /// <returns></returns>
        public bool CreateRAR(string rar_path, string archive_name, string arguments)
        {
            try
            {
                DeleteFile(archive_name);

                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = rar_path,
                    Arguments = arguments,
                    WorkingDirectory = Application.StartupPath + "\\bin\\"
                };
                var process = Process.Start(startInfo);
                do
                {
                    //wait
                } while (!process.HasExited);
                process.Dispose();

                if (File.Exists(archive_name))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        #endregion

        #region MIDI Stuff

        /// <summary>
        /// Returns clean Track Name from midi event string
        /// </summary>
        /// <param name="raw_event">The raw ToString value of the midi event</param>
        /// <returns></returns>
        public string GetMidiTrackName(string raw_event)
        {
            var name = raw_event;
            name = name.Substring(2, name.Length - 2); //remove track number
            name = name.Replace("SequenceTrackName", "");
            return name.Trim();
        }

        /// <summary>
        /// Returns whether the midi has EMH charted by counting and comparing number of notes
        /// </summary>
        /// <param name="track">Midi track to process (i.e. drums, keys)</param>
        /// <param name="ExpertLow">Lower value cutoff for Expert Difficulty</param>
        /// <param name="ExpertHigh">Higher value cutoff for Expert Difficulty</param>
        /// <param name="HardLow">Lower value cutoff for Hard Difficulty</param>
        /// <param name="HardHigh">Higher value cutoff for Hard Difficulty</param>
        /// <param name="MediumLow">Lower value cutoff for Medium Difficulty</param>
        /// <param name="MediumHigh">Higher value cutoff for Medium Difficulty</param>
        /// <param name="EasyLow">Lower value cutoff for Easy Difficulty</param>
        /// <param name="EasyHigh">Higher value cutoff for Easy Difficulty</param>
        /// <param name="OnlyCheckForEMH">True if you only care whether EMH has ANYTHING charted</param>
        /// <returns></returns>
        private int CheckTrackForEMH(IList<MidiEvent> track, int ExpertLow, int ExpertHigh, int HardLow, int HardHigh, int MediumLow, int MediumHigh, int EasyLow, int EasyHigh, bool OnlyCheckForEMH = false)
        {
            var Expert = new List<MidiEvent>();
            var Hard = new List<MidiEvent>();
            var Medium = new List<MidiEvent>();
            var Easy = new List<MidiEvent>();
            var trackname = GetMidiTrackName(track[0].ToString());
            var x_only = 0;

            try
            {
                foreach (var notes in track)
                {
                    if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                    var note = (NoteOnEvent)notes;
                    if (note.NoteNumber >= ExpertLow && note.NoteNumber <= ExpertHigh)
                    {
                        Expert.Add(notes);
                    }
                    else if (note.NoteNumber >= HardLow && note.NoteNumber <= HardHigh)
                    {
                        Hard.Add(notes);
                    }
                    else if (note.NoteNumber >= MediumLow && note.NoteNumber <= MediumHigh)
                    {
                        Medium.Add(notes);
                    }
                    else if (note.NoteNumber >= EasyLow && note.NoteNumber <= EasyHigh)
                    {
                        Easy.Add(notes);
                    }
                }

                if (Hard.Count() >= Expert.Count() && !OnlyCheckForEMH)
                {
                    MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: " + trackname + " Hard has " + (!Hard.Any() ? "0 notes" : "the same amount or more notes than Expert");
                    x_only++;
                }
                else if (Hard.Count() < 10 && Expert.Count() > 10)
                {
                    MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: " + trackname + " Hard " + (Hard.Any() ? "only has " + Hard.Count() + " notes but Expert has " + Expert.Count + " notes" : "has 0 notes");
                    x_only++;
                }
                if (Medium.Count() >= Hard.Count() && !OnlyCheckForEMH)
                {
                    MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: " + trackname + " Medium has " + (!Medium.Any() ? "0 notes" : "the same amount or more notes than Hard");
                    x_only++;
                }
                else if (Medium.Count() < 10 && Expert.Count() > 10)
                {
                    MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: " + trackname + " Medium " + (Medium.Any() ? "only has " + Medium.Count() + " notes but Expert has " + Expert.Count + " notes" : "has 0 notes");
                    x_only++;
                }
                if (Easy.Count() >= Medium.Count() && !OnlyCheckForEMH)
                {
                    MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: " + trackname + " Easy has " + (!Easy.Any() ? "0 notes" : "the same amount or more notes than Medium");
                    x_only++;
                }
                else if (Easy.Count() < 10 && Expert.Count() > 10)
                {
                    MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: " + trackname + " Easy " + (Easy.Any() ? "only has " + Easy.Count() + " notes but Expert has " + Expert.Count + " notes" : "has 0 notes");
                    x_only++;
                }
            }
            catch (Exception)
            {
                MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nERROR reading MIDI file to check if " + trackname + " has lower difficulties";
            }

            return x_only;
        }

        public MidiFile NemoLoadMIDI(string midi_in)
        {
            //NAudio is limited in its ability to read some non-standard MIDIs
            //before this step was added, 3 different errors would prevent this program from reading
            //MIDIs with those situations
            //thanks raynebc we can fix them first and load the fixed MIDIs
            var midishrink = Application.StartupPath + "\\bin\\midishrink.exe";
            if (!File.Exists(midishrink)) return null;
            var midi_out = Application.StartupPath + "\\bin\\temp.mid";
            DeleteFile(midi_out);
            MidiFile MIDI;
            try
            {
                MIDI = new MidiFile(midi_in, false);
            }
            catch (Exception)
            {
                var folder = Path.GetDirectoryName(midi_in) ?? Environment.CurrentDirectory;
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = Application.StartupPath + "\\bin\\midishrink.exe",
                    Arguments = "\"" + midi_in + "\" \"" + midi_out + "\"",
                    WorkingDirectory = folder
                };
                var start = (DateTime.Now.Minute * 60) + DateTime.Now.Second;
                var process = Process.Start(startInfo);
                do
                {
                    //this code checks for possible memory leak in midishrink
                    //typical usage outputs a fixed file in 1 second or less, at 15 seconds there's a problem
                    if ((DateTime.Now.Minute * 60) + DateTime.Now.Second - start < 15) continue;
                    break;

                } while (!process.HasExited);
                if (!process.HasExited)
                {
                    process.Kill();
                    process.Dispose();
                }
                if (File.Exists(midi_out))
                {
                    try
                    {
                        MIDI = new MidiFile(midi_out, false);
                    }
                    catch (Exception)
                    {
                        MIDI = null;
                    }
                }
                else
                {
                    MIDI = null;
                }
            }
            DeleteFile(midi_out);  //the file created in the loop is useless, delete it
            return MIDI;
        }

        public bool DoesMidiHaveEMH(string midifile, bool pro_keys_enabled = true)
        {
            MIDI_ERROR_MESSAGE = "Starting Basic EMH Check";
            var Expert = new List<MidiEvent>();
            var Hard = new List<MidiEvent>();
            var Medium = new List<MidiEvent>();
            var Easy = new List<MidiEvent>();
            var x_only = 0;

            try
            {
                var songMidi = NemoLoadMIDI(midifile);
                if (songMidi == null)
                {
                    MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nERROR: Could not load MIDI file to check if charts have lower difficulties";
                    return false;
                }
                for (var i = 0; i < songMidi.Events.Tracks; i++)
                {
                    if (songMidi.Events[i][0].ToString().ToLowerInvariant().Contains("part drums"))
                    {
                        x_only = x_only + CheckTrackForEMH(songMidi.Events[i], 96, 100, 84, 88, 72, 76, 60, 64);
                    }
                    else if (songMidi.Events[i][0].ToString().ToLowerInvariant().Contains("part bass"))
                    {
                        x_only = x_only + CheckTrackForEMH(songMidi.Events[i], 96, 100, 84, 88, 72, 76, 60, 64);
                    }
                    else if (songMidi.Events[i][0].ToString().ToLowerInvariant().Contains("part guitar"))
                    {
                        x_only = x_only + CheckTrackForEMH(songMidi.Events[i], 96, 100, 84, 88, 72, 76, 60, 64);
                    }
                    else if (songMidi.Events[i][0].ToString().ToLowerInvariant().Contains("part keys") && !songMidi.Events[i][0].ToString().ToLowerInvariant().Contains("anim"))
                    {
                        x_only = x_only + CheckTrackForEMH(songMidi.Events[i], 96, 100, 84, 88, 72, 76, 60, 64);
                    }
                    else if (songMidi.Events[i][0].ToString().ToLowerInvariant().Contains("real_bass"))
                    {
                        x_only = x_only + CheckTrackForEMH(songMidi.Events[i], 96, 99, 72, 75, 48, 51, 24, 27);
                    }
                    else if (songMidi.Events[i][0].ToString().ToLowerInvariant().Contains("real_guitar"))
                    {
                        x_only = x_only + CheckTrackForEMH(songMidi.Events[i], 96, 101, 72, 77, 48, 53, 24, 29);
                    }
                    else if (songMidi.Events[i][0].ToString().ToLowerInvariant().Contains("real_keys") && pro_keys_enabled)
                    {
                        var track = GetMidiTrackName(songMidi.Events[i][0].ToString());
                        foreach (var notes in from notes in songMidi.Events[i] where notes.CommandCode == MidiCommandCode.NoteOn let note = (NoteOnEvent)notes where note.NoteNumber >= 48 && note.NoteNumber <= 72 select notes)
                        {
                            if (track.ToLowerInvariant().Contains("keys_x"))
                            {
                                Expert.Add(notes);
                            }
                            else if (track.ToLowerInvariant().Contains("keys_h"))
                            {
                                Hard.Add(notes);
                            }
                            else if (track.ToLowerInvariant().Contains("keys_m"))
                            {
                                Medium.Add(notes);
                            }
                            else if (track.ToLowerInvariant().Contains("keys_e"))
                            {
                                Easy.Add(notes);
                            }
                        }
                    }
                }

                if (Expert.Any())
                {
                    if (Hard.Count() >= Expert.Count())
                    {
                        MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: PART REAL_KEYS_H has " +
                                             (!Hard.Any() ? "0 notes" : "the same amount or more notes than Expert");
                        x_only++;
                    }
                    else if (Hard.Count() < 10 && Expert.Count() > 10)
                    {
                        MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: PART REAL_KEYS_H " +
                                             (Hard.Any() ? "only has " + Hard.Count() + " notes but Expert has " + Expert.Count +
                                                    " notes" : "has 0 notes");
                        x_only++;
                    }
                    if (Medium.Count() >= Hard.Count())
                    {
                        MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: PART REAL_KEYS_M has " +
                                             (!Medium.Any() ? "0 notes" : "the same amount or more notes than Hard");
                        x_only++;
                    }
                    else if (Medium.Count() < 10 && Expert.Count() > 10)
                    {
                        MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: PART REAL_KEYS_M " +
                                             (Medium.Any() ? "only has " + Medium.Count() + " notes but Expert has " +
                                                    Expert.Count + " notes" : "has 0 notes");
                        x_only++;
                    }
                    if (Easy.Count() >= Medium.Count())
                    {
                        MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: PART REAL_KEYS_E has " +
                                             (!Easy.Any() ? "0 notes" : "the same amount or more notes than Medium");
                        x_only++;
                    }
                    else if (Easy.Count() < 10 && Expert.Count() > 10)
                    {
                        MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nWARNING: PART REAL_KEYS_E " +
                                             (Easy.Any() ? "only has " + Easy.Count() + " notes but Expert has " + Expert.Count +
                                                    " notes" : "has 0 notes");
                        x_only++;
                    }
                }
            }
            catch (Exception)
            {
                MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nERROR: Could not load MIDI file to check if charts have lower difficulties";
                return false;
            }

            if (x_only == 0)
            {
                MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nMIDI passed Basic EMH Check without reporting any problems\nThis means the charts most likely have reductions charted";
                return true;
            }
            if (x_only > 0 && x_only < 4)
            {
                MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nThere " + (x_only == 1 ? "was" : "were") + " only " + x_only + (x_only == 1 ? " problem" : " problems") + " reported\nThis means the charts most likely have reductions charted except where reported in the log";
                return true;
            }
            MIDI_ERROR_MESSAGE = MIDI_ERROR_MESSAGE + "\nMIDI failed Basic EMH Check. This means the charts are most likely expert only\nRefer to the log to see which chart(s) reported problems";
            return false;
        }
        #endregion

        #region Savegame File Stuff

        public bool ReplaceSaveImages(string saveFile, string image_folder, bool isPS3 = false)//, int offsetDiff = 0)
        {
            if (!File.Exists(saveFile)) return false;

            var in_char = image_folder + "character_";
            var in_art = image_folder + "art_";
            var ext = isPS3 ? ".png_ps3" : ".png_xbox";
            int PS3_OFFSET = 0x74;// + offsetDiff;
            var char_offsets = image_folder + "char.offset";
            var art_offsets = image_folder + "art.offset";
            var name_offsets = image_folder + "names.offset";
            var band_offset = image_folder + "band.offset";
            List<long> char_offset_list = new List<long>();
            List<long> art_offset_list = new List<long>();
            List<long> name_offset_list = new List<long>();
            long actualBandOffset = 0;

            try
            {
                var sr = new StreamReader(char_offsets);
                for (var i = 0; i < 10; i++)
                {
                    char_offset_list.Add(Convert.ToInt64(sr.ReadLine()));
                }
                sr.Dispose();
                sr = new StreamReader(art_offsets);
                for (var i = 0; i < 19; i++)
                {
                    art_offset_list.Add(Convert.ToInt64(sr.ReadLine()));
                }
                sr.Dispose();
                sr = new StreamReader(name_offsets);
                for (var i = 0; i < 10; i++)
                {
                    var offset = Convert.ToInt64(sr.ReadLine());
                    if (offset > 0)
                    {
                        name_offset_list.Add(offset);
                    }
                }
                sr.Dispose();
                sr = new StreamReader(band_offset);
                actualBandOffset = Convert.ToInt64(sr.ReadLine());
                sr.Dispose();
            }
            catch
            {
                MessageBox.Show("Offset information not found or corrupted, can't proceed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (SaveFileCharNames.Count != name_offset_list.Count)
            {
                MessageBox.Show("Mismatch between character name count and character name offset count, can't proceed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {               
                //hack the save.dat file and replace inside the CON
                var datFile = saveFile;
                var tempDAT = Application.StartupPath + "\\bin\\temp.dat";
                var isCON = VariousFunctions.ReadFileType(saveFile) == XboxFileType.STFS;
                STFSPackage package = null;
                FileEntry xent = null;
                if (isCON)
                {
                    isPS3 = false;

                    package = new STFSPackage(saveFile);
                    if (!package.ParseSuccess)
                    {
                        package.CloseIO();
                        return false;
                    }
                    xent = package.GetFile("save.dat");
                    if (xent == null)
                    {
                        package.CloseIO();
                        return false;
                    }
                    DeleteFile(tempDAT);
                    if (!xent.ExtractToFile(tempDAT))
                    {
                        package.CloseIO();
                        return false;
                    }
                    datFile = tempDAT;
                }

                const int BAND_OFFSET = 0x43A773;
                const int BAND_LENGTH = 31; // Max string bytes
                const int BAND_META_OFFSET = BAND_OFFSET - 4;

                string bName = SaveFileBandName;
                if (bName.Length > BAND_LENGTH)
                {
                    bName = bName.Substring(0, BAND_LENGTH);
                }

                // Encode the string as bytes
                byte[] bandNameBytes = new UTF8Encoding(false, false).GetBytes(bName);

                // Create a zero-filled byte array of full length
                byte[] bandPaddedBytes = new byte[BAND_LENGTH];
                Array.Clear(bandPaddedBytes, 0, BAND_LENGTH);

                // Copy the encoded name into the padded array
                Array.Copy(bandNameBytes, 0, bandPaddedBytes, 0, bandNameBytes.Length);

                using (var bw3 = new BinaryWriter(File.Open(datFile, FileMode.Open, FileAccess.Write)))
                {
                    // Write actual length
                    //bw3.BaseStream.Seek((isPS3 ? BAND_META_OFFSET + PS3_OFFSET : BAND_META_OFFSET), SeekOrigin.Begin);
                    bw3.BaseStream.Seek(actualBandOffset - 4, SeekOrigin.Begin);
                    bw3.Write(BitConverter.GetBytes(bandNameBytes.Length));

                    // Write padded name with nulls instead of spaces
                    //bw3.BaseStream.Seek((isPS3 ? BAND_OFFSET + PS3_OFFSET : BAND_OFFSET), SeekOrigin.Begin);
                    bw3.BaseStream.Seek(actualBandOffset, SeekOrigin.Begin);
                    bw3.Write(bandPaddedBytes);
                }                

                int NAME_OFFSET = 0x4B1AB;
                const int NAME_SPACER = 0x20EDE;
                const int NAME_LENGTH = 24; // fixed-length name field (UTF-8 + nulls)

                using (var bw = new BinaryWriter(File.Open(datFile, FileMode.Open, FileAccess.Write)))
                {        
                    for (var c = 0; c < SaveFileCharNames.Count; c++)
                    {
                        var name = SaveFileCharNames[c];
                        string charName = name.Length > NAME_LENGTH ? name.Substring(0, NAME_LENGTH) : name;
                        byte[] nameBytes = new UTF8Encoding(false, false).GetBytes(charName);

                        // Build 24-byte null-padded buffer
                        byte[] paddedBytes = new byte[NAME_LENGTH];
                        Array.Clear(paddedBytes, 0, NAME_LENGTH);
                        Array.Copy(nameBytes, 0, paddedBytes, 0, Math.Min(nameBytes.Length, NAME_LENGTH));

                        // Compute write positions
                        long offset = Convert.ToInt64(name_offset_list[c]);
                        offset = isPS3 ? offset + PS3_OFFSET : offset;
                        long lengthOffset = offset - 4;

                        // Write name
                        bw.BaseStream.Seek(offset, SeekOrigin.Begin);
                        bw.Write(paddedBytes);

                        // Write 4-byte length
                        bw.BaseStream.Seek(lengthOffset, SeekOrigin.Begin);
                        bw.Write((byte)nameBytes.Count());

                        // Move to next entry
                        NAME_OFFSET += NAME_LENGTH + NAME_SPACER;
                    }
                    /*foreach (var name in SaveFileCharNames)
                    {
                        string charName = name.Length > NAME_LENGTH ? name.Substring(0, NAME_LENGTH) : name;
                        byte[] nameBytes = Encoding.UTF8.GetBytes(charName);

                        // Build 24-byte null-padded buffer
                        byte[] paddedBytes = new byte[NAME_LENGTH];
                        Array.Clear(paddedBytes, 0, NAME_LENGTH);
                        Array.Copy(nameBytes, 0, paddedBytes, 0, Math.Min(nameBytes.Length, NAME_LENGTH));

                        // Compute write positions
                        int offset = isPS3 ? NAME_OFFSET + PS3_OFFSET : NAME_OFFSET;
                        int lengthOffset = offset - 4;                                             

                        // Write name
                        bw.BaseStream.Seek(offset, SeekOrigin.Begin);
                        bw.Write(paddedBytes);

                        // Write 4-byte length
                        bw.BaseStream.Seek(lengthOffset, SeekOrigin.Begin);
                        bw.Write((byte)nameBytes.Count());

                        // Move to next entry
                        NAME_OFFSET += NAME_LENGTH + NAME_SPACER;
                    }*/
                }

                /*foreach (var name in SaveFileCharNames)
                {
                    // Truncate if needed
                    string charName = name.Length > NAME_LENGTH ? name.Substring(0, NAME_LENGTH) : name;

                    // Encode to UTF-8 bytes
                    byte[] nameBytes = Encoding.UTF8.GetBytes(charName);

                    // Create a null-padded byte array of fixed length
                    byte[] paddedBytes = new byte[NAME_LENGTH];
                    Array.Clear(paddedBytes, 0, NAME_LENGTH);
                    Array.Copy(nameBytes, 0, paddedBytes, 0, Math.Min(nameBytes.Length, NAME_LENGTH));

                    // Write length and padded name in one stream
                    using (var bw4 = new BinaryWriter(File.Open(datFile, FileMode.Open, FileAccess.Write)))
                    {
                        // Write name length (in bytes, not characters!)
                        bw4.BaseStream.Seek((isPS3 ? NAME_OFFSET + PS3_OFFSET : NAME_OFFSET) - 4, SeekOrigin.Begin);
                        bw4.Write(BitConverter.GetBytes(nameBytes.Length));

                        // Write null-padded name
                        bw4.BaseStream.Seek(isPS3 ? NAME_OFFSET + PS3_OFFSET : NAME_OFFSET, SeekOrigin.Begin);
                        bw4.Write(paddedBytes);
                    }

                    // Move to the next character name slot
                    NAME_OFFSET += NAME_LENGTH + NAME_SPACER;
                }*/

                //write all art images
                //Int32 ART_OFFSET = 0x19495B;
                const Int32 ART_SIZE = 0x10020;
                //const Int32 ART_SPACER = 0x214;
                using (var bw1 = new BinaryWriter(File.Open(datFile, FileMode.Open, FileAccess.Write)))
                {
                    for (var i = 0; i < 19; i++)
                    {
                        var file = in_art + (i + 1) + ext;
                        if (!File.Exists(file) || art_offset_list[i] == 0)
                        {
                            //ART_OFFSET += ART_SIZE + ART_SPACER;
                            continue;
                        }
                        bw1.BaseStream.Seek(art_offset_list[i], SeekOrigin.Begin);
                        bw1.Write(File.ReadAllBytes(file), 0, ART_SIZE);

                        /* original code
                        bw = new BinaryWriter(File.Open(datFile, FileMode.Open, FileAccess.Write));
                        bw.BaseStream.Seek(isPS3 ? ART_OFFSET + PS3_OFFSET : ART_OFFSET, SeekOrigin.Begin);
                        bw.Write(File.ReadAllBytes(file), 0, ART_SIZE);
                        bw.Close();

                        ART_OFFSET += ART_SIZE + ART_SPACER;*/
                    }
                }

                //write all character images
                //Int32 CHAR_OFFSET = 0x4C07D;
                //const Int32 CHAR_SPACER = 0xED6;
                const Int32 CHAR_SIZE = 0x20020;
                using (var bw2 = new BinaryWriter(File.Open(datFile, FileMode.Open, FileAccess.Write)))
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var file = in_char + (i + 1) + ext;
                        if (!File.Exists(file) || char_offset_list[i] == 0)
                        {
                            //CHAR_OFFSET += CHAR_SIZE + CHAR_SPACER;
                            continue;
                        }
                        bw2.BaseStream.Seek(char_offset_list[i], SeekOrigin.Begin);
                        bw2.Write(File.ReadAllBytes(file), 0, CHAR_SIZE);

                        /*original code
                        bw = new BinaryWriter(File.Open(datFile, FileMode.Open, FileAccess.Write));
                        bw.BaseStream.Seek(isPS3 ? CHAR_OFFSET + PS3_OFFSET : CHAR_OFFSET, SeekOrigin.Begin);
                        bw.Write(File.ReadAllBytes(file), 0, CHAR_SIZE);
                        bw.Close();

                        CHAR_OFFSET += CHAR_SIZE + CHAR_SPACER;*/
                    }
                }

                if (!isCON) return true;
                if (!File.Exists(datFile)) return false;

                if (!xent.Inject(datFile))
                {
                    package.CloseIO();
                    return false;
                }
                package.CloseIO();
                DeleteFile(datFile);
                SignCON(saveFile);

                const Int32 VALID_FILE_SIZE = 0x89E000;
                var validateSize = File.ReadAllBytes(saveFile);
                return validateSize.Length == VALID_FILE_SIZE;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes to save game file\n" + ex.Message, "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Detects if the application is running under WINE.
        /// </summary>
        public bool IsRunningOnWine()
        {
            // Primary check using WINE environment variables
            if (Environment.GetEnvironmentVariable("WINELOADER") != null ||
                Environment.GetEnvironmentVariable("WINEPREFIX") != null)
            {
                return true;
            }

            // Secondary check using Registry (fallback)
            try
            {
                var wineKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Wine");
                return wineKey != null;
            }
            catch
            {
                return false;
            }
        }

        public bool ExtractSaveImages(string saveFile, string savepath, bool isPS3 = false, int offsetDiff = 0)
        {
            if (!File.Exists(saveFile)) return false;

            var saveBytes = File.ReadAllBytes(saveFile);
            SaveFileCharNames = new List<string>();

            var outFolder = savepath.Replace(".dat", "") + "_extracted\\";
            if (!Directory.Exists(outFolder))
            {
                Directory.CreateDirectory(outFolder);
            }
            var char_offsets = outFolder + "char.offset";
            var art_offsets = outFolder + "art.offset";
            var names_offsets = outFolder + "names.offset";
            var band_offset = outFolder + "band.offset";
            DeleteFile(char_offsets);
            DeleteFile(art_offsets);
            DeleteFile(names_offsets);
            DeleteFile(band_offset);
            List<long> char_offset_list = new List<long>();
            List<long> art_offset_list = new List<long>();
            List<long> name_offset_list = new List<long>();
            long actualBandOffset = 0;

            try
            {
                const int BAND_OFFSET = 0x43A773;
                const int BAND_LENGTH = 31;
                int PS3_OFFSET = 0x74;// + offsetDiff;
                var ext = isPS3 ? ".png_ps3" : ".png_xbox";

                //get band name from save file
                SaveFileBandName = "";
                var nameStream = new MemoryStream(saveBytes, isPS3 ? BAND_OFFSET + PS3_OFFSET : BAND_OFFSET, BAND_LENGTH);
                var nameBytes = new byte[BAND_LENGTH];
                nameStream.Read(nameBytes, 0, BAND_LENGTH);
                nameStream.Dispose();
                SaveFileBandName = new UTF8Encoding(false, false).GetString(nameBytes).Replace("\0", "").Trim();
                actualBandOffset = (isPS3 ? BAND_OFFSET + PS3_OFFSET : BAND_OFFSET) + (nameBytes[0] == 0x00 ? 1 : 0); //to account for shifted PS3 files

                //get character names
                var NAME_OFFSET = 0x4B1AB;
                const int NAME_SPACER = 0x20EDE;
                const int NAME_LENGTH = 24;
                for (var i = 0; i < 10; i++)
                {
                    nameStream = new MemoryStream(saveBytes, isPS3 ? NAME_OFFSET + PS3_OFFSET : NAME_OFFSET, NAME_LENGTH);
                    nameBytes = new byte[NAME_LENGTH];
                    nameStream.Read(nameBytes, 0, NAME_LENGTH);
                    nameStream.Dispose();
                    var name = new UTF8Encoding(false, false).GetString(nameBytes).Replace("\0","").Trim();
                                                            
                    if (string.IsNullOrWhiteSpace(name)) break;

                    name_offset_list.Add(nameBytes[0] == 0x00 ? NAME_OFFSET + 1 : NAME_OFFSET);//to account for shifted PS3 values
                    SaveFileCharNames.Add(name);
                    NAME_OFFSET += NAME_LENGTH + NAME_SPACER;
                }

                //grab character images
                //var CHAR_OFFSET = 0x4C07D;
                const int CHAR_SIZE = 0x20020;
                //const int CHAR_SPACER = 0xED6;
                var CHAR_HEADER = new byte[]
                {
                    0x01, 0x08, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x00
                };
                //recursive find to fix issues with things being out of expected place
                var CHAR_COUNTER = 0;
                var OUTPUT_CHAR = outFolder + "character_";

                int saveLength = saveBytes.Length;
                for (int z = 0; z <= saveLength - CHAR_SIZE; z++)
                {
                    // Fast slice without memory allocations
                    if (!saveBytes.AsSpan(z, CHAR_HEADER.Length).SequenceEqual(CHAR_HEADER))
                        continue;

                    char_offset_list.Add(z);

                    // Copy only when matched
                    byte[] imgBytes = new byte[CHAR_SIZE];
                    Array.Copy(saveBytes, z, imgBytes, 0, CHAR_SIZE);

                    CHAR_COUNTER++;
                    string outfile = OUTPUT_CHAR + CHAR_COUNTER + ext;

                    DeleteFile(outfile);
                    File.WriteAllBytes(outfile, imgBytes);
                    ConvertRBImage(outfile);
                }

                /* original code
                for (var i = 0; i < 10; i++)
                {
                    var saveStream = new MemoryStream(saveBytes, isPS3 ? CHAR_OFFSET + PS3_OFFSET : CHAR_OFFSET, CHAR_HEADER.Length);
                    var checkBytes = new byte[CHAR_HEADER.Length];
                    saveStream.Read(checkBytes, 0, CHAR_HEADER.Length);
                    saveStream.Dispose();

                    if (!checkBytes.SequenceEqual(CHAR_HEADER))
                    {
                        CHAR_OFFSET += CHAR_SIZE + CHAR_SPACER;
                        continue;
                    }

                    var imgStream = new MemoryStream(saveBytes, isPS3 ? CHAR_OFFSET + PS3_OFFSET : CHAR_OFFSET, CHAR_SIZE);
                    var imgBytes = new byte[CHAR_SIZE];
                    imgStream.Read(imgBytes, 0, CHAR_SIZE);
                    imgStream.Dispose();

                    CHAR_OFFSET += CHAR_SIZE + CHAR_SPACER;
                    CHAR_COUNTER++;
                    var outfile = OUTPUT_CHAR + CHAR_COUNTER + ext;
                    DeleteFile(outfile);
                    File.WriteAllBytes(outfile, imgBytes);
                    ConvertRBImage(outfile);
                }*/

                //grab art images
                //var ART_OFFSET = 0x19495B;
                const int ART_SIZE = 0x10020;
                //const int ART_SPACER = 0x214;
                var ART_HEADER = new byte[]
                {
                    0x01, 0x08, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00
                };

                //recursive find to fix issues with things being out of expected place
                var ART_COUNTER = 0;
                var OUTPUT_ART = outFolder + "art_";

                for (int z = 0; z <= saveLength - ART_SIZE; z++)
                {
                    // Fast slice without memory allocations
                    if (!saveBytes.AsSpan(z, ART_HEADER.Length).SequenceEqual(ART_HEADER))
                        continue;

                    art_offset_list.Add(z);

                    // Copy only when matched
                    byte[] imgBytes = new byte[ART_SIZE];
                    Array.Copy(saveBytes, z, imgBytes, 0, ART_SIZE);

                    ART_COUNTER++;
                    string outfile = OUTPUT_ART + ART_COUNTER + ext;

                    DeleteFile(outfile);
                    File.WriteAllBytes(outfile, imgBytes);
                    ConvertRBImage(outfile);
                }

                /* original code
                for (var i = 0; i < 19; i++)
                { 
                    var save_stream = new MemoryStream(saveBytes, isPS3 ? ART_OFFSET + PS3_OFFSET : ART_OFFSET, ART_HEADER.Length);
                    var check_bytes = new byte[ART_HEADER.Length];
                    save_stream.Read(check_bytes, 0, ART_HEADER.Length);
                    save_stream.Dispose();

                    if (!check_bytes.SequenceEqual(ART_HEADER))
                    {
                        ART_OFFSET += ART_SIZE + ART_SPACER;
                        continue;
                    }

                    var imgStream = new MemoryStream(saveBytes, isPS3 ? ART_OFFSET + PS3_OFFSET : ART_OFFSET, ART_SIZE);
                    var imgBytes = new byte[ART_SIZE];
                    imgStream.Read(imgBytes, 0, ART_SIZE);
                    imgStream.Dispose();

                    ART_OFFSET += ART_SIZE + ART_SPACER;
                    ART_COUNTER++;
                    var outFile = OUTPUT_ART + ART_COUNTER + ext;
                    DeleteFile(outFile);
                    File.WriteAllBytes(outFile, imgBytes);
                    ConvertRBImage(outFile);
                }*/

                var success = ART_COUNTER != 0 || CHAR_COUNTER != 0;
                if (success)
                {
                    //save offsets to file for reference when saving changes
                    var sw = new StreamWriter(char_offsets, false);
                    foreach (var offset in char_offset_list)
                    {
                        sw.WriteLine(offset);
                    }
                    sw.Dispose();
                    sw = new StreamWriter(art_offsets, false);
                    foreach (var offset in art_offset_list)
                    {
                        sw.WriteLine(offset);
                    }
                    sw.Dispose();
                    sw = new StreamWriter(names_offsets, false);
                    foreach (var offset in name_offset_list)
                    {
                        sw.WriteLine(offset);
                    }
                    sw.Dispose();
                    sw = new StreamWriter(band_offset, false);
                    sw.WriteLine(actualBandOffset);
                    sw.Dispose();
                    return true;
                }
                DeleteFolder(outFolder, true);
                MessageBox.Show("No character or art images found in that save game file\nThis tool only allows you to edit existing images,\nso try again after you've created characters or art in game",
                    "Save File Image Editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            catch (Exception)
            {
                DeleteFolder(outFolder, true);
                return false;
            }
        }

        public bool ExtractWiiSaveImages(string savefile, string savepath)
        {
            //I won't lie, treating the Wii like a red-headed stepchild here. too crappy to dedicate much more attention
            //maybe you can fine tune it?

            if (!File.Exists(savefile)) return false;

            SaveFileBandName = "";
            var savebytes = File.ReadAllBytes(savefile);
            const int CHAR_SIZE = 0x4020;
            const int ART_SIZE = 0x10020;
            var char_header = new byte[]
                {
                    0x01, 0x04, 0x48, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x01, 0x40, 0x00, 0x00, 0x00, 0x00
                };
            var art_header = new byte[]
                {
                    0x01, 0x20, 0x40, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x80, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00
                };

            SaveFileCharNames = new List<string>();
            //var out_folder = savepath.Replace(".dat", "") + "_extracted\\";
            if (!Directory.Exists(savepath))
            {
                Directory.CreateDirectory(savepath);
            }
            var out_char = Path.Combine(savepath, "character_");
            var char_counter = 0;
            var out_art = Path.Combine(savepath, "art_");
            var art_counter = 0;

            try
            {
                //get band name from save file
                //wii sucks, the offset varies wildly based on which "slot" the game is saved to
                //this will search all four slots for "a" name, but it's impossible to know
                //which is the correct name since we don't know whose slot we're looking at
                SaveFileBandName = "";
                var band_offsets = new List<int> { 0x157BCB, 0x297BCB, 0x3D7BCB, 0x517BCB };
                foreach (var bandOffset in band_offsets)
                {
                    var name_stream = new MemoryStream(savebytes, bandOffset, 24);
                    var name_bytes = new byte[24];
                    name_stream.Read(name_bytes, 0, 24);
                    name_stream.Dispose();
                    SaveFileBandName = new UTF8Encoding(false, false).GetString(name_bytes).Replace("\0", "").Trim();

                    //if there's no band name, continue, otherwise stop here
                    if (SaveFileBandName != "") break;
                }

                for (var i = 0; i < savebytes.Length - 16; i++)
                {
                    var save_stream = new MemoryStream(savebytes, i, 16);
                    var check_bytes = new byte[16];
                    save_stream.Read(check_bytes, 0, 16);
                    save_stream.Dispose();

                    if (check_bytes.SequenceEqual(char_header))
                    {
                        char_counter++;

                        //grab character image
                        var img = new MemoryStream(savebytes, i, CHAR_SIZE);
                        var imgbytes = new byte[CHAR_SIZE];
                        img.Read(imgbytes, 0, CHAR_SIZE);
                        img.Dispose();

                        var outfile = out_char + char_counter + ".png_wii";
                        DeleteFile(outfile);
                        File.WriteAllBytes(outfile, imgbytes);
                        ConvertWiiImage(outfile);

                        //grab character name
                        const int name_offset = 0xED2;
                        var name_stream = new MemoryStream(savebytes, i - name_offset, 24);
                        var name_bytes = new byte[24];
                        name_stream.Read(name_bytes, 0, 24);
                        name_stream.Dispose();

                        var name = new UTF8Encoding(false, false).GetString(name_bytes).Replace("\0", "").Trim();

                        i = i + CHAR_SIZE;

                        if (string.IsNullOrWhiteSpace(name)) break;
                        SaveFileCharNames.Add(name);
                    }
                    else if (check_bytes.SequenceEqual(art_header))
                    {
                        art_counter++;

                        //grab art image
                        var img = new MemoryStream(savebytes, i, ART_SIZE);
                        var imgbytes = new byte[ART_SIZE];
                        img.Read(imgbytes, 0, ART_SIZE);
                        img.Dispose();

                        var outfile = out_art + art_counter + ".png_wii";
                        DeleteFile(outfile);
                        File.WriteAllBytes(outfile, imgbytes);
                        ConvertWiiImage(outfile);
                        i = i + ART_SIZE;
                    }
                }

                var success = char_counter != 0 || art_counter != 0;
                if (!success)
                {
                    DeleteFolder(savepath, true);
                }
                return success;
            }
            catch (Exception)
            {
                DeleteFolder(savepath, true);
                return false;
            }
        }
        #endregion        

        public bool ExtractPsArc(string inFile, string outFolder, string rsFolder)
        {
            var path = Application.StartupPath + "\\bin\\rs2014\\";
            if (!File.Exists(path + "packer.exe"))
            {
                MessageBox.Show("packer.exe is missing from the bin\\rs2014 directory and I can't continue without it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                var arg = " -u -v=RS2014 -f=PC -d=false -i=\"" + inFile + "\" -o=\"" + outFolder + "\"";
                var app = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    FileName = path + "packer.exe",
                    Arguments = arg,
                    WorkingDirectory = path
                };
                var process = Process.Start(app);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting package:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return Directory.Exists(rsFolder);
        }

        private (byte[] header, int[] values) CreateYARGHeader()
        {
            byte[] header = new byte[24];

            // Add the YARGSONG signature
            Array.Copy(new byte[] { (byte)'Y', (byte)'A', (byte)'R', (byte)'G', (byte)'S', (byte)'O', (byte)'N', (byte)'G' }, header, 8);

            // Generate a random 'z' value as an int
            Random random = new Random();
            int z = random.Next(0, 256); // Start with a byte-sized value for the header

            // Write only the lower byte of z to the header
            header[8] = (byte)z;

            // Generate a random 15-byte cipher set
            byte[] set = new byte[15];
            random.NextBytes(set);
            Array.Copy(set, 0, header, 9, 15);

            // Calculate _values
            int[] values = CalculateYARGValues(z, set);

            return (header, values);
        }

        private int[] CalculateYARGValues(int z, byte[] set)
        {
            if (set.Length != 15)
                throw new ArgumentException("Set must be exactly 15 bytes long.");

            // Process z as an integer
            z += 1679; // Large prime number
            int w = (z ^ 4) - z * 2;
            int n = 25 * w - 5;
            int x = (w + (z << 1)) ^ 4;

            // Add the missing "infinite summation" logic
            int l = (n + 73) * (n + 23);
            l -= n * n + 96 * n;
            x = -l + n + x - w * (5 * 5);

            x = (x + 5) % 255; // Ensure x fits in a byte range

            // Calculate values array
            int[] values = new int[4];
            unchecked
            {
                for (int i = 0, j = 0; i < 24; i++, j += x)
                {
                    values[0] += (byte)(set[j % 15] + i * 3298 + 88903);
                    values[1] -= set[(j + 7001) % 15];
                    values[2] += set[j % 15];
                    values[3] += j << 2;
                }
            }

            return values;
        }

        public void EncryptYARG(byte[] buffer, int offset, int count, int[] values)
        {
            int pos = 0;
            unchecked
            {
                int a = values[0];
                int b = values[1];
                int c = values[2];

                for (int i = 0; i < count; i++)
                {
                    buffer[offset + i] = (byte)(((buffer[offset + i] ^ a) + ((i + pos) * c) + b));
                }
            }
        }

        public bool EncryptSNGtoYargSong(string sngFile)
        {
            var outFile = sngFile.Replace(".sng", ".yargsong");
            using (var outputStream = new FileStream(outFile, FileMode.Create))
            {
                // Generate the header and values
                var (header, values) = CreateYARGHeader();
                outputStream.Write(header, 0, header.Length);

                // Encrypt and write the file data
                byte[] buffer = new byte[4096];
                int bytesRead;

                using (var inputStream = new FileStream(sngFile, FileMode.Open))
                {
                    while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        EncryptYARG(buffer, 0, bytesRead, values);
                        outputStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            File.Delete(sngFile);
            return File.Exists(outFile);
        }

        public async Task<bool> DecryptExtractYARGSONG(string inFile, string outFolder)
        {
                await ExtractSNG(inFile, outFolder);
                return Directory.Exists(outFolder) && Directory.GetFiles(outFolder, "*.*", SearchOption.TopDirectoryOnly).Any();            
        }

        public async Task PackSNG(string inFolder, string outFolder)
        {
            try
            {
                await SngEncode.EncodeSng(inFolder, outFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error encoding input folder to SNG file:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }       

        public async Task ExtractSNG(string filePath, string outFolder)
        {
            try
            {
                if (Path.GetExtension(filePath) == ".yargsong")
                {
                    byte[] SNGPKG = { (byte)'S', (byte)'N', (byte)'G', (byte)'P', (byte)'K', (byte)'G' };

                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        // Load YARG file into memory
                        YARGSongFileStream yargFileStream = TryLoad(fileStream);
                        byte[] sngBytes = new byte[yargFileStream.Length];
                        yargFileStream.Read(sngBytes, 0, sngBytes.Length);
                        yargFileStream.Close();

                        // ✅ Overwrite the first bytes of sngBytes with SNGPKG (exactly like your temp file method)
                        Buffer.BlockCopy(SNGPKG, 0, sngBytes, 0, SNGPKG.Length);

                        // Process directly in memory                        
                        using (var ms = new MemoryStream(sngBytes))
                        {
                            await SngDecode.DecodeSng(ms, outFolder);
                        }
                    }
                }
                else
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, FileOptions.Asynchronous))
                    {
                        await SngDecode.DecodeSng(fs, outFolder); // ✅ Pass FileStream instead of byte[]
                    } 
                }                
            }
            catch (Exception ex)
            {
                var error = $"Error decoding that file\nError:\n{ex.Message}\nStack Trace:\n{ex.StackTrace}";
                Clipboard.SetText(error);
                MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }       

        public bool ExtractPKG(string pkg, string folder, out string klic)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "pkgripper.exe"))
            {
                MessageBox.Show("pkgripper.exe is missing from the bin directory and I can't continue without it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                klic = "";
                return false;
            }
            try
            {
                var arg = " -o \"" + folder + "\" \"" + pkg + "\"";
                var app = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    FileName = path + "pkgripper.exe",
                    Arguments = arg,
                    WorkingDirectory = path
                };
                var process = Process.Start(app);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting package:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                klic = "";
                return false;
            }

            var success = false;
            if (Directory.Exists(folder))
            {
                var dta = Directory.GetFiles(folder, "*.dta", SearchOption.AllDirectories);
                success = dta.Count() > 0;
            }

            folder += "\\USRDIR\\";
            var int_folder = Directory.GetDirectories(folder);
            klic = GetKLIC(int_folder[0]);
            return success;
        }

        public string GetKLIC(string folder)
        {
            var folder_value = Path.GetFileName(folder);
            var unhashed_klic = "Ih38rtW1ng3r" + folder_value + "10025250";
            return CreateMD5(unhashed_klic);
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }

        public bool DecryptEdat(string edat, string midi, string klic = "")
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "edattool.exe"))
            {
                MessageBox.Show("edattool.exe is missing from the bin directory and I can't continue without it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            const string c3_klic = "0B72B62DABA8CAFDA3352FF979C6D5C2";
            if (string.IsNullOrEmpty(klic))
            {
                klic = c3_klic;
            }
            try
            {
                var arg = " decrypt -custom:" + klic + " \"" + edat + "\" \"" + midi + "\"";
                var app = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    FileName = path + "edattool.exe",
                    Arguments = arg,
                    WorkingDirectory = path
                };
                var process = Process.Start(app);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error decrypting EDAT file:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return File.Exists(midi);
        }

        #region GHWT:DE Stuff - Most of the code by AddyMills
        private static byte[] FSB4 = new byte[] { 0x46, 0x53, 0x42, 0x34 };

        public bool fsbIsEncrypted(string fsb)
        {
            if (!File.Exists(fsb)) return false;
            using (var ms = new MemoryStream(File.ReadAllBytes(fsb)))
            {
                using (var br = new BinaryReader(ms))
                {
                    var fsb4 = br.ReadBytes(4);
                    return !fsb4.SequenceEqual(FSB4);
                }
            }            
        }

        /// <summary>
        /// Removes the first character from the file name if it starts with "adlc".
        /// </summary>
        /// <param name="fileName">The file name to be renamed.</param>
        /// <returns>The renamed file name.</returns>
        public static string FileRenamer(string fileName)
        {
            if (fileName.StartsWith("adlc"))
            {
                // Remove the first character
                fileName = fileName.Substring(1);
            }
            return fileName;
        }

        /// <summary>
        /// Flips the bits of the given audio.
        /// </summary>
        /// <param name="audio">The audio to flip the bits of.</param>
        /// <returns>The audio with flipped bits.</returns>
        public static byte[] FlipBits(byte[] audio)
        {
            byte[] result = new byte[audio.Length];
            for (int i = 0; i < audio.Length; i++)
            {
                result[i] = binaryReverse[audio[i]];
            }
            return result;
        }

        /// <summary>
        /// Performs the XOR operation between the audio and the key.
        /// </summary>
        /// <param name="audio">The audio to perform the XOR operation on.</param>
        /// <param name="key">The key to use for the XOR operation.</param>
        /// <returns>The result of the XOR operation.</returns>
        public static byte[] XorProcess(byte[] audio, byte[] key)
        {
            // Calculate the number of repetitions needed to match or exceed the length of the audio.
            int repetitions = 1 + (audio.Length / key.Length);

            // Create an array to hold the extended key.
            byte[] extendedKey = new byte[audio.Length];

            // Fill the extendedKey array with repeated copies of the key.
            for (int i = 0; i < repetitions; i++)
            {
                Array.Copy(key, 0, extendedKey, i * key.Length, Math.Min(key.Length, extendedKey.Length - (i * key.Length)));
            }

            // Create an array to hold the result of the XOR operation.
            byte[] result = new byte[audio.Length];

            // Perform the XOR operation between each byte of the audio and the extended key.
            for (int i = 0; i < audio.Length; i++)
            {
                result[i] = (byte)(audio[i] ^ extendedKey[i]);
            }

            return result;
        }

        /// <summary>
        /// Generates an FSB encryption key based on the provided string.
        /// </summary>
        /// <param name="toGen">The string used to generate the key.</param>
        /// <returns>The generated FSB key.</returns>
        public static byte[] GenerateFsbKey(string toGen)
        {
            uint xor = 0xffffffff;
            string encStr = "";
            const int cycle = 32;
            List<byte> key = new List<byte>();

            for (int i = 0; i < cycle; i++)
            {
                char ch = toGen[i % toGen.Length];
                uint crc = QBKeyUInt(new string(ch, 1));
                xor ^= crc;

                int index = (int)(xor % toGen.Length);
                encStr += toGen[index];
            }

            for (int i = 0; i < cycle - 1; i++)
            {
                char ch = encStr[i];
                uint crc = QBKeyUInt(new string(ch, 1));
                xor ^= crc;

                int c = i & 0x03;
                xor >>= c;

                uint z = 0; // Set to 0
                for (int x = 0; x < 32 - c; x++)
                {
                    z += (uint)(1 << x);
                }

                xor &= z;

                byte checkByte = (byte)(xor & 0xFF); // Equivalent to Python's hex(xor)[-2:],16

                if (checkByte == 0)
                {
                    break;
                }

                key.Add(checkByte);
            }

            return key.ToArray();
        }

        public static uint QBKeyUInt(string textBytes)
        {
            return ConvertHexToUInt(QBKey(textBytes));
        }
        private static uint ConvertHexToUInt(string hexString)
        {
            // Ensure the string starts with '0x' and is not longer than 10 characters.
            if (hexString.StartsWith("0x") && hexString.Length <= 10)
            {
                return Convert.ToUInt32(hexString, 16);
            }
            throw new ArgumentException("Invalid hex string: " + hexString);
        }

        public static string QBKey(string text)
        {
            if (text.StartsWith("0x") && text.Length <= 10)
            {
                return text;
            }
            text = text.ToLower().Replace("/", "\\");
            byte[] textBytes = new UTF8Encoding(false, false).GetBytes(text);

            return GenQBKey(textBytes);
        }

        public static string GenQBKey(byte[] textBytes)
        {
            uint crc = 0xffffffff;

            foreach (var b in textBytes)
            {
                uint numA = (crc ^ b) & 0xFF;
                crc = CRC32Table[numA] ^ crc >> 8 & 0x00ffffff;
            }

            uint finalCRC = ~crc;
            long value = -finalCRC - 1;
            string result = (value & 0xffffffff).ToString("x8");

            // Pad to 8 characters
            result = result.PadLeft(8, '0');
            result = "0x" + result;
            return result;
        }

        /// <summary>
        /// Decrypts the given audio using the provided key.
        /// </summary>
        /// <param name="audio">The audio to decrypt.</param>
        /// <param name="key">The key to use for decryption.</param>
        /// <returns>The decrypted audio.</returns>
        public static byte[] DecryptFsb4(byte[] audio, byte[] key)
        {
            var decrypted = FlipBits(audio);
            decrypted = XorProcess(decrypted, key);

            return decrypted;
        }

        /// <summary>
        /// Decrypts the given audio file.
        /// </summary>
        /// <param name="audio">The audio file to decrypt.</param>
        /// <param name="filename">The name of the audio file.</param>
        /// <returns>The decrypted audio.</returns>
        /// <exception cref="NotImplementedException">Thrown when the file type is not supported.</exception>
        public static byte[] DecryptFile(byte[] audio, string filename = "")
        {
            // Remove the extension and convert to lowercase. Sometimes there are two extensions which this hopefully covers.
            string noExt = FileRenamer(Path.GetFileNameWithoutExtension(filename).ToLower()).Replace(".fsb", "");
            byte[] key = GenerateFsbKey(noExt);
            return DecryptFsb4(audio, key);
        }

        /// <summary>
        /// Decrypts the audio file from the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the audio file.</param>
        /// <returns>The decrypted audio.</returns>
        public byte[] DecryptFSBFile(string filePath)
        {
            var audio = File.ReadAllBytes(filePath);
            return DecryptFile(audio, filePath);
        }

        public static Dictionary<byte, byte> binaryReverse = new Dictionary<byte, byte>
        {
            {0, 0},
            {1, 128},
            {2, 64},
            {3, 192},
            {4, 32},
            {5, 160},
            {6, 96},
            {7, 224},
            {8, 16},
            {9, 144},
            {10, 80},
            {11, 208},
            {12, 48},
            {13, 176},
            {14, 112},
            {15, 240},
            {16, 8},
            {17, 136},
            {18, 72},
            {19, 200},
            {20, 40},
            {21, 168},
            {22, 104},
            {23, 232},
            {24, 24},
            {25, 152},
            {26, 88},
            {27, 216},
            {28, 56},
            {29, 184},
            {30, 120},
            {31, 248},
            {32, 4},
            {33, 132},
            {34, 68},
            {35, 196},
            {36, 36},
            {37, 164},
            {38, 100},
            {39, 228},
            {40, 20},
            {41, 148},
            {42, 84},
            {43, 212},
            {44, 52},
            {45, 180},
            {46, 116},
            {47, 244},
            {48, 12},
            {49, 140},
            {50, 76},
            {51, 204},
            {52, 44},
            {53, 172},
            {54, 108},
            {55, 236},
            {56, 28},
            {57, 156},
            {58, 92},
            {59, 220},
            {60, 60},
            {61, 188},
            {62, 124},
            {63, 252},
            {64, 2},
            {65, 130},
            {66, 66},
            {67, 194},
            {68, 34},
            {69, 162},
            {70, 98},
            {71, 226},
            {72, 18},
            {73, 146},
            {74, 82},
            {75, 210},
            {76, 50},
            {77, 178},
            {78, 114},
            {79, 242},
            {80, 10},
            {81, 138},
            {82, 74},
            {83, 202},
            {84, 42},
            {85, 170},
            {86, 106},
            {87, 234},
            {88, 26},
            {89, 154},
            {90, 90},
            {91, 218},
            {92, 58},
            {93, 186},
            {94, 122},
            {95, 250},
            {96, 6},
            {97, 134},
            {98, 70},
            {99, 198},
            {100, 38},
            {101, 166},
            {102, 102},
            {103, 230},
            {104, 22},
            {105, 150},
            {106, 86},
            {107, 214},
            {108, 54},
            {109, 182},
            {110, 118},
            {111, 246},
            {112, 14},
            {113, 142},
            {114, 78},
            {115, 206},
            {116, 46},
            {117, 174},
            {118, 110},
            {119, 238},
            {120, 30},
            {121, 158},
            {122, 94},
            {123, 222},
            {124, 62},
            {125, 190},
            {126, 126},
            {127, 254},
            {128, 1},
            {129, 129},
            {130, 65},
            {131, 193},
            {132, 33},
            {133, 161},
            {134, 97},
            {135, 225},
            {136, 17},
            {137, 145},
            {138, 81},
            {139, 209},
            {140, 49},
            {141, 177},
            {142, 113},
            {143, 241},
            {144, 9},
            {145, 137},
            {146, 73},
            {147, 201},
            {148, 41},
            {149, 169},
            {150, 105},
            {151, 233},
            {152, 25},
            {153, 153},
            {154, 89},
            {155, 217},
            {156, 57},
            {157, 185},
            {158, 121},
            {159, 249},
            {160, 5},
            {161, 133},
            {162, 69},
            {163, 197},
            {164, 37},
            {165, 165},
            {166, 101},
            {167, 229},
            {168, 21},
            {169, 149},
            {170, 85},
            {171, 213},
            {172, 53},
            {173, 181},
            {174, 117},
            {175, 245},
            {176, 13},
            {177, 141},
            {178, 77},
            {179, 205},
            {180, 45},
            {181, 173},
            {182, 109},
            {183, 237},
            {184, 29},
            {185, 157},
            {186, 93},
            {187, 221},
            {188, 61},
            {189, 189},
            {190, 125},
            {191, 253},
            {192, 3},
            {193, 131},
            {194, 67},
            {195, 195},
            {196, 35},
            {197, 163},
            {198, 99},
            {199, 227},
            {200, 19},
            {201, 147},
            {202, 83},
            {203, 211},
            {204, 51},
            {205, 179},
            {206, 115},
            {207, 243},
            {208, 11},
            {209, 139},
            {210, 75},
            {211, 203},
            {212, 43},
            {213, 171},
            {214, 107},
            {215, 235},
            {216, 27},
            {217, 155},
            {218, 91},
            {219, 219},
            {220, 59},
            {221, 187},
            {222, 123},
            {223, 251},
            {224, 7},
            {225, 135},
            {226, 71},
            {227, 199},
            {228, 39},
            {229, 167},
            {230, 103},
            {231, 231},
            {232, 23},
            {233, 151},
            {234, 87},
            {235, 215},
            {236, 55},
            {237, 183},
            {238, 119},
            {239, 247},
            {240, 15},
            {241, 143},
            {242, 79},
            {243, 207},
            {244, 47},
            {245, 175},
            {246, 111},
            {247, 239},
            {248, 31},
            {249, 159},
            {250, 95},
            {251, 223},
            {252, 63},
            {253, 191},
            {254, 127},
            {255, 255}
        };


        private static readonly uint[] CRC32Table = new uint[]{
    0x00000000, 0x77073096, 0xee0e612c, 0x990951ba,
    0x076dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,
    0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988,
    0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,
    0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de,
    0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
    0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec,
    0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,
    0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172,
    0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
    0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940,
    0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
    0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116,
    0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,
    0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
    0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,
    0x76dc4190, 0x01db7106, 0x98d220bc, 0xefd5102a,
    0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433,
    0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818,
    0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01,
    0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e,
    0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,
    0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c,
    0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
    0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2,
    0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,
    0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0,
    0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,
    0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086,
    0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
    0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4,
    0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,
    0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a,
    0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683,
    0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8,
    0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
    0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe,
    0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,
    0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc,
    0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
    0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252,
    0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
    0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60,
    0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,
    0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
    0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,
    0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04,
    0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
    0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a,
    0x9c0906a9, 0xeb0e363f, 0x72076785, 0x05005713,
    0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38,
    0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21,
    0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e,
    0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
    0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c,
    0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
    0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2,
    0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,
    0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0,
    0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
    0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6,
    0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,
    0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94,
    0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
    };

        #endregion
    }

        public class FolderPicker
        {
            private readonly List<string> _resultPaths = new List<string>();
            private readonly List<string> _resultNames = new List<string>();

            public IReadOnlyList<string> ResultPaths => _resultPaths;
            public IReadOnlyList<string> ResultNames => _resultNames;
            public string ResultPath => ResultPaths.FirstOrDefault();
            public string ResultName => ResultNames.FirstOrDefault();
            public virtual string InputPath { get; set; }
            public virtual bool ForceFileSystem { get; set; }
            public virtual bool Multiselect { get; set; }
            public virtual string Title { get; set; }
            public virtual string OkButtonLabel { get; set; }
            public virtual string FileNameLabel { get; set; }

            protected virtual int SetOptions(int options)
            {
                if (ForceFileSystem)
                {
                    options |= (int)FOS.FOS_FORCEFILESYSTEM;
                }

                if (Multiselect)
                {
                    options |= (int)FOS.FOS_ALLOWMULTISELECT;
                }
                return options;
            }

            // for all .NET
            public virtual bool? ShowDialog(IntPtr owner, bool throwOnError = false)
            {
                var dialog = (IFileOpenDialog)new FileOpenDialog();
                if (!string.IsNullOrEmpty(InputPath))
                {
                    if (CheckHr(SHCreateItemFromParsingName(InputPath, null, typeof(IShellItem).GUID, out var item), throwOnError) != 0)
                        return null;

                    dialog.SetFolder(item);
                }

                var options = FOS.FOS_PICKFOLDERS;
                options = (FOS)SetOptions((int)options);
                dialog.SetOptions(options);

                if (Title != null)
                {
                    dialog.SetTitle(Title);
                }

                if (OkButtonLabel != null)
                {
                    dialog.SetOkButtonLabel(OkButtonLabel);
                }

                if (FileNameLabel != null)
                {
                    dialog.SetFileName(FileNameLabel);
                }

                if (owner == IntPtr.Zero)
                {
                    owner = Process.GetCurrentProcess().MainWindowHandle;
                    if (owner == IntPtr.Zero)
                    {
                        owner = GetDesktopWindow();
                    }
                }

                var hr = dialog.Show(owner);
                if (hr == ERROR_CANCELLED)
                    return null;

                if (CheckHr(hr, throwOnError) != 0)
                    return null;

                if (CheckHr(dialog.GetResults(out var items), throwOnError) != 0)
                    return null;

                items.GetCount(out var count);
                for (var i = 0; i < count; i++)
                {
                    items.GetItemAt(i, out var item);
                    CheckHr(item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out var path), throwOnError);
                    CheckHr(item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEEDITING, out var name), throwOnError);
                    if (path != null || name != null)
                    {
                        _resultPaths.Add(path);
                        _resultNames.Add(name);
                    }
                }
                return true;
            }

            private static int CheckHr(int hr, bool throwOnError)
            {
                if (hr != 0 && throwOnError) Marshal.ThrowExceptionForHR(hr);
                return hr;
            }

            [DllImport("shell32")]
            private static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItem ppv);

            [DllImport("user32")]
            private static extern IntPtr GetDesktopWindow();

#pragma warning disable IDE1006 // Naming Styles
            private const int ERROR_CANCELLED = unchecked((int)0x800704C7);
#pragma warning restore IDE1006 // Naming Styles

            [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")] // CLSID_FileOpenDialog
            private class FileOpenDialog { }

            [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface IFileOpenDialog
            {
                [PreserveSig] int Show(IntPtr parent); // IModalWindow
                [PreserveSig] int SetFileTypes();  // not fully defined
                [PreserveSig] int SetFileTypeIndex(int iFileType);
                [PreserveSig] int GetFileTypeIndex(out int piFileType);
                [PreserveSig] int Advise(); // not fully defined
                [PreserveSig] int Unadvise();
                [PreserveSig] int SetOptions(FOS fos);
                [PreserveSig] int GetOptions(out FOS pfos);
                [PreserveSig] int SetDefaultFolder(IShellItem psi);
                [PreserveSig] int SetFolder(IShellItem psi);
                [PreserveSig] int GetFolder(out IShellItem ppsi);
                [PreserveSig] int GetCurrentSelection(out IShellItem ppsi);
                [PreserveSig] int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
                [PreserveSig] int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
                [PreserveSig] int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
                [PreserveSig] int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
                [PreserveSig] int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
                [PreserveSig] int GetResult(out IShellItem ppsi);
                [PreserveSig] int AddPlace(IShellItem psi, int alignment);
                [PreserveSig] int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
                [PreserveSig] int Close(int hr);
                [PreserveSig] int SetClientGuid();  // not fully defined
                [PreserveSig] int ClearClientData();
                [PreserveSig] int SetFilter([MarshalAs(UnmanagedType.IUnknown)] object pFilter);
                [PreserveSig] int GetResults(out IShellItemArray ppenum);
                [PreserveSig] int GetSelectedItems([MarshalAs(UnmanagedType.IUnknown)] out object ppsai);
            }

            [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface IShellItem
            {
                [PreserveSig] int BindToHandler(); // not fully defined
                [PreserveSig] int GetParent(); // not fully defined
                [PreserveSig] int GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
                [PreserveSig] int GetAttributes();  // not fully defined
                [PreserveSig] int Compare();  // not fully defined
            }

            [ComImport, Guid("b63ea76d-1f85-456f-a19c-48159efa858b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface IShellItemArray
            {
                [PreserveSig] int BindToHandler();  // not fully defined
                [PreserveSig] int GetPropertyStore();  // not fully defined
                [PreserveSig] int GetPropertyDescriptionList();  // not fully defined
                [PreserveSig] int GetAttributes();  // not fully defined
                [PreserveSig] int GetCount(out int pdwNumItems);
                [PreserveSig] int GetItemAt(int dwIndex, out IShellItem ppsi);
                [PreserveSig] int EnumItems();  // not fully defined
            }

#pragma warning disable CA1712 // Do not prefix enum values with type name
            private enum SIGDN : uint
            {
                SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
                SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
                SIGDN_FILESYSPATH = 0x80058000,
                SIGDN_NORMALDISPLAY = 0,
                SIGDN_PARENTRELATIVE = 0x80080001,
                SIGDN_PARENTRELATIVEEDITING = 0x80031001,
                SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
                SIGDN_PARENTRELATIVEPARSING = 0x80018001,
                SIGDN_URL = 0x80068000
            }

            [Flags]
            private enum FOS
            {
                FOS_OVERWRITEPROMPT = 0x2,
                FOS_STRICTFILETYPES = 0x4,
                FOS_NOCHANGEDIR = 0x8,
                FOS_PICKFOLDERS = 0x20,
                FOS_FORCEFILESYSTEM = 0x40,
                FOS_ALLNONSTORAGEITEMS = 0x80,
                FOS_NOVALIDATE = 0x100,
                FOS_ALLOWMULTISELECT = 0x200,
                FOS_PATHMUSTEXIST = 0x800,
                FOS_FILEMUSTEXIST = 0x1000,
                FOS_CREATEPROMPT = 0x2000,
                FOS_SHAREAWARE = 0x4000,
                FOS_NOREADONLYRETURN = 0x8000,
                FOS_NOTESTFILECREATE = 0x10000,
                FOS_HIDEMRUPLACES = 0x20000,
                FOS_HIDEPINNEDPLACES = 0x40000,
                FOS_NODEREFERENCELINKS = 0x100000,
                FOS_OKBUTTONNEEDSINTERACTION = 0x200000,
                FOS_DONTADDTORECENT = 0x2000000,
                FOS_FORCESHOWHIDDEN = 0x10000000,
                FOS_DEFAULTNOMINIMODE = 0x20000000,
                FOS_FORCEPREVIEWPANEON = 0x40000000,
                FOS_SUPPORTSTREAMABLEITEMS = unchecked((int)0x80000000)
            }
#pragma warning restore CA1712 // Do not prefix enum values with type name
        } } 

