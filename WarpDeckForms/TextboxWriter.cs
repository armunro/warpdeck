using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WarpDeckForms
{
    public class TextboxWriter : TextWriter
    {
        private readonly TextBox _textBox;
        private StringBuilder _content = new();

        public TextboxWriter(TextBox textBox)
        {
            _textBox = textBox;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            base.Write(value);
            _content.Append(value);

            if (value != '\n') return;
            if (_textBox.InvokeRequired)
            {
                try
                {
                    _textBox.Invoke(new MethodInvoker(() => _textBox.Text += _content.ToString()));
                    _textBox.Invoke(new MethodInvoker(() => _textBox.SelectionStart = _textBox.TextLength));
                    _textBox.Invoke(new MethodInvoker(() => _textBox.ScrollToCaret()));
                }
                catch (ObjectDisposedException ex)
                {
                }
            }
            else
            {
                _textBox.Text += _content.ToString();
                _textBox.SelectionStart = _textBox.TextLength;
                _textBox.ScrollToCaret();
            }

            _content.Clear();
        }
    }
}