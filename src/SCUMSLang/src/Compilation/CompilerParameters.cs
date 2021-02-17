using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SCUMSLang.Compilation
{
    public sealed class CompilerParameters
    {
        public const string RelativeUMSLFilesIndexPath = "UMSLFiles/Index.umsh";
        public const string RelativeUMSLFilesUInt32PoolPath = "UMSLFiles/UInt32Pool.umsl";

        internal static Lazy<string> AbsoluteUMSLFilesIndexPath { get; }
        internal static Lazy<string> AbsoluteUMSLFilesUInt32PoolPath { get; }

        private static Lazy<string> AbsoluteAssemblyDirectory { get; }

        static CompilerParameters()
        {
            AbsoluteAssemblyDirectory = new Lazy<string>(() => {
                var assemblyPath = typeof(Program).Assembly.Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;
                return assemblyDirectory;
            });

            AbsoluteUMSLFilesIndexPath = getLazyUMSLFilePath(RelativeUMSLFilesIndexPath);
            AbsoluteUMSLFilesUInt32PoolPath = getLazyUMSLFilePath(RelativeUMSLFilesUInt32PoolPath);
        }

        private static Lazy<string> getLazyUMSLFilePath(string relativeFilePath) =>
            new Lazy<string>(() =>
                Path.Combine(AbsoluteAssemblyDirectory.Value, relativeFilePath));

        public List<string> SystemSources {
            get {
                if (systemSources is null) {
                    Interlocked.CompareExchange(ref systemSources, new List<string> { AbsoluteUMSLFilesIndexPath.Value }, null);
                }

                return systemSources;
            }

            set => systemSources = value;
        }

        public List<string> UserSources { get; set; } = new List<string>();

        public bool NoImplicitUInt32Pool {
            get => noImplicitUInt32Pool;

            set {
                if (value) {
                    implicitUInt32PoolSource = null;
                }

                noImplicitUInt32Pool = value;
            }
        }

        public string? ImplicitUInt32PoolSource {
            get {
                if (!noImplicitUInt32Pool) {
                    Interlocked.CompareExchange(ref implicitUInt32PoolSource, AbsoluteUMSLFilesUInt32PoolPath.Value, null);
                }

                return implicitUInt32PoolSource!;
            }

            set {
                noImplicitUInt32Pool = true;
                implicitUInt32PoolSource = value;
            }
        }

        private List<string>? systemSources;
        private bool noImplicitUInt32Pool;
        private string? implicitUInt32PoolSource;
    }
}
