using System;
using System.Collections.Generic;

namespace ResponseWrapper.Client.Models
{
    public class ResponseType
    {
        public string Description { get; set; }

        public static ResponseType Success = Make("Success");

        public static ResponseType NotFound = Make("NotFound");
        public static ResponseType ExceptionError = Make("ExceptionError");
        public static ResponseType ValidationError = Make("ValidationError");

        public static ResponseType[] All = new ResponseType[]
        {
            Success,
            NotFound,
            ExceptionError,
            ValidationError
        };

        static ResponseType Make(string v)
        {
            return new ResponseType { Description = v };
        }

        public override bool Equals(object obj)
        {
            return obj is ResponseType type &&
                   Description == type.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Description);
        }

        public static bool operator ==(ResponseType left, ResponseType right)
        {
            return EqualityComparer<ResponseType>.Default.Equals(left, right);
        }

        public static bool operator !=(ResponseType left, ResponseType right)
        {
            return !(left == right);
        }
    }
}
