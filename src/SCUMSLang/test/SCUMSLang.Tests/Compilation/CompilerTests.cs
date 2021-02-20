using System.Threading.Tasks;
using SCUMSLang.Compilation;
using Xunit;
using static SCUMSLANG.SCUMSLangTestsLibrary;

namespace SCUMSLANG.Compilation
{
    public class CompilerTests
    {
        [Fact]
        public async Task Should_compile() =>
            (await Compiler.Default.CompileAsync())
                .ThrowOnError();

        [Fact]
        public async Task Should_compile_set_death()
        {
            var result = await Compiler.Default.CompileAsync(parameters => {
                parameters.UserSources.Add(GetUMSLFilePath("SetDeaths.umsl"));
            });

            result.ThrowOnError();
        }
    }
}
