namespace SharedLib.Enums
{
    public enum Tables
    {
        User,
        Comment,
        Video,
        ServerLog
    }
    public enum LogType
    {
        загрузка,
        просмотр
    }

    public enum ErrorType
    {
        NotFound,       // 404
        ValidationError,// 400  
        Unauthorized,   // 401
        Forbidden,      // 403
        Conflict,       // 409
        ServerError     // 500
    }
}
