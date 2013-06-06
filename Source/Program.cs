using System.Reflection;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csvvaluecounter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = assembly.GetName();

                Console.WriteLine(Environment.NewLine + "csvvaluecounter v" + assemblyName.Version.ToString(3) + Environment.NewLine);

                Options options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(args, options) == false)
                {
                    return;
                }

                List<ValueCounter> data = new List<ValueCounter>();

                using (var csvReader = new CsvReader(new StreamReader(options.Input), options.HasHeaders, options.Delimiter))
                {
                    while (csvReader.ReadNextRecord())
                    {
                        var temp = (from d in data where d.Value.ToLower() == csvReader[options.Field].ToLower() select d).SingleOrDefault();
                        if (temp == null)
                        {
                            data.Add(new ValueCounter(csvReader[options.Field]));
                        }
                        else
                        {
                            temp.Count++;
                        }
                    }
                }

                var sorted = from d in data orderby d.Count, d.Value select d;
                foreach (ValueCounter vc in sorted)
                {
                    WriteTextToFile(vc.Value + "\t" + vc.Count + Environment.NewLine, options.Output, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filename"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        private static string WriteTextToFile(string text,
                                            string filename,
                                            bool append)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename, append, Encoding.GetEncoding(1252)))
                {
                    streamWriter.Write(text);
                }

                return string.Empty;
            }
            catch (IOException ex)
            {
                return ex.Message;
            }
            catch (UnauthorizedAccessException unex)
            {
                return unex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
