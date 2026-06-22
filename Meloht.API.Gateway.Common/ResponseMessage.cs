using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Common
{
    public class ResponseMessage<T> where T : class
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public string StatusCode { get; set; }
        public T Data { get; set; }

        public static ResponseMessage<T> Error(string msg)
        {
            ResponseMessage<T> response = new ResponseMessage<T>();
            response.Message = msg;
            response.StatusCode = ResponseStatus.FailedCode;
            response.Status = ResponseStatus.Failed;
            return response;
        }

        public static ResponseMessage<T> Success(string msg)
        {
            return Success(msg,null);
        }

        public static ResponseMessage<T> Success(string msg,T data)
        {
            ResponseMessage<T> response = new ResponseMessage<T>();
            response.Message = msg;
            response.Data = data;
            response.StatusCode = ResponseStatus.SuccessCode;
            response.Status = ResponseStatus.Success;
            return response;
        }
    }


}
