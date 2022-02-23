using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data; 

namespace mdDiLeuRatioParser
{

    public static class QuantifiedPeakExtensions
    {
        public static bool CheckModificationsAreEquivalent(this QuantifiedPeak qpeak, QuantifiedPeak quantifiedPeak)
        {
            return qpeak.ModificationsEqual(quantifiedPeak);
        }
        public static bool BaseSequencesAreEqual(this QuantifiedPeak qPeak, QuantifiedPeak quantifiedPeak)
        {
            bool result = false;
            if (qPeak.BaseSequence.Equals(quantifiedPeak.BaseSequence))
            {
                result = true;
            }
            return result;
        }
        public static bool CheckPeaksAreQuantifiable(this QuantifiedPeak qPeak, QuantifiedPeak quantifiedPeak)
        {
            bool peakAreQuantifiable = false;
            // if base sequences are equal
            if (qPeak.BaseSequence.Equals(quantifiedPeak.BaseSequence))
            {
                // if full sequences are equal
                // full sequences are equal if modifications are at the same location
                // and modifications are equivalent.
                if (qPeak.CheckModificationsAreEquivalent(quantifiedPeak))
                {
                    peakAreQuantifiable = true;
                }
            }
            return peakAreQuantifiable;
        }
        public static List<QuantifiedPeak> CreateQuantifiedPeaksList(this DataTable dataTable)
        {
            List<QuantifiedPeak> quantPeakList = new();
            foreach (DataRow row in dataTable.Rows)
            {
                quantPeakList.Add(new QuantifiedPeak(row));
            }
            return quantPeakList;
        }
        // Dict< string = base sequence, list = list of peaks matching base sequence>
        public static Dictionary<string, List<QuantifiedPeak>> SelectQuantifiablePeaksInRTWindow(RetentionTimeWindow rtwindow, List<QuantifiedPeak> quantPeakList)
        {
            Dictionary<string, List<QuantifiedPeak>> results = new();
            List<QuantifiedPeak> rtFilteredQuantPeaks = quantPeakList.Where(i => i.RetentionTime < rtwindow.LastRT & i.RetentionTime > rtwindow.FirstRT)
                .Select(j => j).ToList();
            // find all unique base sequences
            List<string> baseSequences = rtFilteredQuantPeaks.Select(i => i.BaseSequence).Distinct().ToList();
            // add each unique base sequence and the list of quantified peaks into the dictionary
            foreach (string baseSeq in baseSequences)
            {
                results.Add(baseSeq, rtFilteredQuantPeaks.Where(i => i.BaseSequence == baseSeq).Select(j => j).ToList());
            }
            return results;
        }

        public static bool Ratio801Over261(QuantifiedPeak qp)
        {
            return ((int)qp.LabelType == (int)LabelTypes.Heavy | (int)qp.LabelType == (int)LabelTypes.Medium);
        }
        public static bool Ratio090Over261(QuantifiedPeak qp)
        {
            return ( (int)qp.LabelType == (int)LabelTypes.Light | (int)qp.LabelType == (int)LabelTypes.Medium);
        }
        public static Dictionary<string, Dictionary<string, double>> CalculateRatio(this Dictionary<string, List<QuantifiedPeak>> filteredDict)
        {
            // For each unique base sequence: 
            // Find all quantifiable peaks for the given base sequence
            var resultsDict = new Dictionary<string, Dictionary<string, double>>();

            foreach (var baseSequence in filteredDict.Keys)
            {

                var ratioOutput1 = CalculateRatiosFromList(filteredDict[baseSequence], LabelTypes.Heavy, LabelTypes.Medium);
                var ratioOutput2 = CalculateRatiosFromList(filteredDict[baseSequence], LabelTypes.Light, LabelTypes.Medium);
                var tempOutput = ratioOutput1.Concat(ratioOutput2).ToDictionary(key => key.Key, val => val.Value);
                resultsDict.Add(baseSequence, tempOutput); 
            }
            return resultsDict;
        }
        // use .FindAll() to get all base sequences across the whole thing, then keep using FindAll to get those within a certain retention time, and those with matching modifications. 
        // it'll probably be a lot cleaner than what you've already written. 

        public static Dictionary<string, double> CalculateRatiosFromList(List<QuantifiedPeak> qpList, LabelTypes lab1, LabelTypes lab2)
        {
            var labelType1 = qpList.Where(i => (int)i.LabelType == (int)lab1).Select(i=>i).ToList();
            var labelType2 = qpList.Where(i => (int)i.LabelType == (int)lab2).Select(i=>i).ToList();

            Dictionary<string, double> ratioDict = new();

            for (int i = 0; i < labelType1.Count; i++)
            {
                for (int j = 0; j < labelType2.Count; j++)
                {
                    if (!labelType1[i].CheckModificationsAreEquivalent(labelType2[j]))
                    {
                        continue;
                    }
                    else
                    {
                        double ratio = labelType1[i].Intensity / labelType2[j].Intensity;
                        string ratioName = String.Join("\t", labelType1[i].BaseSequence.ToString(), labelType1[i].FullSequence.ToString(),
                            "Label1: " + labelType1[i].LabelType.ToString(), "Label2: " + labelType2[j].LabelType.ToString());
                        ratioDict.Add(ratioName, ratio);
                    }
                }
            }
            return ratioDict;
        }
        public static void MatchAllQuantifiablePeaks(this List<QuantifiedPeak> qPeaksListFilteredByBaseSeq)
        {
            // Get unique base sequences. 
            // Iterate over the unique base sequences
            // accumulate the base sequences in a separate list. 

        }
    }
}
