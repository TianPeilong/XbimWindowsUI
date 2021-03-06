﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;

namespace XbimXplorer.Querying
{
    internal class TextHighliter
    {
        internal abstract class ReportComponent
        {
            internal abstract Block ToBlock();
        }

        internal class MultiPartBit : ReportComponent
        {
            Block _b = null;
            public MultiPartBit(string[] strings, Brush[] brushes)
            {
                int iC = Math.Min(strings.Length, brushes.Length);
                Span s = new Span();
                for (int i = 0; i < iC; i++)
                {
                    Span s2 = new Span(new Run(strings[i]));
                    s2.Foreground = brushes[i];
                    s.Inlines.Add(s2);
                }
                _b = new Paragraph(s);
                
            }
            internal override Block ToBlock()
            {
                return _b;
            }
        }

        internal class ReportBit : ReportComponent
        {
            string _textContent;
            Brush _textBrush;

            public ReportBit(string txt, Brush brsh = null)
            {
                _textContent = txt;
                _textBrush = brsh;
            }

            internal override Block ToBlock()
            {
                Paragraph p = new Paragraph(new Run(_textContent));
                if (_textBrush != null)
                    p.Foreground = _textBrush;
                return p;
            }
        }

        List<ReportComponent> _bits = new List<ReportComponent>();

        internal void Append(string text, Brush color)
        {
            _bits.Add(new ReportBit(text, color));
        }

        public Brush DefaultBrush = null;

        internal void AppendFormat(string format, params object[] args)
        {
            _bits.Add(new ReportBit(
                string.Format(null, format, args),
                DefaultBrush
                ));
        }

        internal void Append(TextHighliter other)
        {
            _bits.AddRange(other._bits);
        }

        internal void Clear()
        {
            _bits = new List<ReportComponent>();
        }

        internal void DropInto(FlowDocument flowDocument)
        {
            flowDocument.Blocks.AddRange(ToBlocks());
        }

        private IEnumerable<Block> ToBlocks()
        {
            foreach (var item in _bits)
            {
                yield return item.ToBlock();
            }
        }

        internal void AppendSpans(string[] strings, Brush[] brushes)
        {
            MultiPartBit b = new MultiPartBit(strings, brushes);
            _bits.Add(b);
            
        }
    }
}
