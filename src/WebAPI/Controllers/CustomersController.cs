using Application.Features.Customers.Commands.CreateCustomer;
using Application.Features.Customers.Commands.DeleteCustomer;
using Application.Features.Customers.Commands.UpdateCustomer;
using Application.Features.Customers.Common;
using Application.Features.Customers.Queries.GetCustomer;
using Application.Features.Customers.Queries.GetStockList;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly IMapper _mapper;

        public CustomersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand request)
        {
            var customerDto = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetCustomer), new { id = customerDto.Id }, customerDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            var updateCommand = _mapper.Map<UpdateCustomerCommand>(request);
            updateCommand.Id = id;

            return await _mediator.Send(updateCommand) != null
                                  ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] DeleteCustomerCommand request)
        {
            return await _mediator.Send(request) !=null
                                  ? NoContent() : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer([FromRoute] GetCustomerQuery query)
        {
            var customerDto = await _mediator.Send(query);

            return (customerDto != null) ? Ok(customerDto): NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] GetCustomerListQuery query)
        {
            var customersDto = await _mediator.Send(query);

            return Ok(customersDto);
        }
    }

}
