using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.Linq; 
namespace mdDiLeuRatioParser
{
    public class ModificationClass
    {
        public Dictionary<int, string> ModificationDict { get; private set; }

        public ModificationClass(string fullSeq)
        {
            ModificationDict = new Dictionary<int, string>(); 
            ParseModifications(fullSeq); 
        }
        public ModificationClass()
		{
            ModificationDict = new Dictionary<int, string>();
        }
        public ModificationClass(DataRow dataRow)
        {
            ModificationDict = new Dictionary<int, string>();
            string fullSeq = dataRow.Field<string>("Full Sequence");
            ParseModifications(fullSeq); 
        }
        public void ParseModifications(string fullSeq)
        {
            // use a regex to get all modifications
            string pattern = @"\[(.+?)\]"; 

            Regex regex = new(pattern);

            // remove each match after adding to the dict. Otherwise, getting positions
            // of the modifications will be rather difficult.
            int patternMatches = regex.Matches(fullSeq).Count;

            MatchCollection matches = regex.Matches(fullSeq);
            int currentPosition = 0; 
            foreach(Match match in matches)
			{

                GroupCollection group = match.Groups;
                string val = group[1].Value;
                int startIndex = group[0].Index;
                int captureLength = group[0].Length; 
                int position = group["(.+?)"].Index; 

                ModificationDict.Add(startIndex - currentPosition, val);
                currentPosition += startIndex + captureLength;
            }
        }

        public string ConvertLabelString(string modification)
        {
            string result = ""; 
            if (ModificationsExtensions.ModificationIsLabel(modification))
            {
                result = "label";
            }
            else
            {
                result = modification; 
            }
            return result; 
        }
    }

    public static class ModificationsExtensions
    {
        public static bool ModificationsEqual(this Dictionary<int, string> modDict1, Dictionary<int, string> modDict2)
        {
            bool isEqual = false;
            // iterate through dictionaries and convert the label modification to just "label"
            if (modDict1.Count == modDict2.Count)
            {
                var dict1 = modDict1.ReplaceLabelStringWithLabel();
                var dict2 = modDict2.ReplaceLabelStringWithLabel();
                isEqual = dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
            }
            return isEqual;
        }
        public static bool ModificationsEqual(ModificationClass mod1, ModificationClass mod2)
        {
            return mod1.ModificationDict.ModificationsEqual(mod2.ModificationDict); 
        }
        public static Dictionary<int, string> ReplaceLabelStringWithLabel(this Dictionary<int, string> modDict1)
        {
            for (int i = 0; i < modDict1.Count; i++)
            {
                if (ModificationIsLabel(modDict1.Values.ElementAt(i)))
                {
                    string fromValue = modDict1.Values.ElementAt(i);
                    string toValue = "label";
                    int positionValue = modDict1.ElementAt(i).Key;
                    modDict1.Remove(positionValue);
                    modDict1.Add(positionValue, toValue);
                }
                else
                {
                    continue;
                }
            }
            return modDict1;
        }

        public static bool ModificationIsLabel(string modification)
        {
            bool modIsLabel = false;
            LabelDictionary labels = new LabelDictionary();
            bool success = labels.LabelDict.ContainsKey(modification);
            if (!success)
            {
                modIsLabel = false;
            }
            else
            {
                modIsLabel = true;
            }
            return modIsLabel;
        }
    }
        
}
