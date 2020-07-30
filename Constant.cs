using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    public static class Constant
    {
        public static class Common
        {
            public static string AuxilaryChannelPostfix = "sync";
            public static string InputChannelPrefix = "i_";
            public static string OutputChannelPrefix = "o_";
            public static string VariablePostfix = "_";
            public static string TemplateRenamePostfix = "_";
        }
        public static class TransitionLabelKind
        {
            public static string Synchronization = "synchronisation";
            public static string Comment = "comment";
            public static string Select = "select";
            public static string Guard = "guard";
            public static string Update = "assignment";
        }

        public static class ActionType
        {
            public static char Sender = '!';
            public static char Receiver = '?';
        }

        public static class TemplateType
        {
            public static string ENV = "ENV";
            public static string SUT = "SUT";
            public static string Other = "OTHER";
        }

        public static class LocationLabelKind
        {
            public static string Initial = "initial";
            public static string Committed = "committed";
            public static string Urgent = "urgent";
        }

        public static class Pattern
        {
            public static string lineCommentPattern = @"(^\s*[/?|\*])";
            public static string declarationPattern = @"\b(?!meta|struct|double|int|bool|true|false|const|typedef|hybrid|clock|urgent|broadcast|chan|priority|while|do|for|if|else|switch|case|break|default|return|[^[]*]|[0-9])\w+\b";
            public static string functionPattern = @"^(\s*\w+\s+\w+\s*)\(.*\)";
            public static string functionArgsPattern = @"\b(?!void|int|bool|const|typedef|clock|urgent|broadcast|chan|double)\w+\b";
            public static string arrayRangePattern = @"\b[a-zA-Z](\w+)*\b";
        }
    }
}
