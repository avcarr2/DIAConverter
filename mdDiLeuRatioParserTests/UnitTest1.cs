using NUnit.Framework;
using System.Linq;
using mdDiLeuRatioParser;
using System.Collections.Generic; 

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
			Assert.IsFalse(labelledHeavy.CheckModificationsAreEquivalent(labelledLight)); 
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
	}
}