using System.Collections.Generic;
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

            AssertTools.Equal(new Token[] { TokenType.String }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_static_variable_declaration_and_assignment()
        {
            var content = @"static UInt32 goofy = 7;";
            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.StaticKeyword,
                TokenType.Name,
                TokenType.Name,
                TokenType.EqualSign,
                TokenType.Number,
                TokenType.Semicolon
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_function_with_empty_body()
        {
            var content = @"function goofy() {
    
}";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.FunctionKeyword,
                TokenType.Name,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_function_with_three_generic_types_and_with_empty_body()
        {
            var content = @"function donald<Tick, Trick, Track>() {
    
}";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
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
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_anonymous_event_handler_with_empty_condition_with_empty_body()
        {
            var content = @"function () when first_condition() {

}";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.FunctionKeyword,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.WhenKeyword,
                TokenType.Name, // first condition
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_anonymous_event_handler_with_condition_with_arguments_with_empty_body()
        {
            var content = @"function () when first_condition(2, name, ""<haha>"") {

}";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.FunctionKeyword,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.WhenKeyword,
                TokenType.Name, // first condition
                TokenType.OpenBracket,
                TokenType.Number,
                TokenType.Comma,
                TokenType.Name,
                TokenType.Comma,
                TokenType.String,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_template_block_with_two_variables()
        {
            var content = @"template (first_var, second_var) for (2) and (""micky"") {

}";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.TemplateKeyword,
                TokenType.OpenBracket,
                TokenType.Name,
                TokenType.Comma,
                TokenType.Name,
                TokenType.CloseBracket,
                TokenType.ForKeyword,
                TokenType.OpenBracket,
                TokenType.Number,
                TokenType.CloseBracket,
                TokenType.AndKeyword,
                TokenType.OpenBracket,
                TokenType.String,
                TokenType.CloseBracket,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_sequence_block()
        {
            var content = @"sequence {

}";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {

                TokenType.SequenceKeyword,
                TokenType.OpenBrace,
                TokenType.CloseBrace
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_attribute()
        {
            var content = @"[attribute_name]";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.OpenSquareBracket,
                TokenType.Name,
                TokenType.CloseSquareBracket
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_attribute_with_brackets()
        {
            var content = @"[attribute_name()]";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.OpenSquareBracket,
                TokenType.Name,
                TokenType.OpenBracket,
                TokenType.CloseBracket,
                TokenType.CloseSquareBracket
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_attribute_with_hexadecimal_argument()
        {
            var content = @"[attribute_name(0x8)]";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.OpenSquareBracket,
                TokenType.Name,
                TokenType.OpenBracket,
                TokenType.Number,
                TokenType.CloseBracket,
                TokenType.CloseSquareBracket
            }, tokens.Span, TokenOnlyTypeComparer.Default);
        }

        [Fact]
        public void Should_tokenize_comments_and_xml_comments()
        {
            var content = @"name_keyword_like

                            // hello

                            /// :)
                            
                            //hello
                            ///:)
                            //            hello
                            ///           :)
                            
                            ////           
                            name_keyword_like";

            var tokens = Tokenizer.Tokenize(content);

            AssertTools.Equal(new Token[] {
                TokenType.Name,
                TokenType.Comment,
                TokenType.XmlComment,
                TokenType.Comment,
                TokenType.XmlComment,
                TokenType.Comment,
                TokenType.XmlComment,
                TokenType.Comment,
                TokenType.Name
            }, tokens.Span, TokenOnlyTypeComparer.Default);

            Assert.Equal("hello", tokens.Span[1].GetValue<string>());
            Assert.Equal(":)", tokens.Span[2].GetValue<string>());
            Assert.Equal("hello", tokens.Span[3].GetValue<string>());
            Assert.Equal(":)", tokens.Span[4].GetValue<string>());
            Assert.Equal("hello", tokens.Span[5].GetValue<string>());
            Assert.Equal(":)", tokens.Span[6].GetValue<string>());
            Assert.Equal("", tokens.Span[7].GetValue<string>());
        }

        [Fact]
        public void Should_tokenize_comments_without_further_input()
        {
            IEnumerable<(string Value, TokenType Token)> yieldImmediatellyEndingComments()
            {
                yield return (@"//", TokenType.Comment);
                yield return (@"///", TokenType.XmlComment);
            }

            foreach (var (value, token) in yieldImmediatellyEndingComments()) {
                var tokens = Tokenizer.Tokenize(value);
                AssertTools.Equal(new Token[] { token }, tokens.Span, TokenOnlyTypeComparer.Default);
                Assert.Equal("", tokens.Span[0].GetValue<string>());
            }
        }
    }
}
