
namespace Astro.CQRS
{
    using System;

    public static class ExceptionEx
    {
        /// <summary>
        /// Returns a string detailing exception message along with any inner exceptions
        /// </summary>
        public static string BuildExceptionInfo(this Exception ex)
        {
            return string.Format(
                "{0}\r\nStack Trace:\r\n{1}",
                ex.GetTypeAndMessage().AppendInnerExceptionMessage(ex),
                ex.StackTrace);
        }

        private static string AppendInnerExceptionMessage(this string s, Exception ex)
        {
            if (ex.InnerException == null)
                return s;

            return string.Format("{0}\r\n-{1}", s, ex.InnerException.GetTypeAndMessage())
                         .AppendInnerExceptionMessage(ex.InnerException);
        }

        private static string GetTypeAndMessage(this Exception ex)
        {
            return string.Format("{0}: {1}", ex.GetType().Name, ex.Message);
        }
    }
}
