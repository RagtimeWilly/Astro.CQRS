using System;

namespace Astro.CQRS
{
    public static class ExceptionEx
    {
        /// <summary>
        /// Returns a string detailing exception message along with any inner exceptions
        /// </summary>
        public static string BuildExceptionInfo(this Exception ex)
        {
            return $"{ex.GetTypeAndMessage().AppendInnerExceptionMessage(ex)}\r\nStack Trace:\r\n{ex.StackTrace}";
        }

        private static string AppendInnerExceptionMessage(this string s, Exception ex)
        {
            if (ex.InnerException == null)
                return s;

            return $"{s}\r\n-{ex.InnerException.GetTypeAndMessage()}"
                .AppendInnerExceptionMessage(ex.InnerException);
        }

        private static string GetTypeAndMessage(this Exception ex)
        {
            return $"{ex.GetType().Name}: {ex.Message}";
        }
    }
}
