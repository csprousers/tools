using System.IO;
using System.Text;

namespace CSPro
{
    public class IO
    {
        private static readonly byte[] Utf8Bom = { 0xEF,0xBB,0xBF };
        private const int PreUnicodeCodePage = 1252;
        private const double Utf8FirstVersion = 5.0;

        public static StreamReader CreateStreamReaderFromSpecFile(string filename)
        {
            // read the BOM to see if the file should be opened as a UTF-8 file or as an ANSI file with codepage 1252
            bool hasUtf8BOM = false;

            using( BinaryReader br = new BinaryReader(File.Open(filename,FileMode.Open)) )
            {
                byte[] header = br.ReadBytes(Utf8Bom.Length);
                hasUtf8BOM = header.Length == 3 && header.Equals(Utf8Bom);
            }

            return new StreamReader(filename,hasUtf8BOM ? Encoding.UTF8 : Encoding.GetEncoding(PreUnicodeCodePage));
        }

        public static StreamWriter CreateStreamReaderForSpecFile(string filename,double versionNumber = CurrentVersion.Version)
        {
            return new StreamWriter(filename,false,versionNumber >= Utf8FirstVersion ? Encoding.UTF8 : Encoding.GetEncoding(PreUnicodeCodePage));
        }

    }
}
