using System.Text;

namespace TS.Shared.Imports
{
    public class ImportStringBuilder
    {
        private int _lastLineIndex;

        private bool isAppendErrmsg = false;

        private StringBuilder _stringBuilder = new StringBuilder();

        public ImportStringBuilder()
        {

        }

        public ImportStringBuilder AppendNewLine(string lineHeader)
        {
            isAppendErrmsg = false;
            _stringBuilder.AppendLine(lineHeader);

            return this;
        }

        public ImportStringBuilder AppendErrmsg(string errmsg, string splitter = "，")
        {
            _stringBuilder.Append(errmsg).Append(splitter);
            isAppendErrmsg = true;

            return this;
        }

        public ImportStringBuilder EndLine(string splitter = "，", string lineFooter = "。</br>")
        {
            if (isAppendErrmsg)
            {
                _stringBuilder.Remove(_stringBuilder.Length - splitter.Length, splitter.Length);
                _stringBuilder.Append(lineFooter);
            }
            else
            {
                _stringBuilder.Remove(_lastLineIndex, _stringBuilder.Length - _lastLineIndex);
            }

            return this;
        }

        public ImportStringBuilder AppendErrmsgAndEndLine(string errmsg, string splitter = "，", string lineFooter = "。</br>")
        {
            _stringBuilder.Append(errmsg).Append(lineFooter);
            _lastLineIndex = _stringBuilder.Length;

            return this;
        }
    }
}
