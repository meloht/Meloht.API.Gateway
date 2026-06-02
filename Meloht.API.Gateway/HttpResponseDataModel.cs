using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Meloht.API.Gateway
{
    internal class HttpResponseDataModel
    {
        public Guid Guid { get; set; }
        public string ContentType { get; set; }

        public string Json { get; set; }

        public ResultType ResultType { get; set; }

        public Attachment Attachment { get; set; }
    }

    public class Attachment
    {
        public string[] ContentDisposition { get; set; }
        public Guid BlobGuid { get; set; }
    }

    public enum ResultType
    {
        Json = 0,
        Blob = 1,
        Redis = 2,
        File = 3
    }
}
