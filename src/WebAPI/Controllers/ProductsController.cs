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
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly IMapper _mapper;

        public ProductsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand request)
        {
            var response = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
        {
            var updateCommand = _mapper.Map<UpdateProductCommand>(request);
            updateCommand.Id = id;

            return await _mediator.Send(updateCommand) != null
                                  ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] DeleteProductCommand request)
        {
            return await _mediator.Send(request) !=null
                                  ? NoContent() : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] GetProductQuery query)
        {
            var response = await _mediator.Send(query);

            return (response != null) ? Ok(response): NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] GetProductListQuery query)
        {
            var response= await _mediator.Send(query);

            return Ok(response);
        }
    }

}
