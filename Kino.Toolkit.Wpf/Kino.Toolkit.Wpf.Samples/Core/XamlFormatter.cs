﻿/*************************************************************************************

   Toolkit for WPF

   Copyright (C) 2007-2016 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at https://xceed.com/xceed-toolkit-plus-for-wpf/

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.LiveExplorer.Core.CodeFormatting;

namespace Xceed.Wpf.Toolkit.LiveExplorer.Core
{
    /// <summary>
    /// Formats the RichTextBox text as colored Xaml
    /// </summary>
    public class XamlFormatter : ITextFormatter
    {
        public static readonly XamlFormatter Instance = new XamlFormatter();

        public string GetText(FlowDocument document)
        {
            return new TextRange(document.ContentStart, document.ContentEnd).Text;
        }

        public void SetText(FlowDocument document, string text)
        {
            document.Blocks.Clear();
            //document.PageWidth = 100;
            ColorizeXAML(text, document);
        }

        #region SyntaxColoring

        #region ColorizeXAML

        public static FlowDocument ColorizeXAML(string xamlText, FlowDocument targetDoc)
        {
            XmlTokenizer tokenizer = new XmlTokenizer();
            XmlTokenizerMode mode = XmlTokenizerMode.OutsideElement;

            List<XmlToken> tokens = tokenizer.Tokenize(xamlText, ref mode);
            List<string> tokenTexts = new List<string>(tokens.Count);
            List<Color> colors = new List<Color>(tokens.Count);
            int position = 0;
            foreach (XmlToken token in tokens)
            {
                string tokenText = xamlText.Substring(position, token.Length);
                tokenTexts.Add(tokenText);
                Color color = ColorForToken(token, tokenText);
                colors.Add(color);
                position += token.Length;
            }

            Paragraph p = new Paragraph();

            // Loop through tokens
            for (int i = 0; i < tokenTexts.Count; i++)
            {
                Run r = new Run(tokenTexts[i])
                {
                    Foreground = new SolidColorBrush(colors[i])
                };
                p.Inlines.Add(r);
            }

            targetDoc.Blocks.Add(p);

            return targetDoc;
        }

        #endregion //ColorizeXAML

        private static Color ColorForToken(XmlToken token, string tokenText)
        {
            Color color = Color.FromRgb(0, 0, 0);
            switch (token.Kind)
            {
                case XmlTokenKind.Open:
                case XmlTokenKind.OpenClose:
                case XmlTokenKind.Close:
                case XmlTokenKind.SelfClose:
                case XmlTokenKind.CommentBegin:
                case XmlTokenKind.CommentEnd:
                case XmlTokenKind.CDataBegin:
                case XmlTokenKind.CDataEnd:
                case XmlTokenKind.Equals:
                case XmlTokenKind.OpenProcessingInstruction:
                case XmlTokenKind.CloseProcessingInstruction:
                case XmlTokenKind.AttributeValue:
                    color = Color.FromRgb(3, 47, 98);
                    // color = "blue";
                    break;
                case XmlTokenKind.ElementName:
                    color = Color.FromRgb(34, 134, 58);
                    // color = "brown";
                    break;
                case XmlTokenKind.TextContent:
                    // color = "black";
                    break;
                case XmlTokenKind.AttributeName:
                case XmlTokenKind.Entity:
                    color = Color.FromRgb(111, 66, 193);
                    // color = "red";
                    break;
                case XmlTokenKind.CommentText:
                    color = Color.FromRgb(0, 128, 0);
                    // color = "green";
                    break;
            }
            if (token.Kind == XmlTokenKind.ElementWhitespace
                || (token.Kind == XmlTokenKind.TextContent && tokenText.Trim() == ""))
            {
                // color = null;
            }
            return color;
        }
        #endregion SyntaxColoring
    }
}
