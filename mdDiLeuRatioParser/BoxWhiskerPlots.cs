using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Data;
using System.Threading;
using OxyPlot.Wpf;
using OxyPlot.SkiaSharp;

namespace mdDiLeuRatioParser
{
    public class BoxWhiskerPlots
    {
		public static void CreateDiLeuBoxPlot(string importPath, LabelTypes label1, LabelTypes label2, string exportPath)
        {
			var model = CreateBoxPlot(importPath, label1, label2);
			WritePlotModelToPng(model, exportPath); 
        }
		/// <summary>
		/// Creates a boxplot from the .tsv file output of Program file. This was preferable because the 
		/// ratio dictionary output is a dictionary of concatenated strings, so the values of label 1 and label 2
		/// aren't accessible without a regex and I didn't really want to dive in on the regex testing. 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="label1"></param>
		/// <param name="label2"></param>
		/// <returns></returns>
		public static PlotModel CreateBoxPlot(string path, LabelTypes label1, LabelTypes label2)
		{
			// read the tsv file
			// extract the label
			// make two lists of doubles, one for each label 
			// make the two plots, one for each label 

			DataTable dt = FilePreprocessing.ReadTSVFile(path);
			// clearly this was before I knew what pass by reference was 
			dt = FilePreprocessing.CorrectRatiosTSVColumns(dt);

			List<double> label1List = new();
			List<double> label2List = new();

			var label1String = label1.ToString(); 
			var label2String = label2.ToString();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				var temp = (string)dt.Rows[i]["Label1"];
				var tempVal = (double)dt.Rows[i]["Intensity Ratio"]; 
				if(temp == label1String)
                {
					label1List.Add(tempVal); 
                }else if(temp == label2String)
                {
					label2List.Add(tempVal); 
                }
			}

			var model = new PlotModel() { Title = "Ratio BoxPlot"};
			var series1 = new BoxPlotSeries
			{
				BoxWidth = 0.5
			};

			series1.Items.Add(CreateBoxPlotItem(label1List, 0D));
			series1.Items.Add(CreateBoxPlotItem(label2List, 1D));
			var logAxis = new LogarithmicAxis()
			{
				Position = AxisPosition.Left,
				Base = 2
			};
			model.Axes.Add(logAxis); 
            var categoryAxis = new CategoryAxis()
			{
				Position = AxisPosition.Bottom, MinimumPadding = 0.1, MaximumPadding = 0.1
            };

            string[] labels = new string[]
            {
                LabelTypes.Light.ToString() + " vs " + LabelTypes.Medium.ToString(),
				LabelTypes.Light.ToString() + " vs " + LabelTypes.Heavy.ToString()

            };

            foreach(var label in labels)
            {
				categoryAxis.Labels.Add(label);
            }
			model.Axes.Add(categoryAxis);
			model.Series.Add(series1); 
			// create method for writing the plot model to pdf or png or whatever
			return model; 
		}
		public static void WritePlotModelToPng(PlotModel model, string filePath)
        {
			var pdfExport = new OxyPlot.SkiaSharp.PdfExporter();
			pdfExport.Width = 72*5; pdfExport.Height = 72 * 4;
			pdfExport.ExportToFile(model, filePath); 

        }
		private static BoxPlotItem CreateBoxPlotItem(List<double> valsList, double boxNumber)
		{
			// box number 
			// lower whisker
			// firstQuartile
			// median 
			// third quartile 
			// upperWhisker
			// outliers

			// 1) Median 
			valsList.Sort();
			var median = GetMedian(valsList);

			int r = valsList.Count % 2;
			double firstQuart = GetMedian(valsList.Take((valsList.Count + r) / 2));
			double thirdQuart = GetMedian(valsList.Skip((valsList.Count - r) / 2));

			var iqr = thirdQuart - firstQuart;
			var step = iqr * 1.5;
			var upperWhisk = thirdQuart + step;
			upperWhisk = valsList.Where(v => v <= upperWhisk).Max();
			var lowerWhisk = firstQuart - step;
			lowerWhisk = valsList.Where(v => v >= lowerWhisk).Min();
			var outliers = valsList.Where(v => v > upperWhisk || v < lowerWhisk).ToList();
			return new BoxPlotItem(boxNumber, lowerWhisk, firstQuart, median, thirdQuart, upperWhisk);
		}
		public static double GetMedian(IEnumerable<double> values)
		{
			var sortedInterval = new List<double>(values);
			sortedInterval.Sort();
			var count = sortedInterval.Count;
			if (count % 2 == 1)
			{
				return sortedInterval[(count - 1) / 2];
			}

			return 0.5 * sortedInterval[count / 2] + 0.5 * sortedInterval[(count / 2) - 1];
		}
	}
}
