using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Dietphone.Tools
{
    public class PostSender
    {
        public Dictionary<string, string> Inputs { get; set; }
        public event UploadStringCompletedEventHandler Completed;
        private readonly string targetUrl;

        public PostSender(string targetUrl)
        {
            Inputs = new Dictionary<string, string>();
            this.targetUrl = targetUrl;
        }

        public void SendAsync()
        {
            var web = new WebClient();
            web.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            web.UploadStringCompleted += OnCompleted;
            var uri = new Uri(targetUrl);
            var inputs = InputsAsString();
            web.UploadStringAsync(uri, inputs);
        }

        private string InputsAsString()
        {
            var result = new StringBuilder();
            foreach (var input in Inputs)
            {
                if (result.Length != 0)
                {
                    result.Append('&');
                }
                result.Append(input.Key);
                result.Append('=');
                var doubleEncoded = HttpUtility.UrlEncode(HttpUtility.UrlEncode(input.Value));
                result.Append(doubleEncoded);
            }
            return result.ToString();
        }

        protected void OnCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (Completed != null)
            {
                Completed(sender, e);
            }
        }
    }
}
