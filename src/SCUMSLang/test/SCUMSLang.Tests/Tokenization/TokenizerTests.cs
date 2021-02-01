using Xunit;

namespace SCUMSLang.Tokenization
{
    public class TokenizerTests
    {
        [Fact]
        public void Should_tokenize_string()
        {
            var content = @"""<hello>""";
            var tokens = Tokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.String
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_static_variable_declaration_and_assignment()
        {
            var content = @"static int goofy = 7;";
            var tokens = Tokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.StaticKeyword,
                TokenType.IntKeyword,
                TokenType.Name,
                TokenType.EqualSign,
                TokenType.Integer,
                TokenType.Semicolon
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_function_with_empty_body()
        {
            var content = @"function goofy() {
    
}";

            var tokens = Tokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.FunctionKeyword,
                TokenType.Name,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_function_with_three_generic_types_and_with_empty_body()
        {
            var content = @"function donald<Tick, Trick, Track>() {
    
}";

            var tokens = Tokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.FunctionKeyword,
                TokenType.Name,
                TokenType.OpenAngleBracket,
                TokenType.Name,
                TokenType.Comma,
                TokenType.Name,
                TokenType.Comma,
                TokenType.Name,
                TokenType.CloseAngleBracket,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_anonymous_event_handler_with_empty_condition_with_empty_body()
        {
            var content = @"function () when first_condition() {

}";

            var tokens = Tokenizer.Tokenize(content);

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
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_anonymous_event_handler_with_condition_with_arguments_with_empty_body()
        {
            var content = @"function () when first_condition(2, name, ""<haha>"") {

}";

            var tokens = Tokenizer.Tokenize(content);

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
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_template_block_with_two_variables()
        {
            var content = @"template (first_var, second_var) for (2) and (""micky"") {

}";

            var tokens = Tokenizer.Tokenize(content);

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
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_sequence_block()
        {
            var content = @"sequence {

}";

            var tokens = Tokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {

                TokenType.SequenceKeyword,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_attribute()
        {
            var content = @"[attribute_name]";

            var tokens = Tokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.OpenSquareBracket,
                TokenType.Name,
                TokenType.CloseSquareBracket
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_attribute_with_brackets()
        {
            var content = @"[attribute_name()]";

            var tokens = Tokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {

                TokenType.OpenSquareBracket,
                TokenType.Name,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.CloseSquareBracket
            }, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_attribute_with_hexadecimal_argument()
        {
            var content = @"[attribute_name(0x8)]";

            var tokens = Tokenizer.Tokenize(content);

            Assert.Equal(tokens, new Token[] {
                TokenType.OpenSquareBracket,
                TokenType.Name,
                TokenType.OpenBracket,
                TokenType.Integer,
                TokenType.CloseBracket,
                TokenType.CloseSquareBracket
            }, TokenOnlyTypeComparer.Default);
        }
    }
}
