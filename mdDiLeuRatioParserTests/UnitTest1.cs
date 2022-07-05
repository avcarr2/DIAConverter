using NUnit.Framework;
using System.Linq;
using mdDiLeuRatioParser;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NUnit;
using System.Text.RegularExpressions;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows.Threading;
using System; 
namespace mdDiLeuRatioParserTests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
		}
		[Test]
		public void TestReadPSMTVFile()
		{

		}
		[Test]
		public void TestCalculateRatio()
		{

		}
		public void TestRatio090Over261()
		{
			List<QuantifiedPeak> qpList = new List<QuantifiedPeak>() {
				new QuantifiedPeak("PEPTIDE", "PEP[mod]TIDE", 25, 2500),
				new QuantifiedPeak("PEPTIDE", "PEP[mod]TIDE", 25, 5000),
				new QuantifiedPeak("PEPTIDE", "PEP[mod]TIDE", 25, 7500),
				new QuantifiedPeak("PEPTIDE", "P[mod]EPTIDE", 25, 10000),
				new QuantifiedPeak("SEQUENCE", "[Heavy Label:mdDL 090 on X]SEQUENCE", 25, 2500),
				new QuantifiedPeak("SEQUENCE", "[Light Label:mdDL 801 on X]SEQUENCE", 25, 5000),
				new QuantifiedPeak("SEQUENCE", "[Med Label:mdDL 261 on X]SEQUENCE", 25, 7500)
			};

			


		}
		public void TestRatio801Over261()
		{

		}
		[Test]
		public void TestCalculateRatios()
		{
			Dictionary<string, List<QuantifiedPeak>> dict = new();

			List<QuantifiedPeak> qpList = new List<QuantifiedPeak>() {
				new QuantifiedPeak("PEPTIDE", "PEP[mod]TIDE", 25, 2500),
				new QuantifiedPeak("PEPTIDE", "PEP[mod]TIDE", 25, 5000),
				new QuantifiedPeak("PEPTIDE", "PEP[mod]TIDE", 25, 7500),
				new QuantifiedPeak("PEPTIDE", "P[mod]EPTIDE", 25, 10000),
				new QuantifiedPeak("SEQUENCE", "[Heavy Label:mdDL 090 on X]SEQUENCE", 25, 2500),
				new QuantifiedPeak("SEQUENCE", "[Light Label:mdDL 801 on X]SEQUENCE", 25, 5000),
				new QuantifiedPeak("SEQUENCE", "[Med Label:mdDL 261 on X]SEQUENCE", 25, 7500)
			};

			List<string> baseSequences = qpList.Select(i => i.BaseSequence).Distinct().ToList();
			foreach (string baseSeq in baseSequences)
			{
				dict.Add(baseSeq, qpList.Where(i => i.BaseSequence == baseSeq).ToList());
			}

			var results = dict.CalculateRatio();
			// There should be two base sequences. 
			var assertionTestResults = new Dictionary<string, double>();
			results.TryGetValue("SEQUENCE", out assertionTestResults);
			double[] expectedArray = { 0.3333, 0.6666 }; 
			
			// There should be no quantification in the unlabelled
			// There should be only quantified sequence peptides. 

		}
		[Test]
		public void TestSelectByLabelType()
		{
			List<QuantifiedPeak> qpList = new List<QuantifiedPeak>() {
				new QuantifiedPeak("SEQUENCE", "[Heavy Label:mdDL 090 on X]SEQUENCE", 25, 2500),
				new QuantifiedPeak("SEQUENCE", "[Med Label:mdDL 261 on X]SEQUENCE", 25, 7500)
			};

			var labelType1 = qpList.Where(i => (int)i.LabelType == (int)LabelTypes.Medium).ToList();
			Assert.AreEqual(1, labelType1.Count); 
		}

		[Test]
		public void TestCalculateRatiosFromList()
		{
			List<QuantifiedPeak> qpList = new List<QuantifiedPeak>() {
				new QuantifiedPeak("SEQUENCE", "[Heavy Label:mdDL 090 on X]SEQUENCE", 25, 2500),
				new QuantifiedPeak("SEQUENCE", "[Med Label:mdDL 261 on X]SEQUENCE", 25, 7500)
			};
			var results = QuantifiedPeakExtensions.CalculateRatiosFromList(qpList, LabelTypes.Heavy, LabelTypes.Medium);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual(0.3333, results.ElementAt(0).Value, 0.0001); 
		}

		[Test]
		public void TestCheckModificationsAreEquivalent()
		{

			QuantifiedPeak pep1 = new QuantifiedPeak("PEPTIDE", "PEP[mod]TIDE", 25, 250000);
			QuantifiedPeak pep2 = new QuantifiedPeak("PEPTIDE", "PEP[mod]TIDE", 25, 250000);
			QuantifiedPeak pep3 = new QuantifiedPeak("PEPTIDE", "P[mod]EPTIDE", 25, 250000);
			QuantifiedPeak labelledHeavy = new QuantifiedPeak("PEPTIDE", "[Heavy Label:mdDL 090 on X]PEPTIDE", 25, 250000);
			QuantifiedPeak labelledLight = new QuantifiedPeak("PEPTIDE", "[Light Label:mdDL 801 on X]PEPTIDE", 25, 250000);
			Assert.IsTrue(pep1.CheckModificationsAreEquivalent(pep2));
			Assert.IsFalse(pep1.CheckModificationsAreEquivalent(pep3));
			Assert.IsTrue(labelledHeavy.CheckModificationsAreEquivalent(labelledLight)); 
		}

		[Test]
		public void TestCheckPeaksAreQuantifiable()
		{
			var qp1 = new QuantifiedPeak(("PEPTIDE", "PEP[mod]TIDE", 25.9, 250000.90));
			var qp2 = new QuantifiedPeak(("PEPTIDE", "PEP[mod]TIDE", 25.9, 250000.90));
			var qp3 = new QuantifiedPeak(("PEPTIDE", "PE[mod]PTIDE", 25.9, 250000.90));
			QuantifiedPeak labelledHeavy = new QuantifiedPeak("PEPTIDE", "[Heavy Label:mdDL 090 on X]PEPTIDE", 25, 250000);
			QuantifiedPeak labelledLight = new QuantifiedPeak("PEPTIDE", "[Light Label:mdDL 801 on X]PEPTIDE", 25, 250000);
			QuantifiedPeak lightShifted = new QuantifiedPeak("PEPTIDE", "P[Light Label:mdDL 801 on X]EPTIDE", 25, 250000); 
			
			Assert.IsTrue(qp1.CheckPeaksAreQuantifiable(qp2));
			Assert.IsFalse(qp1.CheckPeaksAreQuantifiable(qp3));
			Assert.IsFalse(labelledHeavy.CheckPeaksAreQuantifiable(lightShifted));
		}

		[Test]
		[TestCase("Light Label:mdDL 801 on X", true)]
		[TestCase("mod", false)]
		public void TestModificationIsLabel(string seq, bool truthValue)
		{
			QuantifiedPeak qp = new QuantifiedPeak();
			bool isLabel = qp.ModificationIsLabel(seq);
			Assert.AreEqual(truthValue, isLabel); 
		}

		[Test]
		[TestCase(@"Light Label:mdDL 801 on X", 0)]
		[TestCase(@"Heavy Label:mdDL 090 on X", 2)]
		[TestCase(@"Med Label:mdDL 261 on X", 1)]
		[TestCase(@"mod", 3)]
		public void TestAssignModificationType(string seq, int enumVal)
		{
			QuantifiedPeak qp = new();
			qp.AssignModificationLabelType(seq); 
			Assert.AreEqual(enumVal, (int)qp.LabelType); 
		}

		[Test]
		[TestCase(@"[Light Label:mdDL 801 on X]PEPTIDE", 0)]
		[TestCase(@"[Heavy Label:mdDL 090 on X]PEPTIDE", 2)]
		[TestCase(@"[Med Label:mdDL 261 on X]PEPTIDE", 1)]
		public void TestAssignModificationLabelType(string seq, int enumVal)
		{
			QuantifiedPeak testQp = new QuantifiedPeak("PEPTIDE", seq, 25, 25000000);
			testQp.ClassifyAndAssignLabelType();
			Assert.AreEqual(enumVal, (int)testQp.LabelType);
		}
		[Test]
		[TestCase(@"[Acetylation]PEPTIDE", 1, new string[] {"Acetylation"}, new int[] { 0 })]
		[TestCase(@"[Acetylation]PEPT[Phosphorylation]IDE",2, new string[] { "Acetylation", "Phosphorylation" }, new int[] { 0, 4})]
		public void TestParseModifications(string fullSeq, int expectedCount, string[] expectedMods, int[] position)
		{
			ModificationClass modClassTest = new();
			modClassTest.ParseModifications(fullSeq);

			Assert.AreEqual(expectedCount, modClassTest.ModificationDict.Count);
			Assert.AreEqual(position, modClassTest.ModificationDict.Keys.ToArray());
			Assert.AreEqual(expectedMods, modClassTest.ModificationDict.Values.ToArray()); 
		}
		[Test]
		public void TestImportTSVFile()
		{
			string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "DataFiles", "AllQuantifiedPeaks.tsv");
			DataTable dt = FilePreprocessing.ReadPSMTVFile(path);
			DataTable newTable = FilePreprocessing.CorrectColumnType(dt);

			string[] correctClassTypesForFirstFiveColumns = { "System.String", "System.String", "System.String", "System.String", "System.Double" };
			string[] dataTableFirstFiveClassTypes = new string[5]; 

			for(int i = 0; i < dataTableFirstFiveClassTypes.Length; i++)
			{
				dataTableFirstFiveClassTypes[i] = newTable.Columns[i].DataType.ToString(); 
			}


			string originalFirstRow = string.Join(", ", dt.Rows[0].ItemArray);
			string copiedFirstRow = string.Join(", ", newTable.Rows[0].ItemArray);

			Assert.AreEqual(correctClassTypesForFirstFiveColumns, dataTableFirstFiveClassTypes);
			Assert.AreEqual(originalFirstRow, copiedFirstRow); 

		}
		[Test]
		[TestCase("<mdDL_1109_111-calib>", true)]
		[TestCase(@"<mdDL_1109_111-calib>", true)]
		[TestCase(@"1174.742169", false)]
		public void TestRegex(string testString, bool truthValue)
		{
			Regex regex = new Regex(@"[^0-9.]"); 
			Assert.AreEqual(truthValue, regex.IsMatch(testString)); 
		}
		[Test]
		[TestCase(@"1174.742169", true)]
		[TestCase("-", false)]
		public void TestTryParse(string val, bool truthValue)
		{
			bool exptlTruthValue = double.TryParse(val, out double retVal);
			Assert.AreEqual(truthValue, exptlTruthValue); 
			
		}


		[Test]
		public void TestPrintRatioResultsToTextFile()
		{
			string outputPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "DataFiles", "mdDiLeuTestResults1.tsv");

			Dictionary<string, double> subDictionary = new();
			subDictionary.Add("test string", 11.53157);
			subDictionary.Add("test string2", 12.9874654);

			Dictionary<string, Dictionary<string, double>> fullDictionary = new();
			fullDictionary.Add("base sequence", subDictionary); // full dictionary is correctly initialized
			fullDictionary.Add("base sequence2", subDictionary);
			fullDictionary.PrintRatioResultsToTextFile(outputPath); 

		}
		[Test]
		public void TestFileWriting()
		{
			string outputPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "DataFiles", "mdDiLeuTestResults2.tsv");

			Dictionary<string, double> subDictionary = new();
			subDictionary.Add("test string", 11.53157);
			subDictionary.Add("test string2", 12.9874654); 

			Dictionary<string, Dictionary<string, double>> fullDictionary = new();
			fullDictionary.Add("base sequence", subDictionary); // full dictionary is correctly initialized
			fullDictionary.Add("base sequence2", subDictionary);

			using (TextWriter writer = new StreamWriter(outputPath))
			{
				foreach(var baseSeq in fullDictionary.Keys)
				{
					foreach(var fullDictVals in fullDictionary[baseSeq])
					{
						writer.WriteLine("{0}\t{1}\t{2}", baseSeq, fullDictVals.Key.ToString(), fullDictVals.Value.ToString()); 
					}
				}
			}
		}
        [Test]        
		public void TestBoxPlot()
        {
			string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "DataFiles", "mdDiLeuRatiosOutput.tsv");
			PlotModel pm = BoxPlot();
			PlotModel pm2 = BoxWhiskerPlots.CreateBoxPlot(filePath, LabelTypes.Light, LabelTypes.Heavy); 
			BoxWhiskerPlots.WritePlotModelToPng(pm, "testBoxPlot.pdf");
			BoxWhiskerPlots.WritePlotModelToPng(pm2, "mdDiLeuBoxplot.pdf"); 
        }
		public static PlotModel BoxPlot()
		{
			const int boxes = 10;

			var model = new PlotModel() { Title = string.Format("BoxPlot (n={0})", boxes) };

			var s1 = new BoxPlotSeries
			{
				Title = "BoxPlotSeries",
				BoxWidth = 0.1
			};

			var random = new Random();
			for (var i = 0; i < boxes; i++)
			{
				double x = i;
				var points = 5 + random.Next(15);
				var values = new List<double>();
				for (var j = 0; j < points; j++)
				{
					values.Add(random.Next(0, 20));
				}

				values.Sort();
				var median = BoxWhiskerPlots.GetMedian(values);
				int r = values.Count % 2;
				double firstQuartil = BoxWhiskerPlots.GetMedian(values.Take((values.Count + r) / 2));
				double thirdQuartil = BoxWhiskerPlots.GetMedian(values.Skip((values.Count - r) / 2));

				var iqr = thirdQuartil - firstQuartil;
				var step = iqr * 1.5;
				var upperWhisker = thirdQuartil + step;
				upperWhisker = values.Where(v => v <= upperWhisker).Max();
				var lowerWhisker = firstQuartil - step;
				lowerWhisker = values.Where(v => v >= lowerWhisker).Min();

				var outliers = values.Where(v => v > upperWhisker || v < lowerWhisker).ToList();

				s1.Items.Add(new BoxPlotItem(x, lowerWhisker, firstQuartil, median, thirdQuartil, upperWhisker));
			}

			model.Series.Add(s1);
			model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left});
			model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, MinimumPadding = 0.1, MaximumPadding = 0.1 });
			return model;
		}
		// write a method to count and plot the number of QuantifiedPeaks with light, medium, and heavy labels. 
	}
}