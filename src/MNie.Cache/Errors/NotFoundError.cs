namespace MNie.Cache.Errors
{
    using System;
    using System.Runtime.CompilerServices;
    using ResultType.Results;

    public class NotFoundError : ErrorWithException
    {
        public NotFoundError(string msg, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0) : base(new Exception(msg), memberName, filePath, line)
        {
        }

        public NotFoundError(string msg, Exception exp, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0) : base(msg, exp, memberName, filePath, line)
        {
        }
    }
}