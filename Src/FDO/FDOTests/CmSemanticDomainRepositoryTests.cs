﻿// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2012, SIL International. All Rights Reserved.
// <copyright from='2012' to='2012' company='SIL International'>
//		Copyright (c) 2012, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: CmSemanticDomainRepositoryTests.cs
// Responsibility:
// Last reviewed:
//
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SIL.FieldWorks.FDO.Application.ApplicationServices;
using NUnit.Framework;

namespace SIL.FieldWorks.FDO.FDOTests
{
	/// <summary>
	/// Test stuff related to Possibility List Repository functions.
	/// </summary>
	[TestFixture]
	public class CmSemanticDomainRepositoryTests : MemoryOnlyBackendProviderRestoredForEachTestTestBase
	{
		private ICmSemanticDomainRepository m_semdomRepo;

		private const string XMLTESTDATA =
	@"<?xml version='1.0' encoding='UTF-8'?>
		<LangProject>
			<SemanticDomainList>
				<CmPossibilityList>
					<ItemClsid><Integer val='66'/></ItemClsid>
					<WsSelector><Integer val='-3'/></WsSelector>
					<Name>
						<AUni ws='en'>Semantic Domains</AUni>
					</Name>
					<Abbreviation>
						<AUni ws='en'>Sem</AUni>
					</Abbreviation>
					<Possibilities>
						<CmSemanticDomain guid='63403699-07C1-43F3-A47C-069D6E4316E5'>
							<Abbreviation>
								<AUni ws='en'>1</AUni>
							</Abbreviation>
							<Name>
								<AUni ws='en'>Universe, creation</AUni>
							</Name>
							<Description>
								<AStr ws='en'>
									<Run ws='en'>Use this domain for the physical universe.</Run>
								</AStr>
							</Description>
							<Questions>
								<CmDomainQ>
									<ExampleWords>
										<AUni ws='en'>universe, creation, cosmos, heaven and earth, macrocosm, everything that exists</AUni>
									</ExampleWords>
								</CmDomainQ>
							</Questions>
							<SubPossibilities>
								<CmSemanticDomain guid='999581C4-1611-4ACB-AE1B-5E6C1DFE6F0C'>
									<Abbreviation>
										<AUni ws='en'>1.1</AUni>
									</Abbreviation>
									<Name>
										<AUni ws='en'>Sky</AUni>
									</Name>
									<Description>
										<AStr ws='en'>
											<Run ws='en'>Use this domain for words related to the sky.</Run>
										</AStr>
									</Description>
									<Questions>
										<CmDomainQ>
											<ExampleWords>
												<AUni ws='en'>sky, firmament</AUni>
											</ExampleWords>
										</CmDomainQ>
										<CmDomainQ>
											<ExampleWords>
												<AUni ws='en'>air, atmosphere</AUni>
											</ExampleWords>
										</CmDomainQ>
									</Questions>
									<SubPossibilities>
										<CmSemanticDomain guid='DC1A2C6F-1B32-4631-8823-36DACC8CB7BB'>
											<Abbreviation>
												<AUni ws='en'>1.1.8</AUni>
											</Abbreviation>
											<Name>
												<AUni ws='en'>Sun</AUni>
											</Name>
											<Description>
												<AStr ws='en'>
													<Run ws='en'>words related to the sun.</Run>
												</AStr>
											</Description>
											<Questions>
												<CmDomainQ>
													<ExampleWords>
														<AUni ws='en'>sun, solar</AUni>
													</ExampleWords>
												</CmDomainQ>
											</Questions>
										</CmSemanticDomain>
									</SubPossibilities>
								</CmSemanticDomain>
							</SubPossibilities>
						</CmSemanticDomain>
						<CmSemanticDomain guid='1C3F8996-362E-4EE0-AF02-0DD02887F6AA'>
							<Abbreviation>
								<AUni ws='en'>4.9.6</AUni>
							</Abbreviation>
							<Name>
								<AUni ws='en'>Heaven, hell</AUni>
							</Name>
							<Description>
								<AStr ws='en'>
									<Run ws='en'>words related to heaven and hell.</Run>
								</AStr>
							</Description>
							<Questions>
								<CmDomainQ>
									<ExampleWords>
										<AUni ws='en'>Heaven, highest heaven, abode of God</AUni>
									</ExampleWords>
								</CmDomainQ>
							</Questions>
						</CmSemanticDomain>
						<CmSemanticDomain guid='4BF411B7-2B5B-4673-B116-0E6C31FBD08A'>
							<Abbreviation>
								<AUni ws='en'>8.3.3</AUni>
							</Abbreviation>
							<Name>
								<AUni ws='en'>Light</AUni>
							</Name>
							<Description>
								<AStr ws='en'>
									<Run ws='en'>words related to light.</Run>
								</AStr>
							</Description>
							<Questions>
								<CmDomainQ>
									<ExampleWords>
										<AUni ws='en'>light, sunshine</AUni>
									</ExampleWords>
								</CmDomainQ>
							</Questions>
							<SubPossibilities>
								<CmSemanticDomain guid='A7824686-A3F3-4C8A-907E-5D841CF846C8'>
									<Abbreviation>
										<AUni ws='en'>8.3.3.1</AUni>
									</Abbreviation>
									<Name>
										<AUni ws='en'>Shine</AUni>
									</Name>
									<Description>
										<AStr ws='en'>
											<Run ws='en'>light source shining something to make light.</Run>
										</AStr>
									</Description>
									<Questions>
										<CmDomainQ>
											<ExampleWords>
												<AUni ws='en'>shine, glow (n), glow-worm, it's alive!</AUni>
											</ExampleWords>
										</CmDomainQ>
									</Questions>
								</CmSemanticDomain>
							</SubPossibilities>
						</CmSemanticDomain>
					</Possibilities>
				</CmPossibilityList>
			</SemanticDomainList>
		</LangProject>";

		private const string WRONG_NUMBER_OF_MATCHES = "Found the wrong number of results";
		private const string WRONG_SEMDOM_NUMBER = "Wrong Semantic Domain number returned.";
		private const string WRONG_SEMDOM_NAME = "Wrong Semantic Domain name returned.";

		/// <summary>
		/// Load the test data into the Semantic Domains list.
		/// </summary>
		protected override void CreateTestData()
		{
			base.CreateTestData();

			Cache.ActionHandlerAccessor.EndUndoTask(); // I'll make my own Undo tasks, since ImportList has its own.
			m_semdomRepo = Cache.ServiceLocator.GetInstance<ICmSemanticDomainRepository>();

			LoadSemDomTestData();
		}

		private void LoadSemDomTestData()
		{
			LoadSemDomTestData(XMLTESTDATA);
		}

		private void LoadSemDomTestData(string xmlSemDomData)
		{
			using (var trdr = new StringReader(xmlSemDomData))
			{
				var loader = new XmlList();
				loader.ImportList(Cache.LangProject, "SemanticDomainList", trdr, new DummyProgressDlg());
			}
		}

		/// <summary>
		/// Test finding matching semantic domains using an alphabetic search string.
		/// </summary>
		[Test]
		public void FindOneDomain_Alpha_WholeWordMatchesGroup1CaseInsensitive()
		{
			//Setup
			const string searchString = "sky";
			const string expectedNum = "1.1"; // group1 match
			const string expectedName = "Sky";

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			var cresult = resultList.Count;
			Assert.AreEqual(1, cresult, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedName, resultList[0].Name.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NAME);
		}

		/// <summary>
		/// Test finding partial match in 2nd question Example Words.
		/// </summary>
		[Test]
		public void FindOneDomain_Alpha_PartialWordMatchesExampleInSecondQuestion()
		{
			//Setup
			const string searchString = "atmos";
			const string expectedNum = "1.1"; // group3 match
			const string expectedName = "Sky";

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			var cresult = resultList.Count;
			Assert.AreEqual(1, cresult, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test finding matching semantic domains using 'shine' as the alphabetic search string.
		/// </summary>
		[Test]
		public void FindOneDomain_Alpha_DoesNotMatchFinally()
		{
			//Setup
			const string searchString = "shine";
			const string expectedNum1 = "8.3.3.1"; // group1 match 'shine'
			// specifically should NOT match 'sunshine'

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			var cresult = resultList.Count;
			Assert.AreEqual(1, cresult, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum1, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test finding matching semantic domains using a short alphabetic search string.
		/// </summary>
		[Test]
		public void FindDomains_Alpha_Groups1And3()
		{
			//Setup
			const string searchString = "sun";
			const string expectedNum1 = "1.1.8"; // group1 match 'sun'
			const string expectedName1 = "Sun";
			const string expectedNum2 = "8.3.3"; // group3 match 'sunshine', under 'Light'
			const string expectedName2 = "Light";

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			var cresult = resultList.Count;
			Assert.AreEqual(2, cresult, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum1, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedName1, resultList[0].Name.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NAME);
			Assert.AreEqual(expectedNum2, resultList[1].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedName2, resultList[1].Name.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NAME);
		}

		/// <summary>
		/// Test finding matching semantic domains using 's' as the alphabetic search string.
		/// </summary>
		[Test]
		public void FindDomains_Alpha_SingleChar_SeveralMatchesButNotDuplicateExampleWord()
		{
			//Setup
			const string searchString = "s";
			const string expectedNum1 = "1.1";     // group1 match 'sky'
			const string expectedNum2 = "1.1.8";   // group1 match 'sun'
			const string expectedNum3 = "8.3.3.1"; // group1 match 'shine'
			// no group2 matches
			const string expectedNum4 = "8.3.3";   // group3 match 'sunshine', under 'Light'
			// should NOT get an additional match for 'solar', because '1.1.8 Sun' is already a group1 match.

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			Assert.AreEqual(4, resultList.Count, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum1, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedNum2, resultList[1].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedNum3, resultList[2].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedNum4, resultList[3].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test that finding group2 matching semantic domains in Example Words is case insensitive.
		/// </summary>
		[Test]
		public void FindDomain_Alpha_TestGroup2CaseInsensitivity_ExampleWords()
		{
			//Setup
			const string searchString = "Sunshine";
			// no group1 or group3 matches
			const string expectedNum = "8.3.3";   // group2 match 'sunshine', under 'Light'

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			Assert.AreEqual(1, resultList.Count, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test that finding group2 matching semantic domains in Name (not first word) is case insensitive.
		/// </summary>
		[Test]
		public void FindDomain_Alpha_TestGroup2CaseInsensitivity_Name()
		{
			//Setup
			const string searchString = "Creation";
			// no group1 or group3 matches
			const string expectedNum = "1";   // group2 match 'creation', after 'Universe'

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			Assert.AreEqual(1, resultList.Count, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test finding a group3 matching semantic domain in Name (not first word).
		/// </summary>
		[Test]
		public void FindDomain_Alpha_TestGroup3_Name()
		{
			//Setup
			const string searchString = "cre";
			// no group1 or group2 matches
			const string expectedNum = "1";   // group3 match 'creation', after 'Universe'

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			Assert.AreEqual(1, resultList.Count, WRONG_NUMBER_OF_MATCHES); // should not have group1or2
			Assert.AreEqual(expectedNum, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test finding and sorting a group2 match and a group1 match.
		/// </summary>
		[Test]
		public void FindDomain_Alpha_TestGroup1AndGroup2Difference()
		{
			//Setup
			const string searchString = "heaven";
			const string expectedNum1 = "4.9.6"; // group1 match 'Heaven, hell'
			const string expectedNum2 = "1";     // group2 match 'heaven and earth', under 'Universe, creation'
			// no group3 matches

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			Assert.AreEqual(2, resultList.Count, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum1, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedNum2, resultList[1].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test that searching for 'n' does not match (n).
		/// </summary>
		[Test]
		public void DontFindDomain_Alpha_SurroundedByParentheses()
		{
			//Setup
			const string searchString = "n";
			// no matches expected

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			Assert.AreEqual(0, resultList.Count, WRONG_NUMBER_OF_MATCHES);
		}

		/// <summary>
		/// Test that searching for "glow-worm" works.
		/// </summary>
		[Test]
		public void FindDomain_Alpha_Hyphen()
		{
			//Setup
			const string searchString = "glow-worm";
			// no group1 or group3 matches
			const string expectedNum = "8.3.3.1";   // group2 match 'glow-worm', under 'Shine'

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			Assert.AreEqual(1, resultList.Count, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test that searching for "it's" works.
		/// </summary>
		[Test]
		public void FindDomain_Alpha_EmbeddedSingleQuote()
		{
			//Setup
			const string searchString = "it's";
			// no group1 or group3 matches
			const string expectedNum = "8.3.3.1";   // group2 match "it's", under 'Shine'

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			Assert.AreEqual(1, resultList.Count, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
		}

		/// <summary>
		/// Test finding matching semantic domains using a numeric search string.
		/// </summary>
		[Test]
		public void FindDomains_NumericSearchString_DoesntMatchFinally()
		{
			//Setup
			const string searchString = "8";
			const string expectedNum1 = "8.3.3";
			const string expectedName1 = "Light";
			const string expectedNum2 = "8.3.3.1";
			const string expectedName2 = "Shine";
			// specifically should NOT match '1.1.8'

			// SUT
			var result = m_semdomRepo.FindDomainsThatMatch(searchString);

			// Verification
			var resultList = result.ToList();
			var cresult = resultList.Count;
			Assert.AreEqual(2, cresult, WRONG_NUMBER_OF_MATCHES);
			Assert.AreEqual(expectedNum1, resultList[0].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedName1, resultList[0].Name.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NAME);
			Assert.AreEqual(expectedNum2, resultList[1].Abbreviation.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NUMBER);
			Assert.AreEqual(expectedName2, resultList[1].Name.BestAnalysisAlternative.Text,
				WRONG_SEMDOM_NAME);
		}
	}
}