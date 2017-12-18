namespace SAF.LocalMachine.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.IO;

    /// <summary>
    ///This is a test class for ArchiveFilesCommandTest and is intended
    ///to contain all ArchiveFilesCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class ArchiveFilesCommandTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }
            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            foreach (string file in Directory.GetFiles(Properties.Settings.Default.ArchiveSourceDirectory))
            {
                File.Delete(file);
            }

            foreach (string file in Directory.GetFiles(Properties.Settings.Default.ArchiveTargetDirectory))
            {
                File.Delete(file);
            }

            foreach (string file in Directory.GetFiles(Properties.Settings.Default.SourceFilesDirectory))
            {
                File.Copy(
                    file, 
                    Path.Combine(Properties.Settings.Default.ArchiveSourceDirectory, Path.GetFileName(file)));
            }
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (string file in Directory.GetFiles(Properties.Settings.Default.ArchiveSourceDirectory))
            {
                File.Delete(file);
            }

            foreach (string file in Directory.GetFiles(Properties.Settings.Default.ArchiveTargetDirectory))
            {
                File.Delete(file);
            }
        }
        
        #endregion


        /////// <summary>
        ///////A test for ExecuteWithParameters
        ///////</summary>
        ////[TestMethod()]
        ////[DeploymentItem("SAF.LocalMachine.dll")]
        ////public void ExecuteWithParameters_DirectoryWithFiles_FilesLandInTarget()
        ////{
        ////    List<FileArchiveResult> results = ArchiveFiles();

        ////    foreach (FileArchiveResult result in results)
        ////    {
        ////        Assert.IsNotNull(result);
        ////        Assert.IsDefaultValue(result.ItemError, result.ItemError == null ? String.Empty : result.ItemError.Message);
        ////        Assert.IsNotNull(result.Target);
        ////        Assert.IsTrue(result.Result == ArchiveResult.Success, result.Result.ToString());
        ////        int compare = 
        ////            String.Compare(
        ////                result.Target.DirectoryName, Properties.Settings.Default.ArchiveTargetDirectory, true);

        ////        Assert.IsTrue(compare == 0);             
        ////    }
        ////}

        /////// <summary>
        ///////A test for ExecuteWithParameters
        ///////</summary>
        ////[TestMethod()]
        ////[DeploymentItem("SAF.LocalMachine.dll")]
        ////public void ExecuteWithParameters_DirectoryWithFiles_FilesAreNamedCorrectly()
        ////{
        ////    List<FileArchiveResult> results = ArchiveFiles();

        ////    foreach (FileArchiveResult result in results)
        ////    {
        ////        Assert.IsNotNull(result);
        ////        Assert.IsDefaultValue(result.ItemError, result.ItemError == null ? String.Empty : result.ItemError.Message);
        ////        Assert.IsNotNull(result.Target);
        ////        Assert.IsNotNull(result.Directive);
        ////        string expected =  
        ////            String.Format(
        ////                "{0}{1}{2}",
        ////                Path.GetFileNameWithoutExtension(result.Directive.Source.Name),
        ////                result.Target.LastWriteTime.ToString(result.Directive.Policy.NameFormat),
        ////                Path.GetExtension(result.Directive.Source.Name));

        ////        Assert.IsTrue(
        ////            String.Compare(result.Target.Name, expected, true) == 0, 
        ////            "Expected: '{0}' <> '{1}'", 
        ////            expected, 
        ////            result.Target.Name);
        ////    }
        ////}

        ////private static List<FileArchiveResult> ArchiveFiles()
        ////{
        ////    DirectoryInfo sourceDir = new DirectoryInfo(Properties.Settings.Default.ArchiveSourceDirectory);

        ////    Assert.IsTrue(sourceDir.GetFiles().Length > 0, "The directory '{0}' is empty.", sourceDir);

        ////    DirectoryInfo targetDir = new DirectoryInfo(Properties.Settings.Default.ArchiveTargetDirectory);
        ////    ArchivePolicy<DirectoryInfo> policy = new ArchivePolicy<DirectoryInfo>("yyyy_MM_dd", targetDir);

        ////    var directives = from x in sourceDir.GetFiles()
        ////                     select new FileArchiveDirective(policy, x);

        ////    FileArchiveManager target = new FileArchiveManager();
        ////    List<FileArchiveResult> results = new List<FileArchiveResult>();
        ////    object completeLock = new object();
        ////    bool completed = false;

        ////    target.ItemsProduced += delegate(Object o, ItemsProducedEventArgs e)
        ////    {
        ////        while (target.TaskResultConsumer.ConsumeNext())
        ////        {
        ////            results.Add(target.TaskResultConsumer.Current);
        ////        }
        ////    };

        ////    target.ProcessStopped += delegate(object o, ProcessStoppedEventArgs e)
        ////    {
        ////        lock (completeLock)
        ////        {
        ////            completed = true;
        ////            Monitor.Pulse(completeLock);
        ////        }
        ////    };

        ////    target.ArchiveFiles(directives.ToArray());

        ////    lock (completeLock)
        ////    {
        ////        completed = completed ? true : Monitor.Wait(completeLock);
        ////    }

        ////    return results;
        ////}
    }
}
