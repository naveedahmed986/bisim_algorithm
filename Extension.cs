using System;
using System.Text.RegularExpressions;

namespace Bisimulation_Desktop
{
    public static class Extension
    {
        static Regex lineCommentRegex = new Regex(Constant.Pattern.lineCommentPattern);
        static Regex declarationRegex = new Regex(Constant.Pattern.declarationPattern);
        static Regex functionRegex = new Regex(Constant.Pattern.functionPattern);
        static Regex functionArgsRegex = new Regex(Constant.Pattern.functionArgsPattern);
        static Regex rangeItemsRegex = new Regex(Constant.Pattern.arrayRangePattern);

        public static Nta ModifyC_Code(Nta nta)
        {
            nta = ModifyNtaDeclarations(nta);
            for(int i = 0; i < nta.Template.Count; i++)
            {
                nta.Template[i] = ModifyTemplateDeclarations(nta.Template[i]);
            }
            return nta;
        }

        public static Nta ModifyNtaDeclarations(Nta nta)
        {
            string input = nta.Declaration;
            if (string.IsNullOrEmpty(input))
                return nta;
            input = input.Replace('"', ' ');
            input = input.Replace("\n", Environment.NewLine);
            MatchCollection words, rangeItems;
            string statement, modifiedCode = string.Empty, commentLines = string.Empty, arrayRange;
            foreach (string line in input.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!lineCommentRegex.IsMatch(line))
                {
                    statement = line;
                    if (functionRegex.IsMatch(line)) // e.g void queue(int i, int j) or int x(global variable)
                    {
                        words = functionArgsRegex.Matches(statement);
                        foreach (Match word in words)
                        {
                            statement = ReplaceValue(statement, word.Groups[0].Value, word.Groups[0].Value + Constant.Common.VariablePostfix);
                            nta = RenameGlobalDeclarationsInModel(word.Groups[0].Value, nta);
                        }
                    }
                    else if (declarationRegex.IsMatch(line))
                    {
                        words = declarationRegex.Matches(line);
                        foreach (Match word in words)
                        {
                            statement = ReplaceValue(statement, word.Groups[0].Value, word.Groups[0].Value + Constant.Common.VariablePostfix);
                            nta = RenameGlobalDeclarationsInModel(word.Groups[0].Value, nta);
                        }
                        for (int i = 0; i < statement.Length; i++)
                        {
                            if (statement[i] == '[')
                            {
                                //if (statement[i + 2] - statement[i] == 2) // e.g [N] single value
                                //    statement = statement.Substring(0, i + 1) + statement[i + 1] + Constant.Common.VariablePostfix +
                                //    statement.Substring(i + 2, statement.Length - i - 2);
                                //else // e.g [0, N-1] or [N,N+6]
                                //{
                                    arrayRange = statement.Substring(i + 1, (statement.IndexOf(']', i) - i) - 1);
                                    rangeItems = rangeItemsRegex.Matches(arrayRange);
                                    if (rangeItems.Count == 2)
                                    {
                                        if (string.Equals(rangeItems[0].Groups[0].Value, rangeItems[1].Groups[0].Value))
                                        {
                                            arrayRange = ReplaceValue(arrayRange, rangeItems[0].Groups[0].Value, rangeItems[0].Groups[0].Value + Constant.Common.VariablePostfix);
                                            // arrayRange = arrayRange.Replace(rangeItems[0].Groups[0].Value, rangeItems[0].Groups[0].Value + "_");
                                        }
                                        else
                                        {
                                            foreach (Match item in rangeItems)
                                            {
                                                arrayRange = ReplaceValue(arrayRange, item.Groups[0].Value, item.Groups[0].Value + Constant.Common.VariablePostfix);
                                                // arrayRange = arrayRange.Replace(item.Groups[0].Value, item + "_");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (Match item in rangeItems)
                                        {
                                            arrayRange = ReplaceValue(arrayRange, item.Groups[0].Value, item.Groups[0].Value + Constant.Common.VariablePostfix);
                                            // arrayRange = arrayRange.Replace(item.Groups[0].Value, item + "_");
                                        }
                                    }
                                    statement = statement.Substring(0, i + 1) + arrayRange +
                                        statement.Substring(statement.IndexOf(']', i), statement.Length - statement.IndexOf(']', i));
                                //}
                            }
                        }
                    }
                    modifiedCode = modifiedCode + "\n" + statement;
                    // input = input.Remove(input.IndexOf(line), line.Length);
                }
                else
                    commentLines = String.Format("{0}\n{1}", commentLines, line);
            }
            nta.Declaration = string.Concat(commentLines + modifiedCode);
            return nta;
        }

        public static Template ModifyTemplateDeclarations(Template template)
        {
            string input = template.Declaration;
            if (string.IsNullOrEmpty(input))
                return template;
            input = input.Replace('"', ' ');
            input = input.Replace("\n", Environment.NewLine);
            MatchCollection words, rangeItems;
            string statement, modifiedCode = string.Empty, commentLines = string.Empty, arrayRange;
            foreach (string line in input.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!lineCommentRegex.IsMatch(line))
                {
                    statement = line;
                    if (functionRegex.IsMatch(line)) // e.g void queue(int i, int j) or int x(global variable)
                    {
                        words = functionArgsRegex.Matches(statement);
                        foreach (Match word in words)
                        {
                            statement = ReplaceValue(statement, word.Groups[0].Value, word.Groups[0].Value + Constant.Common.VariablePostfix);
                            template = RenameDeclarationsInTemplate(word.Groups[0].Value, template);
                        }
                    }
                    else if (declarationRegex.IsMatch(line))
                    {
                        words = declarationRegex.Matches(line);
                        foreach (Match word in words)
                        {
                            statement = ReplaceValue(statement, word.Groups[0].Value, word.Groups[0].Value + Constant.Common.VariablePostfix);
                            template = RenameDeclarationsInTemplate(word.Groups[0].Value, template);
                        }
                        for (int i = 0; i < statement.Length; i++)
                        {
                            if (statement[i] == '[')
                            {
                                //if (statement[i + 2] - statement[i] == 2) // e.g [N] single value
                                //    statement = statement.Substring(0, i + 1) + statement[i + 1] + Constant.Common.VariablePostfix +
                                //    statement.Substring(i + 2, statement.Length - i - 2);
                                //else // e.g [0, N-1] or [N,N+6]
                                //{
                                    arrayRange = statement.Substring(i + 1, (statement.IndexOf(']', i) - i) - 1);
                                    rangeItems = rangeItemsRegex.Matches(arrayRange);
                                    if (rangeItems.Count == 2)
                                    {
                                        if (string.Equals(rangeItems[0].Groups[0].Value, rangeItems[1].Groups[0].Value))
                                        {
                                            arrayRange = ReplaceValue(arrayRange, rangeItems[0].Groups[0].Value, rangeItems[0].Groups[0].Value + Constant.Common.VariablePostfix);
                                            // arrayRange = arrayRange.Replace(rangeItems[0].Groups[0].Value, rangeItems[0].Groups[0].Value + "_");
                                        }
                                        else
                                        {
                                            foreach (Match item in rangeItems)
                                            {
                                                arrayRange = ReplaceValue(arrayRange, item.Groups[0].Value, item.Groups[0].Value + Constant.Common.VariablePostfix);
                                                // arrayRange = arrayRange.Replace(item.Groups[0].Value, item + "_");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (Match item in rangeItems)
                                        {
                                            arrayRange = ReplaceValue(arrayRange, item.Groups[0].Value, item.Groups[0].Value + Constant.Common.VariablePostfix);
                                            // arrayRange = arrayRange.Replace(item.Groups[0].Value, item + "_");
                                        }
                                    }
                                    statement = statement.Substring(0, i + 1) + arrayRange +
                                        statement.Substring(statement.IndexOf(']', i), statement.Length - statement.IndexOf(']', i));
                                //}
                            }
                        }
                    }
                    modifiedCode = modifiedCode + "\n" + statement;
                    // input = input.Remove(input.IndexOf(line), line.Length);
                }
                else
                    commentLines = String.Format("{0}\n{1}", commentLines, line);
            }
            template.Declaration = string.Concat(commentLines + modifiedCode);
            return template;
        }
        public static Nta RenameGlobalDeclarationsInModel(string word, Nta nta)
        {
            for(int i = 0; i < nta.Template.Count; i++)
            {
                nta.Template[i] = RenameDeclarationsInTemplate(word, nta.Template[i]);
            }
            return nta;
        }

        public static Template RenameDeclarationsInTemplate(string word, Template template)
        {
            if (!string.IsNullOrEmpty(template.Parameter) && template.Parameter.Contains(word)) // Rename in Parameter
                template.Parameter = ReplaceValue(template.Parameter, word, word + Constant.Common.VariablePostfix);

            for (int j = 0; j < template.Location.Count; j++) // Rename Invariant in Locations
            {
                if (template.Location[j].Label != null && template.Location[j].Label.Text.Contains(word))
                {
                    template.Location[j].Label.Text = ReplaceValue(template.Location[j].Label.Text, word, word + Constant.Common.VariablePostfix);
                }
            }

            for (int k = 0; k < template.Transition.Count; k++) // Rename in Transitions
            {
                for (int l = 0; l < template.Transition[k].Label.Count; l++) // Rename Labels in Transitions
                {
                    if (template.Transition[k].Label[l].Text.Contains(word))
                    {
                        template.Transition[k].Label[l].Text = ReplaceValue(template.Transition[k].Label[l].Text, word, word + Constant.Common.VariablePostfix);
                    }
                }
            }
            return template;
        }

        public static string ReplaceValue(string statement, string replaceFrom, string replaceWith) // exact match word replace
        {
            return Regex.Replace(statement, string.Format(@"\b{0}\b", replaceFrom), replaceWith);
        }

        /*
        * Compare names of the templates from model1 and model2
        * and rename duplications in model2 with postfix "_"
        * returns model2
        */
        public static Nta RenameTemplatesInModel(Nta model1, Nta model2)
        {
            for (int x = 0; x < model2.Template.Count; x++) //For each template in model2 compare name to the templates in model1
            {
                string newName = "";
                for (int y = 0; y < model1.Template.Count; y++) // compare name of each template in model1 with model2 and add postfix _ if names matched
                {
                    if (model2.Template[x].Name.Text.Trim() == model1.Template[y].Name.Text.Trim())
                    {
                        newName = model2.Template[x].Name.Text.Trim();
                        newName = newName + Constant.Common.TemplateRenamePostfix;

                        // modify template name in the system properties as well
                        string properties = @model2.System;
                        properties = properties.Replace((model2.Template[x].Name.Text.Trim() + ","), newName + ",");
                        model2.System = @properties;
                        model2.Template[x].Name.Text = newName;
                    }
                }

                for (int z = 0; z < model2.Template.Count; z++) // if name is duplicated within model2 templates then add another postfix
                {
                    if (x != z && model2.Template[x].Name.Text.Trim() == model2.Template[z].Name.Text.Trim())
                    {
                        newName = newName + Constant.Common.TemplateRenamePostfix;
                        model2.Template[z].Name.Text = newName;
                    }
                }
            }
            return model2;
        }

        //Merge model2 into model1, templates, global declarations and system properties and returns model1
        public static Nta MergeModels(Nta model1, Nta model2)
        {
            // 1. Merge templates of model2 in model1
            foreach (Template template in model2.Template)
            {
                model1.Template.Add(template);
            }

            // 2. Merge global declarations

            model1.Declaration = model1.Declaration + "\n\n//Merged model declarations \n" + model2.Declaration;

            // 3. Merge system properties of model2 in model1

            //******* Remove system statement from model2 system so that 
            //rest of the text can be added to model1 system ******
            string systemProperties2 = @model2.System;
            int startPosition = systemProperties2.IndexOf("\nsystem");
            if (startPosition < 0)
                startPosition = systemProperties2.IndexOf("system");
            int endPosition = systemProperties2.IndexOf(";", startPosition);
            string systemStatement = systemProperties2.Substring(startPosition, (endPosition - startPosition) + 1);
            systemProperties2 = systemProperties2.Replace(systemStatement, "");
            //**********************************************************

            //*** Modify model1 system statement and add templates names of model2 
            //and also append rest of the text in system tag of model2 into model1 system tag ***
            string systemProperties1 = @model1.System;
            startPosition = systemProperties1.IndexOf("\nsystem");
            if (startPosition < 0)
                startPosition = systemProperties1.IndexOf("system");
            endPosition = systemProperties1.IndexOf(";", startPosition);
            systemStatement = systemProperties1.Substring(startPosition, (endPosition - startPosition) + 1);
            systemProperties1 = systemProperties1.Replace(systemStatement, "");
            systemProperties1 = systemProperties1 + "\n\n //Model2 properties \n\n";
            systemProperties1 = systemProperties1 + systemProperties2;

            systemStatement = systemStatement.Replace(";", "");
            foreach (Template template in model2.Template)
                systemStatement = systemStatement + " ," + template.Name.Text.Trim();
            systemStatement = systemStatement + ";";

            systemProperties1 = systemProperties1 + "\n" + systemStatement;

            model1.System = systemProperties1;
            return model1;
        }

        public static string GetIdVariableFromDeclations(string declarations)
        {
            string id = string.Empty;
            if(!string.IsNullOrEmpty(declarations))
            {
                var matched = Regex.Match(declarations, Constant.Pattern.channelDeclarationLinePattern);
                if (matched.Success)
                {
                    id = matched.Value.Substring(matched.Value.IndexOf('[')+1, (matched.Value.IndexOf(']')- matched.Value.IndexOf('['))-1);
                }
            }
            return id;
        }
    }
}
