using CTRFramework.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace CTRFramework_Tests
{
    [TestClass]
    public class PsxPtrTests
    {
        List<PsxPtr> initial = new List<PsxPtr>();
        List<PsxPtr> verify = new List<PsxPtr>();
        string filenameTest = Helpers.PathCombine(Meta.BasePath, "psxptrtest.bin");

        private void GenerateTestData()
        {
            Console.WriteLine("Original test pointers:");

            for (int i = 0; i < 4; i++)
            {
                PsxPtr ptr = new PsxPtr((uint)Helpers.Random.Next(600000));
                ptr.ExtraBits = (HiddenBits)i;

                initial.Add(ptr);

                Console.WriteLine(ptr.ToString());
            }
        }

        /// <summary>
        /// Writes pointers to file.
        /// </summary>
        [TestMethod]
        public void PsxPtrWriteTest()
        {
            GenerateTestData();

            using (var bw = new BinaryWriterEx(File.OpenWrite(filenameTest)))
            {
                foreach (var ptr in initial)
                {
                    ptr.Write(bw, null);
                }
            }

            Assert.IsTrue(File.Exists(filenameTest), "Test file doesn't exist.");

            PsxPtrReadTest();
        }

        /// <summary>
        /// Reads pointers from file and compares to the original.
        /// </summary>
        public void PsxPtrReadTest()
        {
            Console.WriteLine("Verification pointers:");

            using (var br = new BinaryReaderEx(File.OpenRead(filenameTest)))
            {
                for (int i = 0; i < initial.Count; i++)
                {
                    verify.Add(PsxPtr.FromReader(br));
                }
            }

            Assert.AreEqual(initial.Count, verify.Count, 0, "Num pointers mismatch.");

            for (int i = 0; i < initial.Count; i++)
            {
                Assert.IsTrue(
                    initial[i] != verify[i],
                    $"PsxPtr value {i} mismatch: {initial[i].Address} vs {verify[i].Address}, {initial[i].ExtraBits} vs {verify[i].ExtraBits}"
                );

                Console.WriteLine(verify[i].ToString());
            }

            CleanUp();
        }

        private void CleanUp()
        {
            initial.Clear();
            verify.Clear();

            if (File.Exists(filenameTest))
                File.Delete(filenameTest);
        }
    }
}
