using System.IO;
using System.Text;

namespace CSPro
{
    public class IO
    {
        private static readonly byte[] Utf8Bom = { 0xEF,0xBB,0xBF };
        private const int PreUnicodeCodePage = 1252;
        private const double Utf8FirstVersion = 5.0;

        public static bool IsFileUtf8(string filename)
        {
            using( BinaryReader br = new BinaryReader(File.Open(filename,FileMode.Open)) )
            {
                byte[] header = br.ReadBytes(Utf8Bom.Length);
                return header.Length == 3 && header.Equals(Utf8Bom);
            }
        }

        public static StreamReader CreateStreamReaderFromCSProFile(string filename)
        {
            return new StreamReader(filename,IsFileUtf8(filename) ? Encoding.UTF8 : Encoding.GetEncoding(PreUnicodeCodePage));
        }

        public static StreamWriter CreateStreamWriterForCSProFile(string filename,double versionNumber = CurrentVersion.Version)
        {
            return new StreamWriter(filename,false,versionNumber >= Utf8FirstVersion ? Encoding.UTF8 : Encoding.GetEncoding(PreUnicodeCodePage));
        }

        public static StreamWriter CreateStreamWriterForCSProFile(string filename,bool utf8)
        {
            return new StreamWriter(filename,false,utf8 ? Encoding.UTF8 : Encoding.GetEncoding(PreUnicodeCodePage));
        }

    }
}
