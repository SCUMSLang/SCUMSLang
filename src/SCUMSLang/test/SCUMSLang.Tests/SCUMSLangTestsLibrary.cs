using System.IO;

namespace SCUMSLANG
{
    public static class SCUMSLangTestsLibrary
    {
        public static string AssemblyDirectoryName { get; }
        public static string UMSFilesFolderName { get; }

        static SCUMSLangTestsLibrary()
        {
            var assemblyPath = typeof(SCUMSLangTestsLibrary).Assembly.Location;
            AssemblyDirectoryName = Path.GetDirectoryName(assemblyPath)!;

            UMSFilesFolderName = "UMSFiles";
        }

        public static string GetUMSLFilePath(string fileName) =>
            Path.Combine(AssemblyDirectoryName, UMSFilesFolderName, fileName);
    }
}
