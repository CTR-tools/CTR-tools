using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CTRFramework.Lang;
using System.Collections.Generic;
using System.IO;
using CTRFramework.Shared;

namespace CTRFramework_UnitTests
{
    [TestClass]
    public class LangTests
    {
        List<string> testlist = new List<string>();
        int numStrings = 0;
        Random random = new Random();
        string filenameLng = Path.Combine(Meta.BasePath, "test.lng");
        string filenameText = Path.Combine(Meta.BasePath, "test.txt");

        private void GenerateTestData()
        {
            string teststring = "test";
            numStrings = random.Next(300) + 50;

            for (int i = 0; i < numStrings; i++)
            {
                testlist.Add(teststring + i.ToString("X4"));
            }
        }

        /// <summary>
        /// Tests LNG class creation. Populates with random data and saves a LNG file.
        /// </summary>
        [TestMethod]
        public void LangCreateTest()
        {
            GenerateTestData();

            using (LNG lng = new LNG())
            {
                Assert.IsNotNull(lng, "LNG class empty ctor failed.");

                lng.Entries.AddRange(testlist);
                lng.Save(filenameLng);
            }

            Assert.IsTrue(File.Exists(filenameLng), "Test file doesn't exist.");

            LangParseTest();
        }

        /// <summary>
        /// Parses the generated file and compares if obtained strings are equal to test data.
        /// </summary>
        public void LangParseTest()
        {
            using (LNG lng = LNG.FromFile(filenameLng))
            {
                Assert.IsNotNull(lng, "LNG class FromFile failed.");

                int numEntries = lng.Entries.Count;

                Assert.IsTrue(numEntries > 0, "Unexpected number of entries.");
                Assert.AreEqual(numStrings, numEntries, 0, "Num strings mismatch.");

                for (int i = 0; i < numEntries; i++)
                {
                    Assert.AreEqual(
                        testlist[i],
                        lng.Entries[i],
                        $"String {i} mismatch: {testlist[i]} vs {lng.Entries[i]}"
                        );
                }
            }

            LangTextExportTest();
        }

        /// <summary>
        /// Imports file and exports it as text, then compares the result and test data. 
        /// </summary>
        public void LangTextExportTest()
        {
            using (LNG lng = LNG.FromFile(filenameLng))
            {
                Assert.IsNotNull(lng, "LNG class FromFile failed.");

                lng.Export(filenameText);
            }

            Assert.IsTrue(File.Exists(filenameText));

            string[] parseddata = File.ReadAllLines(filenameText);

            int numEntries = parseddata.Length;

            Assert.IsTrue(numEntries > 0, "Unexpected number of entries.");
            Assert.AreEqual(numStrings, numEntries, 0, "Num strings mismatch.");

            for (int i = 0; i < numEntries; i++)
            {
                Assert.AreEqual(
                    testlist[i],
                    parseddata[i],
                    $"String {i} mismatch: {testlist[i]} vs {parseddata[i]}"
                    );
            }

            CleanUp();
        }

        private void CleanUp()
        {
            testlist.Clear();

            if (File.Exists(filenameLng))
                File.Delete(filenameLng);

            if (File.Exists(filenameText))
                File.Delete(filenameText);
        }
    }
}
