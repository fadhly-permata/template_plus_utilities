using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers
{
    /// <summary>
    /// Controller for testing and demonstration purposes
    /// </summary>
    /// <remarks>
    /// Provides simple API endpoints to demonstrate basic functionality
    ///
    /// Example usage:
    /// GET /api/Tester/Add - Performs basic arithmetic operations
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class TesterController : ControllerBase
    {
        /// <summary>
        /// Performs simple addition of two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Sum of the two numbers</returns>
        /// <remarks>
        /// Sample request:
        /// GET /api/Tester/Add?a=5&amp;b=3
        ///
        /// Sample response:
        /// {
        ///     "status": "Success",
        ///     "message": "API processing is done.",
        ///     "data": 8
        /// }
        /// </remarks>
        [HttpGet("Add")]
        public APIResponseData<int> Add([FromQuery] int a, [FromQuery] int b) =>
            new APIResponseData<int>().ChangeData(a + b);
    }
}
