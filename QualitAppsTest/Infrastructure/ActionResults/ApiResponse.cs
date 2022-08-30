using QualitAppsTest.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace QualitAppsTest.Infrastructure.ActionResults
{
    public class ApiResponse
    {
        public ApiResponse()
        {
        }
        public ApiResponse(int responseCode, string responseDesc = null)
        {
            ResponseCode = responseCode;
            ResponseMessage = responseDesc;
        }
        public int ResponseCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseMessage { get; set; }
    }

    public class ApiOkResponse<T> : ApiResponse
    {
        //public T Result { get; set; }
        public ApiOkResponse(T result) : base(200)
        {
            //  Result = result;
        }

        public ApiOkResponse() : base(200)
        {
            // Result = default(T);
        }

        public ApiOkResponse(ApiResponse response, T result) : base((response != null) ? response.ResponseCode : 200, (response != null) ? response.ResponseMessage : null)
        {
            // Result = result;
        }

        public ApiOkResponse((ApiResponse response, T result) responseTuple) : this(response: responseTuple.response, result: responseTuple.result)
        {
            // Result = responseTuple.result;
        }

        public ApiOkResponse(DefaultMetaResult metaResult, T result) : base((metaResult != null) ? metaResult.ResponseCode : 200, (metaResult != null) ? metaResult.ResponseMessage : null)
        {
            //  Result = result;
        }

        public ApiOkResponse((DefaultMetaResult metaResult, T result) responseTuple) : this(metaResult: responseTuple.metaResult, result: responseTuple.result)
        {
            //  Result = responseTuple.result;
        }

        public ApiOkResponse(DefaultMetaResult metaResult) : base((metaResult != null) ? metaResult.ResponseCode : 200, (metaResult != null) ? metaResult.ResponseMessage : null)
        {
            // Result = default(T);
        }
    }


    #region Validation Error Related
    public class ApiValidationErrorResponse : ApiResponse
    {
        public ApiValidationErrorResponse(object result) : base(400)
        {
            Result = result;
        }

        public ApiValidationErrorResponse(ApiResponse response, Object result) : base((response != null) ? response.ResponseCode : 200, (response != null) ? response.ResponseMessage : null)
        {
            Result = result;
        }

        public object Result { get; set; }
    }

    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }

    public class ValidationErrorResult
    {
        public List<ValidationError> Errors { get; set; }

        public ValidationErrorResult(ModelStateDictionary modelState)
        {
            Errors = modelState.Keys.SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage))).ToList();
        }
    }
    #endregion Validation Error Related

    #region Api Error Related
    public class ApiErrorResponse : ApiResponse
    {
        public ApiErrorResponse(object result) : base(500)
        {
            Result = result;
        }

        public ApiErrorResponse(ApiResponse response, Object result) : base((response != null) ? response.ResponseCode : 200, (response != null) ? response.ResponseMessage : null)
        {
            Result = result;
        }

        public object Result { get; set; }
    }

    public class ApiError
    {
        public string Error { get; set; }
        public ApiError(string error)
        {
            Error = error;
        }

    }
    #endregion Validation Error Related
}
