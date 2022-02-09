using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq; 

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
        public static GetQuantifiedPeaksFromRTWindow(DataTable dt, RetentionTimeWindow rtw)
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
