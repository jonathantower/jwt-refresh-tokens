using AngularClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularClient.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly MyContext _myContext;

        public CustomersController(MyContext myContext)
        {
            _myContext = myContext ?? throw new ArgumentNullException(nameof(myContext));
        }

        [HttpGet, Authorize(Roles = "Manager")]
        public IEnumerable<string> Get()
        {
            return _myContext.Customers.Select(c => c.CustomerName);
        }
    }
}
