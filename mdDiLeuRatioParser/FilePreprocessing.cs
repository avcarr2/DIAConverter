using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; 

namespace mdDiLeuRatioParser
{
    public static class FilePreprocessing
    {
        public static DataTable ReadPSMTVFile(string path)
        {
            var data = new DataTable();
            var reader = ReadAsLines(path);

            var headers = reader.First().Split('\t');

            foreach(var header in headers)
            {
                data.Columns.Add(header);
            }
            var records = reader.Skip(1);
            foreach(var record in records)
            {
                data.Rows.Add(record.Split('\t')); 
            }
            return data;             
        }
        public static DataTable CorrectColumnType(this DataTable dt)
		{
            DataTable clonedResultsDT = new();
            var colNames = dt.GetColumnNames();
            Regex regex = new Regex(@"[^0-9.]");
            Regex emptyRegex = new Regex(@"^$"); 
            colNames.ForEach(name =>
            {

                // create columns with matching names of the original dataTable.
                clonedResultsDT.Columns.Add(name);

                // test whether or not the values from the field contains an alphabetical character. 
                var testString = dt.AsEnumerable().Select(j => j.Field<string>(name)).Take(1).ToList().ElementAt(0);

                // if it contains an alphabetical character, it cannot be cast to a double. 
                // if it is a column of blanks, then it automatically becomes a string column. 
				if (emptyRegex.IsMatch(testString) | regex.IsMatch(testString))
				{
                    clonedResultsDT.Columns[name].DataType = typeof(string);
				}
				else
				{
                    clonedResultsDT.Columns[name].DataType = typeof(double);
                }
            }); 
            foreach(DataRow row in dt.AsEnumerable())
			{
                // If clause and valIsNumber bool is to protect from situation where the Peak intensity column is zero, 
                // resulting in dashes ("-") in the peak description columns (start, apex, end). 
                // Additionally, the peak intensity values must be greater than zero. 
                bool valIsNumber = double.TryParse(row.Field<string>("Peak intensity"), out double tempVal); 
                if(valIsNumber && tempVal > 0)
				{
                    clonedResultsDT.ImportRow(row);
                }
            }
            return clonedResultsDT; 
		}

        static IEnumerable<string> ReadAsLines(string path)
        {
            using (var reader = new StreamReader(path))
                while (!reader.EndOfStream)
                    yield return reader.ReadLine();
        }
        public static List<string> GetUniqueBaseSequences(this DataTable table)
        {
            // get only the BaseSequence column
            // then get the unique from that column.
            List<string> distinctIds = new(); 
            table.AsEnumerable().Select(i => i.Field<string>("Base Sequence")).ToList().Distinct();
            return distinctIds; 
        }
        // Define a retention time window.
        // walk the retention across the chromatogram
        // find the peaks whose base sequence match within the retention time window
            // get whether it's a heavy or light labelled peptide.
            // check that base sequence, then full sequence match.
                // if no match, skip to the next peak within a window. 
        // create peak ratio object
        // calculate the actual ratio
        public static void GetQuantifiedPeaksFromRTWindow(DataTable dt, RetentionTimeWindow rtw)
        {

        }

        public static void GetPeaksFromDataTableInRTWindow(this DataTable table, RetentionTimeWindow rtWindow)
        {
            List<DataRow> dataRowList = table.AsEnumerable()
                .Where(i => i.Field<double>("MS2 Retention Time") < rtWindow.LastRT & i.Field<double>("MS2 RetentionTime") > rtWindow.FirstRT)
                .Select(i => i).ToList();
        }
        public static void ConvertDataRowToQuantifiedPeak(this DataRow dataRow)
        {
            dataRow.Field<string>("BaseSequence");
            dataRow.Field<string>("FullSequence");
            dataRow.Field<double>("MS2 Retention Time");
            dataRow.Field<double>("Peak Intensity");
        }
    }

}
