// Inspirowane artykułem http://dotnet.dzone.com/articles/input-box-windows-phone-7
using System;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Dietphone.Tools
{
    public class XnaInputBox
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public event EventHandler Ok;
        public event EventHandler Cancel;
        private readonly UserControl sender;

        public XnaInputBox(UserControl sender)
        {
            this.sender = sender;
            Title = "";
            Description = "";
            Text = "";
        }

        public void Show()
        {
            Guide.BeginShowKeyboardInput(PlayerIndex.One, Title, Description, Text,
                                         new AsyncCallback(Callback), null);
        }

        private void Callback(IAsyncResult result)
        {
            Text = Guide.EndShowKeyboardInput(result);
            var dispatcher = sender.Dispatcher;
            if (string.IsNullOrEmpty(Text))
            {
                dispatcher.BeginInvoke(() => { OnCancel(); });
            }
            else
            {
                dispatcher.BeginInvoke(() => { OnOk(); });
            }
        }

        private void OnOk()
        {
            if (Ok != null)
            {
                Ok(this, EventArgs.Empty);
            }
        }

        private void OnCancel()
        {
            if (Cancel != null)
            {
                Cancel(this, EventArgs.Empty);
            }
        }
    }
}