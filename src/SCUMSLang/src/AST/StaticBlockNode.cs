using System.Collections.Generic;
using Teronis.Collections.Specialized;

namespace SCUMSLang.AST
{
    public partial class StaticBlockNode : BlockNode
    {
        public override Scope Scope => Scope.Static;
        public override BlockNode StaticBlock => this;

        protected override LinkedBucketList<string, Node> ReservedNames { get; }

        public StaticBlockNode()
        {
            ReservedNames = new LinkedBucketList<string, Node>(EqualityComparer<string>.Default) {
                {
                    InBuiltTypeLibrary.Sequences[DefinitionType.Integer],
                    new TypeDefinitionNode(InBuiltTypeLibrary.Sequences[DefinitionType.Integer], DefinitionType.Integer) {
                        AllowOverwriteOnce = true
                    }
                },
                {
                    InBuiltTypeLibrary.Sequences[DefinitionType.String],
                    new TypeDefinitionNode(InBuiltTypeLibrary.Sequences[DefinitionType.String], DefinitionType.String) {
                        AllowOverwriteOnce = true
                    }
                },
                {
                    InBuiltTypeLibrary.Sequences[DefinitionType.Boolean],
                    new EnumerationDefinitionNode(InBuiltTypeLibrary.Sequences[DefinitionType.Boolean], hasReservedNames: true, new []{ "false", "true" }, DefinitionType.Boolean) {
                        AllowOverwriteOnce = true
                    }
                }
            };
        }

        /// <summary>
        /// Begins a block in <see cref="BlockNode.CurrentBlock"/>.
        /// </summary>
        /// <param name="function"></param>
        public virtual void BeginBlock(FunctionNode function)
        {
            var functionBlock = new FunctionBlockNode(this, function);
            AddNode(function);
            BeginBlock(functionBlock);
        }
    }
}
