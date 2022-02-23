using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Linq; 
namespace mdDiLeuRatioParser
{
    public enum LabelTypes {Light, Medium, Heavy, Unassigned}

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
        public QuantifiedPeak(string baseSeq, string fullSeq, double retTime, double intensity)
		{
            BaseSequence = baseSeq;
            FullSequence = fullSeq;
            RetentionTime = retTime;
            Intensity = intensity;
            Modifications = new ModificationClass(fullSeq);
            ClassifyAndAssignLabelType(); 
		}
        public QuantifiedPeak()
		{

		}
        public QuantifiedPeak((string, string, double, double) tupleParams)
		{
            BaseSequence = tupleParams.Item1;
            FullSequence = tupleParams.Item2;
            RetentionTime = tupleParams.Item3;
            Intensity = tupleParams.Item4;
            Modifications = new ModificationClass(tupleParams.Item2);
            ClassifyAndAssignLabelType(); 
        }
        // public QuantifiedPeak(string baseSequence, string fullSequence, double retentionTime, double intensity, ModificationClass)
        public void ClassifyAndAssignLabelType()
        {
            LabelType = LabelTypes.Unassigned; 

            foreach(string mod in Modifications.ModificationDict.Values)
			{
				if (ModificationIsLabel(mod))
				{
                    AssignModificationLabelType(mod);
                    break; 
				}
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
                lt = LabelTypes.Unassigned;
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
                LabelType = LabelTypes.Unassigned;
            }
            else
            {
                LabelType = lt; 
            }
        }
        public bool ModificationsEqual(Dictionary<int, string> modDict2)
        {
            bool isEqual = false;
            // iterate through dictionaries and convert the label modification to just "label"
            if (Modifications.ModificationDict.Count == modDict2.Count)
            {
                var dict1 = Modifications.ModificationDict.ReplaceLabelStringWithLabel();
                var dict2 = modDict2.ReplaceLabelStringWithLabel();
                isEqual = dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
            }
            return isEqual;
        }
        public bool ModificationsEqual(QuantifiedPeak qp)
		{
            bool isEqual = false; 
            if(Modifications.ModificationDict.Count == qp.Modifications.ModificationDict.Count)
			{
                var dict1 = Modifications.ModificationDict.ReplaceLabelStringWithLabel();
                var dict2 = qp.Modifications.ModificationDict.ReplaceLabelStringWithLabel();
                isEqual = dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
            }
            return isEqual; 
		}


    }
}
