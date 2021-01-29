namespace SCUMSLang.Tokenizer
{
    public enum TokenType
    {
        /// <summary>
        /// static
        /// </summary>
        StaticKeyword,
        /// <summary>
        /// E.g. "DrinkMilk" or "drink_milk" (without quotes)
        /// </summary>
        Name,
        EqualSign,
        /// <summary>
        /// A 32-bit integer value.
        /// </summary>
        Integer,
        /// <summary>
        /// function
        /// </summary>
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
        String,
        /// <summary>
        /// template
        /// </summary>
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
        OrderedKeyword,
        /// <summary>
        /// and
        /// </summary>
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
        IfKeyword,
        /// <summary>
        /// else
        /// </summary>
        ElseKeyword,
        /// <summary>
        /// while
        /// </summary>
        WhileKeyword,
        /// <summary>
        /// true or false
        /// </summary>
        Boolean,
        /// <summary>
        /// ;
        /// </summary>
        Semicolon,
        /// <summary>
        /// for
        /// </summary>
        ForKeyword
    }
}
