using System.Threading.Tasks;
using Xunit;
using static SCUMSLang.SCUMSLangTestsLibrary;

namespace SCUMSLang.Compilation
{
    public class CompilerTests
    {
        [Fact]
        public async Task Should_compile_with_default_parameters() =>
            (await Compiler.Default.CompileAsync())
                .ThrowOnError();

        [Fact]
        public async Task Should_compile_set_death()
        {
            var result = await Compiler.Default.CompileAsync(parameters => {
                parameters.ImplicitUInt32PoolSource = GetUMSLFilePath("SetDeaths.umsl");
            });

            result.ThrowOnError();
        }
    }
}
