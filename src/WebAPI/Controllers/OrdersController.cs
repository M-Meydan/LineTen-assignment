using Application.Features.Orders.Delete;
using Application.Features.Orders.Get;
using Application.Features.Orders.GetList;
using Application.Features.Orders.Update;
using Application.Features.Orders.Create;
using Application.Features.Orders.Get;
using Application.Features.Products.Create;
using Application.Features.Products.Delete;
using Application.Features.Products.Get;
using Application.Features.Products.GetList;
using Application.Features.Products.Update;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly IMapper _mapper;

        public OrdersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand request)
        {
            var response = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetOrder), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request)
        {
            var updateCommand = _mapper.Map<UpdateOrderCommand>(request);
            updateCommand.Id = id;

            return await _mediator.Send(updateCommand) != null
                                  ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] DeleteOrderCommand request)
        {
            return await _mediator.Send(request) != null
                                  ? NoContent() : NotFound();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] GetOrderQuery query)
        {
            var response = await _mediator.Send(query);
            return response != null ? Ok(response) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] GetOrderListQuery query)
        {
            var customersDto = await _mediator.Send(query);

            return Ok(customersDto);
        }
    }

}
