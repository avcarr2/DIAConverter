using System;
using System.Linq;
using System.Collections.Generic; 
namespace mdDiLeuRatioParser
{
    // before initialization of this class, you need to perform checks that the
    // peaks are quantifiable. 
    public class PeakRatio
    {
        string BaseSequence { get; set; }
        RetentionTimeWindow RTWindow {get; set; }
        (LabelTypes Label1, LabelTypes Label2, double Ratio) LabelAndRatio { get; set; } 

        public PeakRatio(QuantifiedPeak peak1, QuantifiedPeak peak2, RetentionTimeWindow rtWindow)
        {

        }

        public void CalculateRatio(QuantifiedPeak peak1, QuantifiedPeak peak2)
        {

        }
    }
}
