using Alpha.Customer.API.Models;
using Alpha.Customer.API.Params;
using Alpha.Customer.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Customer.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService customerService;

    public CustomerController(ICustomerService customerService)
        => this.customerService = customerService;

    [HttpGet("status")]
    public Task<CustomerStatus> GetCustomerStatus(int custId, int subAccId)
        => customerService.GetCustomerStatus(new CustomerStatusParam(custId, subAccId));

    [HttpPost("status/multiple")]
    public Task<IEnumerable<CustomerStatus>> GetMultipleCustomerStatus([FromBody] IEnumerable<CustomerStatusParam> query)
        => customerService.GetCustomerStatus(query);

    [HttpGet("{custId}/downline/{downlineUsername}")]
    public Task<DownlineInfo> GetDownlineInfo(int custId, string downlineUsername)
        => customerService.GetDownlineInfo(custId, downlineUsername);
}