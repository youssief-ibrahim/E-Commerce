using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.CommonResult
{
    public class Error
    {
        public string Code { get; }
        public string Description { get; }
        public ErrorType Type { get; }
        private Error(string code, string description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        #region Static Factory Method
        public static Error Failure(string code = "General.Failure", string description = "A general failure has occured")
        {
            return new Error(code, description, ErrorType.Failure);
        }
        public static Error Validation(string code = "General.Validation", string description = "A Validation failure has occured")
        {
            return new Error(code, description, ErrorType.Validation);
        }
        public static Error NotFound(string code = "General.NotFound", string description = "A NotFound failure has occured")
        {
            return new Error(code, description, ErrorType.NotFound);
        }
        public static Error Unauthorized(string code = "General.Unauthorized", string description = "A Unauthorized failure has occured")
        {
            return new Error(code, description, ErrorType.Unauthorized);
        }
        public static Error Forbidden(string code = "General.forbidden", string description = "A forbidden failure has occured")
        {
            return new Error(code, description, ErrorType.Forbidden);
        }
        public static Error InvalidCrendentials(string code = "General.InvalidCrendentials", string description = "A InvalidCrendentials failure has occured")
        {
            return new Error(code, description, ErrorType.InvalidCrendentials);
        }
        #endregion
    }
}
