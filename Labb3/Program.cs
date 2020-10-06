using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Labb3
{
    class Program
    {

        //Läs in filen och lägg till datan som bytes i en array
        //kör en metod som kollar om de första 8 bytes är =  0x 89 50 4E 47 0D 0A 1A 0A. Om true => det är en PNG fil och fortsätt i programmet. Om false = cw("This is not a valid .bmp or .png file!")
        // kör en metod som kollar från plats 16-19 och konvertar från bytes till dec. Detta blir Width. Spara i en variabel.
        // kör en metod som kollar från plats 20 - 23 och konverterar från bytes till dec. Detta blir Height. Spara i en variabel.
        //Skriv ut datan i consolen: storlek, och vilken filtyp.
        //Vi har nu all info om filen för G-nivå.


        // Lenght: Läs av plats 5-8(byte) som visar hur lång datan i den chunken kommer vara kallat Lenght. längden på datan är summan av de 4 bytes dock ej medräknat Lenght



        static void Main(string[] args)
        {
            Console.Write("Skriv in en sökväg för en .png eller .BMP fil:");
            string filePath = Console.ReadLine();
            var fileStream = new FileStream(filePath, FileMode.Open);

            var pngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            var bmpSignature = new byte[] { 66, 77 };
            //var fileSize = (int)filePNG.Length;


            var signatureArr = new byte[8];

            fileStream.Read(signatureArr, 0, 8);

            var dataPixels = new byte[8];

            fileStream.Seek(16, SeekOrigin.Begin);
            fileStream.Read(dataPixels, 0, 8);
            fileStream.Close();


            if (pngSignature.SequenceEqual(signatureArr))
            {
                displayPngInfo(dataPixels);
                displayChunks(filePath);
            }
            else if (bmpSignature[0] == signatureArr[0] && bmpSignature[1] == signatureArr[1])
            {
                displayBMPInfo(dataPixels);
            }
            else
            {
                Console.WriteLine("This is not a valid .bmp or .png file!");
            }

            


        }
        public static void displayPngInfo(byte[] bytes1)
        {
            int width = 0, height = 0;
            

            for (int i = 0; i <= 3; i++)
            {
                width = bytes1[i] | width << 8;
                height = bytes1[i + 4] | height << 8;
            }

            Console.WriteLine($"This is a .png image. Resolution: {width}x{height} pixels.");
        }

        public static void displayBMPInfo(byte[] bytes2)
        {


            int width = 0, height = 0;

            for (int i = 3; i >= 2; i--)
            {
                width = bytes2[i] | width << 8;
                height = bytes2[i + 4] | height << 8;
            }

            Console.WriteLine($"This is a .BMP image. Resolution: {width}x{height} pixels.");
        }

        public static void displayChunks(string filepath)
        {

            var fileStreamChunk = new FileStream(filepath, FileMode.Open);
            var ChunkLengthArr = new byte[4];
            var ChunkTypeArr = new byte[4];
            bool findChunks = false;
            int startIndex = 8;
            string chunkType = "";
            int lengthValue;

            while (findChunks == false)
            {
                fileStreamChunk.Seek(startIndex, SeekOrigin.Begin);
                fileStreamChunk.Read(ChunkLengthArr, 0, 4);
                fileStreamChunk.Seek(startIndex + 4, SeekOrigin.Begin);
                fileStreamChunk.Read(ChunkTypeArr, 0, 4);
                
                Array.Reverse(ChunkLengthArr);
                lengthValue = BitConverter.ToInt32(ChunkLengthArr);
              
                startIndex += lengthValue + 12;
                chunkType = Encoding.ASCII.GetString(ChunkTypeArr);
                Console.WriteLine($"{chunkType} - Length: {lengthValue}");

                if (chunkType == "IEND")
                {
                    findChunks = true;
                }

            }

            fileStreamChunk.Close();
        }
    }

}
