using Xunit;

namespace SCRUML.Tokenizer
{
    public class SCRUMLTokenizerTests
    {
        [Fact]
        public void Should_tokenize_string()
        {
            var content = @"""<hello>""";
            var tokens = SCRUMLTokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.String
            });
        }

        [Fact]
        public void Should_tokenize_static_variable_declaration_and_assignment()
        {
            var content = @"static goofy = 7;";
            var tokens = SCRUMLTokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.StaticKeyword,
                TokenType.Name,
                TokenType.EqualSign,
                TokenType.Integer,
                TokenType.Semicolon
            });
        }

        [Fact]
        public void Should_tokenize_function_with_empty_body()
        {
            var content = @"function goofy() {
    
}";

            var tokens = SCRUMLTokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.FunctionKeyword,
                TokenType.Name,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            });
        }

        [Fact]
        public void Should_tokenize_anonymous_event_handler_with_empty_condition_with_empty_body()
        {
            var content = @"function () when first_condition() {

}";

            var tokens = SCRUMLTokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.FunctionKeyword,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.WhenKeyword,
                TokenType.Name, // first condition
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            });
        }

        [Fact]
        public void Should_tokenize_anonymous_event_handler_with_condition_with_arguments_with_empty_body()
        {
            var content = @"function () when first_condition(2, name, ""<haha>"") {

}";

            var tokens = SCRUMLTokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.FunctionKeyword,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.WhenKeyword,
                TokenType.Name, // first condition
                TokenType.OpenBracket,
                TokenType.Integer,
                TokenType.Comma,
                TokenType.Name,
                TokenType.Comma,
                TokenType.String,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            });
        }

        [Fact]
        public void Should_tokenize_template_block_with_two_variables()
        {
            var content = @"template (first_var, second_var) for (2) and (""micky"") {

}";

            var tokens = SCRUMLTokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.TemplateKeyword,
                TokenType.OpenBracket,
                TokenType.Name,
                TokenType.Comma,
                TokenType.Name,
                TokenType.CloseBracket,
                TokenType.ForKeyword,
                TokenType.OpenBracket,
                TokenType.Integer,
                TokenType.CloseBracket,
                TokenType.AndKeyword,
                TokenType.OpenBracket,
                TokenType.String,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            });
        }
    }
}
