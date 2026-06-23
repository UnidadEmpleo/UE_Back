using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Seguridad.Application.Core
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; } = true;
        public string? Error { get; set; }
        public T? Value { get; set; }
        public int Code { get; set; }

        public static Result<T> Success(T value) => new Result<T> {IsSuccess = true, Value = value };
        public static Result<T> Failure(string error, int code) => new()
        { 
            IsSuccess = false, 
            Error = error, 
            Code = code
        };
    }
    
}