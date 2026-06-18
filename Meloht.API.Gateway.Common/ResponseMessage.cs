using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common
{
    public class ResponseMessage
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public string StatusCode { get; set; }

        public static ResponseMessage Error(string msg)
        {
            ResponseMessage response = new ResponseMessage();
            response.Message = msg;
            response.StatusCode = ResponseStatus.FailedCode;
            response.Status = ResponseStatus.Failed;
            return response;
        }

        public static ResponseMessage Success(string msg)
        {
            ResponseMessage response = new ResponseMessage();
            response.Message = msg;
            response.StatusCode = ResponseStatus.SuccessCode;
            response.Status = ResponseStatus.Success;
            return response;
        }

       
    }


}
