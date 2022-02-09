using System;
namespace mdDiLeuRatioParser
{
    public class RetentionTimeWindow
    {
        public double FirstRT { get; set; }
        public double LastRT { get; set; }

        public RetentionTimeWindow(double firstRT, double lastRT)
        {
            FirstRT = firstRT;
            LastRT = lastRT; 
        }
    }
}
