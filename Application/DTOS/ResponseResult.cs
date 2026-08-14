namespace Application.DTOS
{
    public class ResponseResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        public ResponseResult(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static ResponseResult Success() => new ResponseResult(true, string.Empty);
        public static ResponseResult Failure(string errorMessage) => new ResponseResult(false, errorMessage);
    }
}
