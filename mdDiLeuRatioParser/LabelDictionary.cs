using System;
using System.Collections.Generic; 
namespace mdDiLeuRatioParser
{
    public class LabelDictionary
    {
        public Dictionary<string, LabelTypes> LabelDict { get; private set; }
        // [Light Label:mdDL 801 on X]
        // [Heavy Label:mdDL 090 on X]
        // [Med Label:mdDL 261 on X]
        
        public LabelDictionary()
        {
            LabelDict = new Dictionary<string, LabelTypes>(); 
            string lightLabelString = "Light Label:mdDL 801 on X";
            string heavyLabelString = "Heavy Lable:mdDL 090 on X";
            string medLabelString = "Med Label:mdDL 261 on X";

            LabelDict.Add(lightLabelString, LabelTypes.Light);
            LabelDict.Add(medLabelString, LabelTypes.Medium);
            LabelDict.Add(heavyLabelString, LabelTypes.Heavy);
        }
    }
}
