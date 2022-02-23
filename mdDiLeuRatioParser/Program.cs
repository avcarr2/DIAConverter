using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq; 

namespace mdDiLeuRatioParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Refactor to account for it being wrapped by a GUI now

            // args[0] = file path
            // args[1] = output file path
            string path = args[0];
            string outputFilePath = args[1];

            DataTable psms = FilePreprocessing.ReadPSMTVFile(path).AsEnumerable().OrderBy(i => i.Field<double>("Retention Time")).CopyToDataTable();
            double minRetentionTime = psms.AsEnumerable().Select(i => i.Field<double>("Retention Time")).Min();
            double maxRetentionTime = psms.AsEnumerable().Select(i => i.Field<double>("Retention Time")).Max();

            // get unique base sequences: 
            List<string> baseSequences = psms.AsEnumerable().Select(i => i.Field<string>("Base Sequence")).Distinct().ToList(); 

            psms.AsEnumerable().OrderBy(i => i.Field<double>("Retention Time"));

            List<QuantifiedPeak> qPeaks = new(); 
            foreach(DataRow row in psms.Rows)
			{
                qPeaks.Add(new QuantifiedPeak(row)); 
			}

            Dictionary<string, List<QuantifiedPeak>> seqDictionary = new(); 
            foreach(string baseSeq in baseSequences)
			{
                seqDictionary.Add(baseSeq, qPeaks.Where(i => i.BaseSequence == baseSeq).ToList());  
			}

            Dictionary<string, Dictionary<string, double>> ratioResults = seqDictionary.CalculateRatio();
            ratioResults.PrintRatioResultsToTextFile(outputFilePath); 
        }
    }

    public static class ProgramHelpers
    {
        public static void PrintRatioResultsToTextFile(this Dictionary<string, Dictionary<string, double>> ratioResults, string filePath)
		{
            using (TextWriter writer = new StreamWriter(filePath))
			{
                foreach(var baseSeq in ratioResults.Keys)
				{
                    foreach(var ratio in ratioResults[baseSeq])
					{
                        writer.WriteLine("{0}\t{1}\t{2]", baseSeq, ratio.Key, ratio.Value.ToString()); 
					}
				}
                writer.Flush(); 
			}
		}
        public static void SetPrimaryKey(this DataTable dt, string[] primaryKeys)
		{
            DataColumn[] keys = new DataColumn[primaryKeys.Length]; 
            foreach(string key in primaryKeys)
            {
                dt.Columns.CopyTo(keys, dt.Columns.IndexOf(key)); 
			}
            dt.PrimaryKey = keys; 
		}
        public static void PrintNLines(DataTable dt, int Nlines)
        {
            int numCol = dt.Columns.Count;
            
            for(int i = 0; i < Nlines; i++)
            {
                List<string> consoleOutput = new(); 
                for(int j= 0; j < numCol; j++)
                {
                    consoleOutput.Add(dt.Rows[i][j].ToString()); 
                }
                Console.WriteLine(string.Join(", ", consoleOutput.ToArray())); 
            }
        }
        public static List<string> GetColumnNames(this DataTable table)
        {
            int colNumber = table.Columns.Count;
            List<string> colNames = new(); 
            for(int i = 0; i < colNumber; i++)
            {
                colNames.Add(table.Columns[i].ColumnName); 
            }
            return colNames; 
        }
        public static void PrintColumnNames(this List<string> colNames)
        {
            foreach(string str in colNames)
            {
                Console.WriteLine(str); 
            }
        }

        // create .tsv writer to write data directly from the List<string, Dictionary<>> output

    }
}