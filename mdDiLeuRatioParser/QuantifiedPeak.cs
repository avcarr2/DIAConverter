using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Linq; 
namespace mdDiLeuRatioParser
{
    public enum LabelTypes {Light, Medium, Heavy, Unlabeled}

    public class QuantifiedPeak
    {
        public string BaseSequence { get; private set; }
        public string FullSequence { get; set; }
        public double Intensity { get; set; }
        public ModificationClass Modifications { get; set; }
        public LabelTypes LabelType { get; set; }
        public double RetentionTime { get; set; }
        
        // get all base sequences, then match full sequences 

        public QuantifiedPeak(DataRow dataRow)
        {
            BaseSequence = dataRow.Field<string>("BaseSequence");
            FullSequence = dataRow.Field<string>("FullSequence");
            RetentionTime = dataRow.Field<double>("MS2 Retention Time");
            Intensity = dataRow.Field<double>("Peak Intensity");
            Modifications = new ModificationClass(dataRow);
            ClassifyAndAssignLabelType(); 
        }
        public void ClassifyAndAssignLabelType()
        {
            List<string> labels = new();
            Modifications.ModificationDict.Values.ToList().ForEach(i =>
            {
                
                if (ModificationIsLabel(i))
                {
                    labels.Add(i); 
                }
            });
            if (labels.Any())
            {
                AssignModificationLabelType(labels[0]);
            }
        }
        
        public bool ModificationIsLabel(string modification)
        {
            bool modIsLabel = false; 
            LabelDictionary labels = new LabelDictionary();
            LabelTypes lt;
            // 
            bool success = labels.LabelDict.ContainsKey(modification);
            if (!success)
            {
                lt = LabelTypes.Unlabeled;
                modIsLabel = false; 
            }
            else
            {
                modIsLabel = true; 
            }
            return modIsLabel; 
        }
        public void AssignModificationLabelType(string modification)
        {
            
            LabelDictionary labels = new LabelDictionary();
            LabelTypes lt = new();
            
            bool success = labels.LabelDict.TryGetValue(modification, out lt);
            if (!success)
            {
                LabelType = LabelTypes.Unlabeled;
            }
            else
            {
                LabelType = lt; 
            }
        }

    }

    public static class QuantifiedPeakExtensions
    {
        public static bool CheckModificationsAreEquivalent(this QuantifiedPeak qpeak, QuantifiedPeak quantifiedPeak)
        {
            return qpeak.CheckModificationsAreEquivalent(quantifiedPeak); 
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
            foreach(DataRow row in dataTable.Rows)
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
            foreach(string baseSeq in baseSequences)
            {
                results.Add(baseSeq, rtFilteredQuantPeaks.Where(i => i.BaseSequence == baseSeq).Select(j => j).ToList()); 
            }
            return results; 
        }
        public static Dictionary<string, List<(LabelTypes Label1, LabelTypes Label2, double Intensity)>> CalculateRatio(this Dictionary<string, List<QuantifiedPeak>> filteredDict)
        {
            // For each unique base sequence: 
            // Find all quantifiable peaks for the given base sequence
            foreach(string baseSeq in filteredDict.Keys)
            {

            }

            // Calculate the ratios.
            // Ratio are 801/261 and 090/261
            /*  Label Type
             *  
             *  
             */
            // From each base sequence, fill out the table.
            // At max, you'll have two comparisons.

             

        }
        public static void FindAllQuantifiablePeaks(this List<QuantifiedPeak> qPeaksListFilteredByBaseSeq)
        {
            
        }
    }
}
