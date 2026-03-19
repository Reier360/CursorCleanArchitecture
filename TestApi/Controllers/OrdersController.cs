using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestApi.Application.Orders;

namespace TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private static readonly ActivitySource ActivitySource = new("TestApi.Controllers.Orders");
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Orders.GetAll");
        var orders = await _orderService.GetAllAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetByUserAsync(userId, cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Orders.GetById");
        activity?.SetTag("order.id", id.ToString());
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Orders.Create");
        activity?.SetTag("order.userId", request.UserId.ToString());
        var created = await _orderService.CreateAsync(request, cancellationToken);
        if (created is null)
        {
            return BadRequest("Invalid order data.");
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OrderDto>> Update(Guid id, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Orders.Update");
        activity?.SetTag("order.id", id.ToString());
        var updated = await _orderService.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return BadRequest("Order not found or invalid data.");
        }

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Orders.Delete");
        activity?.SetTag("order.id", id.ToString());
        var deleted = await _orderService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
