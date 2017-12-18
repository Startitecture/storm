namespace SAF.LocalMachine.Tests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.IO;

    /// <summary>
    ///This is a test class for FileRetentionPolicyManagerTest and is intended
    ///to contain all FileRetentionPolicyManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class FileRetentionPolicyManagerTest
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
            if (!Directory.Exists(Properties.Settings.Default.RetentionTargetDirectory))
            {
                Directory.CreateDirectory(Properties.Settings.Default.RetentionTargetDirectory);
            }

            foreach (string file in Directory.GetFiles(Properties.Settings.Default.RetentionTargetDirectory))
            {
                File.Delete(file);
            }

            foreach (string file in Directory.GetFiles(Properties.Settings.Default.RetentionSourceDirectory))
            {
                File.Copy(
                    file,
                    Path.Combine(Properties.Settings.Default.RetentionTargetDirectory, Path.GetFileName(file)));
            }
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (Directory.Exists(Properties.Settings.Default.RetentionTargetDirectory))
            {
                foreach (string file in Directory.GetFiles(Properties.Settings.Default.RetentionTargetDirectory))
                {
                    File.Delete(file);
                }
            }
        }
        
        #endregion


        /////// <summary>
        ///////A test for ApplyRetention
        ///////</summary>
        ////[TestMethod()]
        ////public void ApplyRetention_TwoDaysToTheHour_FilesOlderThanCutoffDeleted()
        ////{
        ////    IEnumerable<FileRetentionPolicyResult> taskResults = ApplyRetention();

        ////    foreach (FileRetentionPolicyResult taskResult in taskResults)
        ////    {
        ////        Assert.IsNotNull(taskResult.Result);
        ////        Assert.IsDefaultValue(taskResult.ItemError, taskResult.ItemError == null ? String.Empty : taskResult.ItemError.Message);
        ////        Assert.IsFalse(taskResult.Result == PolicyResult.PolicyFailure);
        ////        DateTime cutoffTime = 
        ////            RetentionPolicy.GetRetentionCutoff(
        ////                taskResult.EffectiveDate.LocalDateTime,
        ////                taskResult.Directive.Policy.Length,
        ////                taskResult.Directive.Policy.Granularity);

        ////        switch (taskResult.Result)
        ////        {
        ////            case PolicyResult.PolicyDoesNotApply:
                        
        ////                Assert.IsTrue(
        ////                    taskResult.Directive.Target.LastWriteTime > cutoffTime,
        ////                    "Last write time '{0}' for [N/A] '{1}' is older than {2}",
        ////                    taskResult.Directive.Target.LastWriteTime,
        ////                    taskResult.Directive.Target, 
        ////                    cutoffTime);
        ////                break;

        ////            case PolicyResult.PolicySuccess:

        ////                Assert.IsTrue(
        ////                    taskResult.Directive.Target.LastWriteTime < cutoffTime,
        ////                    "Last write time '{0}' for [SUCCESS] '{1}' is more recent than {2}",
        ////                    taskResult.Directive.Target.LastWriteTime,
        ////                    taskResult.Directive.Target,
        ////                    cutoffTime);

        ////                break;
        ////        }
        ////    }
        ////}

        ////private static IEnumerable<FileRetentionPolicyResult> ApplyRetention()
        ////{
        ////    DirectoryInfo targetDir = new DirectoryInfo(Properties.Settings.Default.RetentionTargetDirectory);
        ////    FileInfo[] files = targetDir.GetFiles();
        ////    int filesToCheck = files.Length;
        ////    List<FileRetentionPolicyResult> taskResults = new List<FileRetentionPolicyResult>();

        ////    object completeLock = new object();
        ////    bool completed = false;

        ////    Assert.IsTrue(files.Length > 0, "The directory '{0}' is empty.", targetDir);

        ////    FileRetentionPolicyManager target = new FileRetentionPolicyManager();

        ////    target.ItemsProduced += delegate(object o, ItemsProducedEventArgs e)
        ////    {
        ////        while (target.TaskResultConsumer.ConsumeNext())
        ////        {
        ////            taskResults.Add(target.TaskResultConsumer.Current);
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

        ////    IRetentionPolicy<DirectoryInfo> policy =
        ////        new RetentionPolicy<DirectoryInfo>(targetDir, new TimeSpan(2, 0, 0, 0), Data.TimeUnit.Hours);

        ////    DateTime oldest = files.Min(x => x.LastWriteTime);

        ////    if (oldest > DateTime.Now - policy.Length)
        ////    {
        ////        Assert.Inconclusive(
        ////            "No files in directory '{0}' would be affected by the retention policy.", targetDir);
        ////    }

        ////    target.ApplyRetention(policy);

        ////    lock (completeLock)
        ////    {
        ////        completed = completed ? true : Monitor.Wait(completeLock);
        ////    }

        ////    return taskResults;
        ////}
    }
}
