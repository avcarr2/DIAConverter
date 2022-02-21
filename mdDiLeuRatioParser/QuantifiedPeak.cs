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
}
