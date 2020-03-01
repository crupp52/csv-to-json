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

        public void ConvertDirectory(string directoryName)
        {
            StartTimer();
            
            string[] filenames = Directory.GetFiles(directoryName);
            numberOfElements = filenames.Length;

            Task[] tasks = new Task[filenames.Length];

            for (int i = 0; i < filenames.Length; i++)
            {
                int index = i;
                tasks[index] = new Task(() => ConvertFile(filenames[index]));
            }

            for (int i = 0; i < filenames.Length; i++)
            {
                int index = i;
                tasks[index].Start();
            }

            Task.WaitAll(tasks);
            
            StopTimer();
        }

        private void ConvertFile(string filename)
        {
            StringBuilder output = new StringBuilder();

            output.Append("[");

            using (StreamReader sr = new StreamReader(filename, Encoding.UTF8))
            {
                for (int i = 0; i < 9; i++)
                {
                    sr.ReadLine();
                }

                while (!sr.EndOfStream)
                {
                    output.Append("[");

                    string line = sr.ReadLine();

                    string[] columns = line.Split(';');

                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (columns[i] != "")
                        {
                            if (columns[i + 1] != "")
                            {
                                output.Append(double.Parse(columns[i]).ToString("0.00", CultureInfo.InvariantCulture) + ", ");
                            }
                            else
                            {
                                output.Append(double.Parse(columns[i]).ToString("0.00", CultureInfo.InvariantCulture));
                            }
                        }
                    }

                    if (!sr.EndOfStream)
                    {
                        output.Append("],");
                    }
                    else
                    {
                        output.Append("]");
                    }
                }
            }

            output.Append("]");

            File.WriteAllText($@"csv2json\{Path.GetFileNameWithoutExtension(filename)}.json", output.ToString(), Encoding.UTF8);

            lock (_lockObject)
            {
                Console.WriteLine($@"Converted: {filename} to csv2json\{Path.GetFileNameWithoutExtension(filename)}.json" );
            }
        }

        private void StartTimer()
        {
            sw.Reset();

            sw.Start();
        }

        private void StopTimer()
        {
            sw.Stop();

            Console.WriteLine($"Converted files: {numberOfElements}\nElapsed time: {sw.Elapsed.TotalMinutes}m\nAverage convert time: {sw.Elapsed.TotalSeconds / numberOfElements}s");
        }
    }
}