using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Labb3
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            var pngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            var bmpSignature = new byte[] { 66, 77 };

            try
            {
                var fileStream = new FileStream(filePath, FileMode.Open);
                var signatureArray = new byte[8];
                fileStream.Read(signatureArray, 0, 8);

                fileStream.Seek(16, SeekOrigin.Begin);
                var dataPixels = new byte[8];
                fileStream.Read(dataPixels, 0, 8);

                fileStream.Close();

                if (pngSignature.SequenceEqual(signatureArray))
                {
                    DisplayPNGInfo(dataPixels);
                    DisplayPNGChunks(filePath);
                }
                else if (bmpSignature[0] == signatureArray[0] && bmpSignature[1] == signatureArray[1])
                {
                    DisplayBMPInfo(dataPixels);
                }
                else
                {
                    Console.WriteLine("This is not a valid .bmp or .png file!");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Application has crashed. Are you sure input was a valid filepath?");
            }

            Console.WriteLine("\nPress any key to exit!");
            Console.ReadKey();
        }
        public static void DisplayPNGInfo(byte[] imageData)
        {
            var widthArray = new byte[4];
            var heightArray = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                widthArray[i] = imageData[i];
                heightArray[i] = imageData[i + 4];
            }
            Array.Reverse(widthArray);
            Array.Reverse(heightArray);

            int width = BitConverter.ToInt32(widthArray);
            int height = BitConverter.ToInt32(heightArray);

            Console.WriteLine($"\nThis is a .png image. Resolution: {width}x{height} pixels.\n");
        }

        public static void DisplayBMPInfo(byte[] imageData)
        {
            var widthArray = new byte[4];
            var heightArray = new byte[4];

            for (int i = 0; i < 2; i++)
            {
                widthArray[i] = imageData[i + 2];
                heightArray[i] = imageData[i + 6];
            }

            int width = BitConverter.ToInt32(widthArray);
            int height = BitConverter.ToInt32(heightArray);

            Console.WriteLine($"\nThis is a .BMP image. Resolution: {width}x{height} pixels.\n");
        }

        public static void DisplayPNGChunks(string filepath)
        {
            var ChunkLengthArr = new byte[4];
            var ChunkTypeArr = new byte[4];

            var fileStreamChunk = new FileStream(filepath, FileMode.Open);

            bool findChunks = false;
            int startIndex = 8;
            int lengthValue;
            string chunkType;

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

                Console.WriteLine($"Chunk type: {chunkType} - Length: {lengthValue} bytes");

                if (chunkType == "IEND")
                {
                    findChunks = true;
                }
            }
            fileStreamChunk.Close();
        }
    }
}
