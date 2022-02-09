using System;
using System.Data;
using System.Collections.Generic;
using System.IO; 

namespace mdDiLeuRatioParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine("Data", "AllQuantifiedPeaks.tsv");
            DataTable psms = FilePreprocessing.ReadPSMTVFile(path);

        }
    }

    public static class ProgramHelpers
    {
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
    }
}