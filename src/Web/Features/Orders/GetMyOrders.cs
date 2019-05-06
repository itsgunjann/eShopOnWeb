﻿using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Collections.Generic;

namespace Microsoft.eShopWeb.Web.Features.Orders
{
    public class GetMyOrdersQuery : IRequest<IEnumerable<OrderViewModel>>
    {
        public string UserName { get; set; }

        public GetMyOrdersQuery(string userName)
        {
            UserName = userName;
        }
    }

    public class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, IEnumerable<OrderViewModel>>
    {
        private readonly IOrderRepository _orderRepository;
        
        public GetMyOrdersHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderViewModel>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var specification = new CustomerOrdersWithItemsSpecification(request.UserName);
            var orders = await _orderRepository.ListAsync(specification);

            return orders.Select(o => new OrderViewModel
                {
                    OrderDate = o.OrderDate,
                    OrderItems = o.OrderItems?.Select(oi => new OrderItemViewModel()
                    {
                        Discount = 0,
                        PictureUrl = oi.ItemOrdered.PictureUri,
                        ProductId = oi.ItemOrdered.CatalogItemId,
                        ProductName = oi.ItemOrdered.ProductName,
                        UnitPrice = oi.UnitPrice,
                        Units = oi.Units
                    }).ToList(),
                    OrderNumber = o.Id,
                    ShippingAddress = o.ShipToAddress,
                    Status = "Pending",
                    Total = o.Total()
                });
        }
    }
}