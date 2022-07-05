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

            DataTable psms = FilePreprocessing.ReadPSMTVFile(path).CorrectColumnType();

            // get unique base sequences: 
            List<string> baseSequences = psms.AsEnumerable().Select(i => i.Field<string>("Base Sequence")).Distinct().ToList(); 

            List<QuantifiedPeak> qPeaks = new(); 
            foreach(DataRow row in psms.Rows)
			{
                qPeaks.Add(new QuantifiedPeak(row)); 
			}
            (int, int, int) labelCounts = qPeaks.GetLabelCounts();
            labelCounts.PrintLabelCount("labelCounts.tsv");

            Dictionary<string, List<QuantifiedPeak>> seqDictionary = new(); 
            foreach(string baseSeq in baseSequences)
			{
                seqDictionary.Add(baseSeq, qPeaks.Where(i => i.BaseSequence == baseSeq).ToList());  
			}
            Dictionary<string, Dictionary<string, double>> ratioResults = seqDictionary.CalculateRatio();
            string outputPath = "mdDiLeuRatiosOutput.tsv"; 
            ratioResults.PrintRatioResultsToTextFile(Path.Combine(outputFilePath, outputPath));

            BoxWhiskerPlots.CreateDiLeuBoxPlot(outputPath, LabelTypes.Medium, LabelTypes.Heavy, "DiLeuLabellingBoxPlots.png");
        }
    }

    public static class ProgramHelpers
    {
        public static void PrintRatioResultsToTextFile(this Dictionary<string, Dictionary<string, double>> ratioResults, string filePath)
		{
            using (TextWriter writer = new StreamWriter(filePath))
			{
                writer.WriteLine(string.Join("\t", "Base Sequence", "Full Sequence", "Label1", "Label2", "Retention Time", "Intensity Ratio")); 
                foreach(var baseSeq in ratioResults.Keys)
				{
                    foreach(var ratio in ratioResults[baseSeq])
					{
                        writer.WriteLine("{0}\t{1}\t", ratio.Key.ToString(), ratio.Value.ToString());  
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
        public static (int, int, int) GetLabelCounts(this List<QuantifiedPeak> qPeaks)
        {
            int light; int medium; int heavy; 

            light = qPeaks.Where(i => i.LabelType == LabelTypes.Light).Count();
            medium = qPeaks.Where(i => i.LabelType == LabelTypes.Medium).Count();
            heavy = qPeaks.Where(i => i.LabelType == LabelTypes.Heavy).Count();

            return (light, medium, heavy); 
        }
        public static void PrintLabelCount(this (int, int, int) labelCounts, string path)
        {
            using TextWriter writer = new StreamWriter(path);

            writer.WriteLine("{0}\t{1}\t{2}", labelCounts.Item1, labelCounts.Item2, labelCounts.Item3);
            writer.Flush(); 
        }

        // create .tsv writer to write data directly from the List<string, Dictionary<>> output

    }
}