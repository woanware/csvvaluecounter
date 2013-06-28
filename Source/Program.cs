using System.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace csvvaluecounter
{
    class Program
    {
        private static Options _options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = assembly.GetName();

                Console.WriteLine(Environment.NewLine + "csvvaluecounter v" + assemblyName.Version.ToString(3) + Environment.NewLine);

                _options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(args, _options) == false)
                {
                    return;
                }

                List<ValueCounter> data = new List<ValueCounter>();

                CsvHelper.Configuration.CsvConfiguration csvConfiguration = new CsvHelper.Configuration.CsvConfiguration();
                csvConfiguration.HasHeaderRecord = _options.HasHeaders;
                csvConfiguration.Delimiter = _options.Delimiter.ToString();
                csvConfiguration.IsStrictMode = false;

                using (var csvReader = new CsvReader(new StreamReader(_options.Input), csvConfiguration ))
                {
                    int index = 0;
                    while (csvReader.Read())
                    {
                        index++;

                        if (index == 1)
                        {
                            if (_options.HasHeaders == true)
                            {
                                if (_options.Field > csvReader.FieldHeaders.Count() - 1)
                                {
                                    Console.WriteLine("The field index supplied does not exist in the file");
                                    return;
                                }
                            }
                        }

                        if (csvReader[_options.Field] == null)
                        {
                            break;
                        }
                        
                        var temp = (from d in data where d.Value.ToLower() == csvReader[_options.Field].ToLower() select d).SingleOrDefault();
                        if (temp == null)
                        {
                            data.Add(new ValueCounter(csvReader[_options.Field]));
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
                    WriteTextToFile(vc.Value + "\t" + vc.Count + Environment.NewLine, _options.Output, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private static void csvReader_ParseError(object sender, ParseErrorEventArgs e)
        //{

        //    string fileName = Path.GetFileNameWithoutExtension(_options.Output);
        //    fileName += ".Errors" + Path.GetExtension(_options.Output);

        //    WriteTextToFile(e.Error + "\t" + e.Error.Data + Environment.NewLine, fileName, true);

        //    e.Action = ParseErrorAction.AdvanceToNextLine;
        //}

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
