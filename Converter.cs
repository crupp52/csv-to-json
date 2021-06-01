using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CsvToJson
{
    public class Converter
    {
        private Stopwatch sw;
        private int numberOfElements;
        private static object _lockObject = new object();

        public Converter()
        {
            sw = new Stopwatch();
            numberOfElements = 0;
        }

        public void ConvertDirectory(string directoryName, string outputDirectory = "csv2json")
        {            
            string[] filenames = Directory.GetFiles(directoryName);
            numberOfElements = filenames.Length;

            Task[] tasks = new Task[filenames.Length];

            for (int i = 0; i < filenames.Length; i++)
            {
                int index = i;
                tasks[index] = new Task(() => ConvertFile(filenames[index], outputDirectory));
            }

            for (int i = 0; i < filenames.Length; i++)
            {
                int index = i;
                tasks[index].Start();
            }

            Task.WaitAll(tasks);
        }

        public void ConvertFile(string filename, string outputDirectory)
        {
            StringBuilder output = new StringBuilder();

            output.Append("[");

            var lines = File.ReadAllLines(filename);

            int i = 0;

            while (lines[i++] != "[Data]")
            {
            }

            while (i < lines.Length)
            {
                output.Append("[");

                string[] columns = lines[i].Split(';');

                for (int j = 0; j < columns.Length; j++)
                {
                    if (columns[j] != "")
                    {
                        if (j + 1 < columns.Length && columns[j + 1] != "")
                        {
                            output.Append(double.Parse(columns[j]).ToString("0.00", CultureInfo.InvariantCulture) + ", ");
                        }
                        else
                        {
                            output.Append(double.Parse(columns[j]).ToString("0.00", CultureInfo.InvariantCulture));
                        }
                    }
                }

                if (i < lines.Length)
                {
                    output.Append("],");
                }
                else
                {
                    output.Append("]");
                }


                i++;
            }

            output.Append("]");

            File.WriteAllText($@"{outputDirectory}\{Path.GetFileNameWithoutExtension(filename)}.json", output.ToString(), Encoding.UTF8);

            //lock (_lockObject)
            //{
            //    Console.WriteLine($@"Converted: {filename} to csv2json\{Path.GetFileNameWithoutExtension(filename)}.json" );
            //}
        }
    }
}