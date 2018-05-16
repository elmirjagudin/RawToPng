using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

class Program
{
    const string IMG_DUMP_DIR = @"E:\RectifiedImgs";
    static void WritePNG(string fname, byte[] pixels, int width, int height)
    {
        PixelFormat pf = PixelFormats.Bgr32;
        int rawStride = (width * pf.BitsPerPixel + 7) / 8;

        BitmapSource bitmap = BitmapSource.Create(width, height,
                    96, 96, pf, null,
                    pixels, rawStride);

        using (FileStream stream = new FileStream(fname, FileMode.Create))
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
        }
    }

    static void Convert(string srcFile, string destFile)
    {

        using (var reader = new BinaryReader(new FileStream(srcFile, FileMode.Open)))
        {
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var pixels = reader.ReadBytes(width * height * 4);
            Console.WriteLine("'{0}' -> '{1}' ({2}x{3})", srcFile, destFile, width, height);
            WritePNG(destFile, pixels, width, height);
        }
    }

    static void ConvertAll()
    {
        foreach(var fname in Directory.GetFiles(IMG_DUMP_DIR))
        {
            Console.WriteLine("fname '{0}'", fname);
            if (!fname.EndsWith(".raw"))
            {
                Console.WriteLine("not .raw file, skipping");
                continue;
            }

            var dst_file = fname.Substring(0, fname.Length - 4) + ".png";
            Convert(fname, dst_file);
        }
    }

    static void Main(string[] args)
    {
        ConvertAll();
    }
}
