using System.Net;
using System.Security.Authentication;
using Newtonsoft.Json;
using Envista.Core.Common.Exceptions;
using SecurityException = Envista.Core.Common.Exceptions.SecurityException;

namespace Template.Service.Web
{
    /// <summary>
    /// Defines a middleware component that handles Web API exceptions. 
    /// </summary>
    public class WebApiExceptionHandler
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the WebApiExceptionHandler class. 
        /// </summary>
        /// <param name="next">
        /// The next delegate/middleware in the HTTP pipeline
        /// </param>
        public WebApiExceptionHandler(RequestDelegate next)
        {
            this._next = next;
        }

        /// <summary>
        /// Invokes the next delegate/middleware in the HTTP pipeline
        /// </summary>
        /// <param name="context">
        /// An object that encapsulates the HTTP request information
        /// </param>

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        /// <summary>
        /// Handles any exceptions and maps them to a HTTP status and message response. 
        /// </summary>
        /// <param name="context">
        /// An object that encapsulates the HTTP request information 
        /// </param>
        /// <param name="exception">
        /// An object that represents an exception thrown in the HTTP pipeline 
        /// </param>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var message = "The content could not be displayed because an internal server error has occured.";

            if (exception is AuthenticationException)
            {
                code = HttpStatusCode.Unauthorized;
                message = exception.Message ?? "The calling user could not be authenticated.";
            }
            else if (exception is SecurityException)
            {
                code = HttpStatusCode.Forbidden;
                message = exception.Message ?? "The calling user is not authorized to perform the requested action.";
            }
            else if (exception is ArgumentNullException)
            {
                code = HttpStatusCode.BadRequest;
                message = exception.Message ?? "The client request is missing required content.";
            }
            else if (exception is ArgumentException)
            {
                code = HttpStatusCode.BadRequest;
                message = exception.Message ?? "The client request is invalid.";
            }
            else if (exception is FormatException)
            {
                code = HttpStatusCode.BadRequest;
                message = exception.Message ?? "The client request is invalid.";
            }
            else if (exception is ObjectNotFoundException)
            {
                code = HttpStatusCode.NotFound;
                message = exception.Message ?? "The requested resource could not be found.";
            }
            else if (exception is ObjectAlreadyExistsException)
            {
                code = HttpStatusCode.Conflict;
                message = exception.Message ?? "The requested resource already exists.";
            }
            else if (exception is InvalidOperationException)
            {
                code = HttpStatusCode.ExpectationFailed;
                message = exception.Message ?? "The client request could not be completed successfully";
            }
            else if ((exception is AggregateException) && (exception.InnerException != null))
            {
                if (exception.InnerException is AuthenticationException)
                {
                    code = HttpStatusCode.Unauthorized;
                    message = exception.Message ?? "The calling user could not be authenticated.";
                }
                else if (exception.InnerException is SecurityException)
                {
                    code = HttpStatusCode.Forbidden;
                    message = exception.Message ?? "The calling user is not authorized to perform the requested action.";
                }
                else if (exception.InnerException is ArgumentNullException)
                {
                    code = HttpStatusCode.BadRequest;
                    message = exception.Message ?? "The client request is missing required content.";
                }
                else if (exception.InnerException is ArgumentException)
                {
                    code = HttpStatusCode.BadRequest;
                    message = exception.Message ?? "The client request is invalid.";
                }
                else if (exception.InnerException is FormatException)
                {
                    code = HttpStatusCode.BadRequest;
                    message = exception.Message ?? "The requested resource could not be found.";
                }
                else if (exception.InnerException is ObjectNotFoundException)
                {
                    code = HttpStatusCode.NotFound;
                    message = exception.Message ?? "The requested resource could not be found.";
                }
                else if (exception.InnerException is ObjectAlreadyExistsException)
                {
                    code = HttpStatusCode.Conflict;
                    message = exception.Message ?? "The requested resource already exists.";
                }
                else if (exception.InnerException is InvalidOperationException)
                {
                    code = HttpStatusCode.ExpectationFailed;
                    message = exception.Message ?? "The client request could not be completed successfully";
                }
            }

            var result = JsonConvert.SerializeObject(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}

