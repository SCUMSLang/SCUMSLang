using SCUMSLang.AST;

namespace SCUMSLang.Tokenization
{
    public enum TokenType
    {
        /// <summary>
        /// static
        /// </summary>
        [TokenKeyword("static")]
        StaticKeyword,
        /// <summary>
        /// E.g. "DrinkMilk" or "drink_milk" (without quotes)
        /// </summary>
        Name,
        /// <summary>
        /// int
        /// </summary>
        [TokenKeyword("int")]
        [InBuiltType(InBuiltType.Integer)]
        IntKeyword,
        /// <summary>
        /// A 32-bit integer value.
        /// </summary>
        [InBuiltType(InBuiltType.Integer)]
        Integer,
        [TokenKeyword("string")]
        [InBuiltType(InBuiltType.String)]
        StringKeyword,
        /// <summary>
        /// string
        /// </summary>
        [InBuiltType(InBuiltType.String)]
        String,
        /// <summary>
        /// bool
        /// </summary>
        [InBuiltType(InBuiltType.Boolean, IsEnumeration = true)]
        [TokenKeyword("bool")]
        BoolKeyword,
        ///// <summary>
        ///// true or false
        ///// </summary>
        //[TokenKeyword("true", "false")]
        //Boolean,
        /// <summary>
        /// =
        /// </summary>
        EqualSign,
        /// <summary>
        /// function
        /// </summary>
        [TokenKeyword("function")]
        FunctionKeyword,
        /// <summary>
        /// (
        /// </summary>
        [SequenceExample("(")]
        OpenBracket,
        /// <summary>
        /// )
        /// </summary>
        [SequenceExample(")")]
        CloseBracket,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// {
        /// </summary>
        [SequenceExample("{")]
        OpenBrace,
        /// <summary>
        /// }
        /// </summary>
        [SequenceExample("}")]
        CloseBrace,
        /// <summary>
        /// when
        /// </summary>
        [TokenKeyword("when")]
        WhenKeyword,
        /// <summary>
        /// [
        /// </summary>
        [SequenceExample("[")]
        OpenSquareBracket,
        /// <summary>
        /// ]
        /// </summary>
        [SequenceExample("]")]
        CloseSquareBracket,
        /// <summary>
        /// template
        /// </summary>
        [TokenKeyword("template")]
        TemplateKeyword,
        /// <summary>
        /// &lt;
        /// </summary>
        [SequenceExample("<")]
        OpenAngleBracket,
        /// <summary>
        /// &gt;
        /// </summary>
        [SequenceExample(">")]
        CloseAngleBracket,
        /// <summary>
        /// ordered
        /// </summary>
        [TokenKeyword("sequence")]
        SequenceKeyword,
        /// <summary>
        /// and
        /// </summary>
        [TokenKeyword("and")]
        AndKeyword,
        /// <summary>
        /// &&
        /// </summary>
        AndLogicalOperator,
        /// <summary>
        /// ||
        /// </summary>
        OrLogicalOperator,
        /// <summary>
        /// &lt;=
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// &gt;=
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// =
        /// </summary>
        EqualOperator,
        /// <summary>
        /// !=
        /// </summary>
        UnequalOperator,
        /// <summary>
        /// if
        /// </summary>
        [TokenKeyword("if")]
        IfKeyword,
        /// <summary>
        /// else
        /// </summary>
        [TokenKeyword("else")]
        ElseKeyword,
        /// <summary>
        /// while
        /// </summary>
        [TokenKeyword("while")]
        WhileKeyword,
        /// <summary>
        /// ;
        /// </summary>
        Semicolon,
        /// <summary>
        /// for
        /// </summary>
        [TokenKeyword("for")]
        ForKeyword,
        /// <summary>
        /// +
        /// </summary>
        AdditionOperator,
        /// <summary>
        /// -
        /// </summary>
        SubtractionOperator,
        /// <summary>
        /// ++
        /// </summary>
        IncrementOperator,
        /// <summary>
        /// --
        /// </summary>
        DecrementOperator,
        /// <summary>
        /// // text
        /// </summary>
        Comment,
        /// <summary>
        /// /// &lt;summary&gt;text&lt;/summary&gt;
        /// </summary>
        XmlComment,
        /// <summary>
        /// import
        /// </summary>
        [TokenKeyword("import")]
        ImportKeyword,
        /// <summary>
        /// import
        /// </summary>
        [TokenKeyword("enum")]
        EnumKeyword,
        /// <summary>
        /// E.g. Player.Player1 or Unit.ProtossProbe
        /// </summary>
        MemberAccess,
        [TokenKeyword("typedef")]
        TypeDefKeyword
    }
}
