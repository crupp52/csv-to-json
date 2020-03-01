using System;

namespace CsvToJson
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Converter converter = new Converter();
            converter.ConvertDirectory("csv_images");
        }
    }
}