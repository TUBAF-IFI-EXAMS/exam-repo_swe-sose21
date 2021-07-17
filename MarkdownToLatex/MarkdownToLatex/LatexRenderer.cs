using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.IO;

namespace MarkdownToLatex
{

    /// <summary>a static class containing methods used for rendering the LaTeX document.</summary>
    public static class LatexRenderer
    {
        /// <summary>a List containing the LaTeX document line by line.</summary>
        public static List<string> LatexLines;

        private static byte _inlist;

        /// <summary>a byte indicating whether a list is currently rendered.</summary>
        /// <exception cref="System.FormatException">Thrown when trying to set the value to something other than 0-3.</exception>
        public static byte InList {get => _inlist; private set {
            if(0 <= value && value <= 3){
                _inlist = value;
            } else {
                throw new FormatException("Input has to be between 0 and 3. Check the documentation for details."); 
            }
        }}

        private static byte _inquote;

        /// <summary>a byte indicating whether a quote is currently rendered.</summary>
        /// <exception cref="System.FormatException">Thrown when trying to set the value to something other than 0-3.</exception>
        public static byte InQuote {get => _inquote; private set {
            if(0 <= value && value <= 3){
                _inquote = value;
            } else {
                throw new FormatException("Input has to be between 0 and 3. Check the documentation for details."); 
            }
        }}

        /// <summary>Writes the LaTeX document into a file at the specified <paramref name="path"/>.</summary>
        public static void WriteLatexDocument(string path){
            File.WriteAllLines(path, LatexLines);
        }

        /// <summary>Writes a MathElement in LaTeX.</summary>
        public static void WriteMathElement(string line){
            LatexLines.Add(line); //The calculation stuff is handled in MdToTex class
        }

        /// <summary>Writes a Headline in LaTeX using a Match <paramref name="m"/>.</summary>
        public static void WriteHeadline(Match m){
            if(m == Match.Empty) {return;} //SHOULD never happen, but you never know ;D

            int length = m.Groups[1].Value.Length; //First Group, number of #'s
            string caption = m.Groups[2].Value; //Second Group, the actual text

            switch(length){
                case 0:
                    LatexLines.Add(caption); //Should also not happen
                    break;
                case 1:
                    LatexLines.Add(String.Format(@"\chapter*{{{0}}}", caption));
                    break;
                case 2:
                    LatexLines.Add(String.Format(@"\section*{{{0}}}", caption));
                    break;
                case 3:
                    LatexLines.Add(String.Format(@"\subsection*{{{0}}}", caption));
                    break;
                default:
                    LatexLines.Add(String.Format(@"\subsection*{{{0}}}", caption)); //Treat all further headlines as subsections in LaTeX
                    break;
            }
        }

        /// <summary>Writes a List in LaTeX using a Match <paramref name="m"/></summary>
        public static void WriteList(Match m){
            int depth = m.Groups[1].Value.Length/2+1;
            string content = m.Groups[3].Value;

            if(m == Match.Empty) depth = 0;

            if(depth < InList){
                InList = 0;
            }

            if(InList == 0 && depth == 1){ //if not in any list
                LatexLines.Add(@"\begin{itemize}");
                LatexLines.Add(String.Format(@"\item{{{0}}}", content));
                LatexLines.Add(@"\end{itemize}");
                InList = 1;
            } else if (InList == 1){
                if(depth == 1){
                    appendNewItem(content);
                } else if(depth == 2) {
                    newListAfterItem(content);
                    InList = 2;
                }
            } else if (InList == 2){
                if(depth == 2){
                    appendNewItem(content);
                } else if(depth == 3){
                    newListAfterItem(content);
                    InList = 3;
                }
            } else if (InList == 3){
                if(depth == 3){
                    appendNewItem(content);
                }
            }
        }

        private static void appendNewItem(string content){
            LatexLines.Insert(LatexLines.FindLastIndex(x => x.StartsWith(@"\item"))+1, String.Format(@"\item{{{0}}}", content));
        }

        private static void newListAfterItem(string content){
            List<string> tmp = new List<string>();
            tmp.Add(@"\begin{itemize}");
            tmp.Add(String.Format(@"\item{{{0}}}", content));
            tmp.Add(@"\end{itemize}");
            LatexLines.InsertRange(LatexLines.FindLastIndex(x => x.StartsWith(@"\item"))+1, tmp);
        }

        /// <summary>Writes a Quote in LaTeX using a Match <paramref name="m"/>.</summary>
        public static void WriteQuote(Match m){
            int depth = m.Groups[1].Value.Length;
            string content = m.Groups[2].Value;

            if(m == Match.Empty) depth = 0;

            if(depth < InQuote){
                InQuote = 0;
            }

            if(InQuote == 0 && depth == 1){ //if not in any quote
                LatexLines.Add(@"\begin{quote}");
                LatexLines.Add(content);
                LatexLines.Add(@"\end{quote}");
                InQuote = 1;
            } else if (InQuote == 1){
                if(depth == 1){
                    appendNewQuoteLine(0, content);
                } else if(depth == 2) {
                    newQuoteInQuote(content);
                    InQuote = 2;
                }
            } else if (InQuote == 2){
                if(depth == 2){
                    appendNewQuoteLine(1, content);
                } else if(depth == 3){
                    newQuoteInQuote(content);
                    InQuote = 3;
                }
            } else if (InQuote == 3){
                if(depth == 3){
                    appendNewQuoteLine(2, content);
                }
            }
        }

        private static void appendNewQuoteLine(byte depth, string content){
            LatexLines.Insert(LatexLines.FindLastIndex(x => x.StartsWith(@"\end{quote}"))-depth, content);
        }

        private static void newQuoteInQuote(string content){
            List<string> tmp = new List<string>();
            tmp.Add(@"\begin{quote}");
            tmp.Add(content);
            tmp.Add(@"\end{quote}");
            LatexLines.InsertRange(LatexLines.FindLastIndex(x => x.StartsWith(@"\end{quote}")), tmp);
        }

        /// <summary>Writes a <paramref name="line"/> with cursive text in LaTeX.</summary>
        /// <returns>the line, converted from Markdown into LaTeX.</returns>
        public static string WriteCursive(string line){
            return MarkdownParser.TextRx["cursive"].Replace(line, @"\textit{$2}");
        }

        /// <summary>Writes a <paramref name="line"/> with bold text in LaTeX.</summary>
        /// <returns>the line, converted from Markdown into LaTeX.</returns>
        public static string WriteBold(string line){
            return MarkdownParser.TextRx["bold"].Replace(line, @"\textbf{$2}");
        }

        /// <summary>Writes a normal text <paramref name="line"/> in LaTeX.</summary>
        public static void WriteText(string line){
            LatexLines.Add(line);
        }

        /// <summary>Starts a new line with the content of <paramref name="line"/>.</summary>
        public static void StartNewLine(string line){
            LatexLines.Add(line + @"\\");
        }

        /// <summary>Starts a new paragraph.</summary>
        public static void StartNewParagraph(){
            LatexLines.Add(@"\\\\");
        }

        static LatexRenderer(){
            LatexLines = new List<string>();
        }
    }
}
