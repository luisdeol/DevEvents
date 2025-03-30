namespace DevEvents.API.Models
{
    public abstract class Result
    {
        protected Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
    }

    public class ResultData<T> : Result
    {
        public ResultData(T? data, bool isSuccess = true, string message = "")
            : base(isSuccess, message)
        {
            Data = data;
        }

        public T? Data { get; set; }

        public static ResultData<T> Success(T data) => new(data);

        public static ResultData<T> Error(string message) => new(default, false, message);
    }

    public static class ResultExtensions
    {
        public static IResult ToOkResult<T>(this T data)
        {
            return Results.Ok(ResultData<T>.Success(data));
        }

        public static IResult ToCreatedResult<T>(this T data, string path)
        {
            return Results.Created(path, ResultData<T>.Success(data));
        }

        public static IResult ToSingleResult<T>(this T? data)
        {
            return data is not null ?
                Results.Ok(ResultData<T>.Success(data)) :
                Results.NotFound(ResultData<T>.Error("Not found."));
        }
    }
}
