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
        [TokenValueType]
        [NodeValueType(NodeValueType.Integer, IsDeclarable = true)]
        IntKeyword,
        /// <summary>
        /// unit
        /// </summary>
        [TokenKeyword("unit")]
        [TokenValueType]
        [NodeValueType(NodeValueType.Unit)]
        UnitKeyword,
        /// <summary>
        /// player
        /// </summary>
        [TokenKeyword("player")]
        [TokenValueType]
        [NodeValueType(NodeValueType.Player)]
        PlayerKeyword,
        /// <summary>
        /// =
        /// </summary>
        EqualSign,
        /// <summary>
        /// A 32-bit integer value.
        /// </summary>
        [NodeValueType(NodeValueType.Integer)]
        Integer,
        /// <summary>
        /// function
        /// </summary>
        [TokenKeyword("function")]
        FunctionKeyword,
        /// <summary>
        /// (
        /// </summary>
        OpenBracket,
        /// <summary>
        /// )
        /// </summary>
        CloseBracket,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// {
        /// </summary>
        OpenBrace,
        /// <summary>
        /// }
        /// </summary>
        CloseBrace,
        /// <summary>
        /// when
        /// </summary>
        [TokenKeyword("when")]
        WhenKeyword,
        /// <summary>
        /// [
        /// </summary>
        OpenSquareBracket,
        /// <summary>
        /// ]
        /// </summary>
        CloseSquareBracket,
        /// <summary>
        /// string
        /// </summary>
        [TokenValueType]
        [NodeValueType(NodeValueType.String)]
        String,
        /// <summary>
        /// template
        /// </summary>
        [TokenKeyword("template")]
        TemplateKeyword,
        /// <summary>
        /// &lt;
        /// </summary>
        OpenAngleBracket,
        /// <summary>
        /// &gt;
        /// </summary>
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
        /// true or false
        /// </summary>
        [TokenKeyword("true", "false")]
        [NodeValueType(NodeValueType.Boolean)]
        Boolean,
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
        Comment,
        XmlComment
    }
}
