using Aliyun.OSS;
using Elderly_Canteen.Data.Dtos.VolServe;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;

namespace Elderly_Canteen.Services.Implements
{
    public class VolServeService : IVolServeService
    {
        private readonly IGenericRepository<DeliverOrder> _deliverOrderRepository;
        private readonly IGenericRepository<OrderInf> _orderInfRepository;
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IGenericRepository<CartItem> _cartItemRepository;
        private readonly IGenericRepository<Dish> _dishRepository;
        private readonly IGenericRepository<Finance> _financeRepository;
        private readonly IGenericRepository<Weekmenu> _weekMenuRepository;
        private readonly IGenericRepository<DeliverV> _deliverVRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Volunteer> _volunteerRepository;


        public VolServeService(IGenericRepository<DeliverOrder> deliverOrderRepository,
                                IGenericRepository<OrderInf> orderInfRepository,
                                IGenericRepository<Cart> cartRepository,
                                IGenericRepository<Dish> dishRepository,
                                IGenericRepository<CartItem> cartItemRepository,
                                IGenericRepository<Finance> financeRepository,
                                IGenericRepository<Weekmenu> weekMenuRepository,
                                IGenericRepository<DeliverV> deliverVRepository,
                                IGenericRepository<Account> accountRepository,
                                IGenericRepository<Volunteer> volunteerRepository)
        {
            _deliverOrderRepository = deliverOrderRepository;
            _orderInfRepository = orderInfRepository;
            _cartRepository = cartRepository;
            _dishRepository = dishRepository;
            _cartItemRepository = cartItemRepository;
            _financeRepository = financeRepository;
            _weekMenuRepository = weekMenuRepository;
            _deliverVRepository = deliverVRepository;
            _volunteerRepository = volunteerRepository;
            _accountRepository = accountRepository;
        }

        public async Task<AccessOrderResponseDto> GetAccessOrder()
        {
            // 1. 查找所有 deliverStatus 为 "待接单" 的 DeliverOrder 记录
            var deliverOrders = await _deliverOrderRepository.FindByConditionAsync(d => d.DeliverStatus == "未接单");

            // 2. 如果没有找到符合条件的订单，返回错误消息
            if (!deliverOrders.Any())
            {
                return new AccessOrderResponseDto
                {
                    Success = false,
                    Msg = "没有待接单的订单",
                    Response = new List<Response>()
                };
            }

            // 3. 创建一个 List 来存储所有的 Response 项
            var responseList = new List<Response>();

            // 4. 遍历所有找到的 DeliverOrder 记录
            foreach (var deliverOrder in deliverOrders)
            {
                // 通过 OrderId 获取订单信息
                var orderInfo = await _orderInfRepository.FindByConditionAsync(o => o.OrderId == deliverOrder.OrderId);
                if (orderInfo == null || !orderInfo.Any())
                {
                    continue; // 跳过没有找到的订单
                }

                var order = orderInfo.FirstOrDefault();

                // 通过 CartId 获取购物车信息
                var cart = await _cartRepository.GetByIdAsync(order.CartId);
                if (cart == null)
                {
                    continue; // 跳过没有购物车信息的订单
                }

                // 获取购物车中的所有项目
                var cartItems = await _cartItemRepository.FindByConditionAsync(ci => ci.CartId == cart.CartId);
                if (cartItems == null || !cartItems.Any())
                {
                    continue; // 跳过没有购物车项目的订单
                }

                // 5. 组装订单菜品信息
                var orderDishes = new List<OrderDish>();
                foreach (var cartItem in cartItems)
                {
                    var dish = await _dishRepository.GetByIdAsync(cartItem.DishId);  // 获取菜品信息
                    var disPrice = (await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(cartItem.DishId, cartItem.Week)).DisPrice;
                    if (dish != null)
                    {
                        orderDishes.Add(new OrderDish
                        {
                            DishName = dish.DishName,
                            Picture = dish.ImageUrl,
                            Price = disPrice == 0 ? dish.Price : disPrice,
                            Quantity = cartItem.Quantity
                        });
                    }
                }
                var finance = await _financeRepository.GetByIdAsync(order.FinanceId);
                // 6. 构建 Response
                var response = new Response
                {
                    OrderId = order.OrderId,
                    CusAddress = deliverOrder.CusAddress,  // 获取客户地址
                    DeliverOrDining = order.DeliverOrDining == "D",  // 处理外送或堂食
                    DeliverStatus = deliverOrder.DeliverStatus,
                    Money = finance.Price,  // 假设总金额为总价加补贴
                    OrderDishes = orderDishes,
                    Remark = order.Remark ?? "无备注",
                    Status = order.Status,
                    Subsidy = order.Bonus,  // 补贴
                    UpdatedTime = finance.FinanceDate
                };

                // 7. 将 Response 添加到 responseList 中
                responseList.Add(response);
            }

            // 8. 返回 AccessOrderResponseDto，包含所有待接单的订单信息
            return new AccessOrderResponseDto
            {
                Success = true,
                Msg = "成功获取待接单订单",
                Response = responseList
            };
        }
        public async Task<NormalResponseDto> AcceptOrderAsync(string orderId, string accountId)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(accountId);
            if (volunteer == null)
            {
                return new NormalResponseDto
                {
                    success = false,
                    msg = "不是志愿者"
                };
            }
            if(volunteer.Available != "1")
            {
                return new NormalResponseDto
                {
                    success = false,
                    msg = "已接单，请完成本单后在接单"
                };
            }
            var deliverOrder = await _deliverOrderRepository.GetByIdAsync(orderId);

            var deliverAccount = await _accountRepository.GetByIdAsync(accountId);

            var newDV = new DeliverV
            {
                OrderId = orderId,
                VolunteerId = accountId
            };

            if (deliverOrder.DeliverStatus != "未接单")
            {
                return new NormalResponseDto
                {
                    success = false,
                    msg = "尚未接单"
                };
            }

            deliverOrder.DeliverPhone = deliverAccount.Phonenum;
            deliverOrder.DeliverStatus = "已接单";
            volunteer.Available = "0";

            await _deliverVRepository.AddAsync(newDV);
            await _volunteerRepository.UpdateAsync(volunteer);
            return new NormalResponseDto
            {
                success = true,
                msg = "已接单"
            };

        }
        public async Task<NormalResponseDto> ConfirmDeliveredAsync(string orderId, string accountId)
        {
            var deliverOrder = await _deliverOrderRepository.GetByIdAsync(orderId);
            var deliverE = await _deliverVRepository.GetByIdAsync(orderId);
            if (deliverOrder.DeliverStatus != "已接单")
            {
                return new NormalResponseDto
                {
                    success = true,
                    msg = "尚未接单"
                };
            }
            deliverOrder.DeliverStatus = "已送达";
            await _deliverVRepository.UpdateAsync(deliverE);
            var volunteer = await _volunteerRepository.GetByIdAsync(accountId);
            if(volunteer.Available != "1")
            {
                volunteer.Available = "1";
                await _volunteerRepository.UpdateAsync(volunteer);
            }
            return new NormalResponseDto
            {
                success = true,
                msg = "已送达"
            };
        }


        public async Task<AccessOrderResponseDto> GetAcceptedOrder(string accountId)
        {
            // 1. 查找所有 deliverStatus 为 "待接单" 的 DeliverOrder 记录
            var deliverOrders = await _deliverOrderRepository.FindByConditionAsync(d => d.DeliverStatus != "待接单");

            // 2. 筛选出 deliverId 在 deliverV 表中与 accountId 匹配的订单
            var filteredOrders = new List<DeliverOrder>();
            foreach (var order in deliverOrders)
            {
                var deliverV = await _deliverVRepository.GetByIdAsync(order.OrderId);  // 假设 DeliverV 中通过 OrderId 获取
                if (deliverV != null && deliverV.VolunteerId == accountId)
                {
                    filteredOrders.Add(order);
                }
            }

            // 3. 如果没有找到符合条件的订单，返回错误消息
            if (!filteredOrders.Any())
            {
                return new AccessOrderResponseDto
                {
                    Success = false,
                    Msg = "没有已接单/已送达的订单",
                    Response = new List<Response>()
                };
            }

            // 4. 创建一个 List 来存储所有的 Response 项
            var responseList = new List<Response>();

            // 5. 遍历所有找到的 DeliverOrder 记录
            foreach (var deliverOrder in filteredOrders)
            {
                // 通过 OrderId 获取订单信息
                var orderInfo = await _orderInfRepository.FindByConditionAsync(o => o.OrderId == deliverOrder.OrderId);
                if (orderInfo == null || !orderInfo.Any())
                {
                    continue; // 跳过没有找到的订单
                }

                var order = orderInfo.FirstOrDefault();

                // 通过 CartId 获取购物车信息
                var cart = await _cartRepository.GetByIdAsync(order.CartId);
                if (cart == null)
                {
                    continue; // 跳过没有购物车信息的订单
                }

                // 获取购物车中的所有项目
                var cartItems = await _cartItemRepository.FindByConditionAsync(ci => ci.CartId == cart.CartId);
                if (cartItems == null || !cartItems.Any())
                {
                    continue; // 跳过没有购物车项目的订单
                }

                // 6. 组装订单菜品信息
                var orderDishes = new List<OrderDish>();
                foreach (var cartItem in cartItems)
                {
                    var dish = await _dishRepository.GetByIdAsync(cartItem.DishId);  // 获取菜品信息
                    var disPrice = (await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(cartItem.DishId, cartItem.Week)).DisPrice;
                    if (dish != null)
                    {
                        orderDishes.Add(new OrderDish
                        {
                            DishName = dish.DishName,
                            Picture = dish.ImageUrl,
                            Price = disPrice == 0 ? dish.Price : disPrice,
                            Quantity = cartItem.Quantity
                        });
                    }
                }

                var finance = await _financeRepository.GetByIdAsync(order.FinanceId);

                // 7. 构建 Response
                var response = new Response
                {
                    OrderId = order.OrderId,
                    CusAddress = deliverOrder.CusAddress,  // 获取客户地址
                    DeliverOrDining = order.DeliverOrDining == "D",  // 处理外送或堂食
                    DeliverStatus = deliverOrder.DeliverStatus,
                    Money = finance.Price,  // 假设总金额为总价加补贴
                    OrderDishes = orderDishes,
                    Remark = order.Remark ?? "无备注",
                    Status = order.Status,
                    Subsidy = order.Bonus,  // 补贴
                    UpdatedTime = finance.FinanceDate
                };

                // 8. 将 Response 添加到 responseList 中
                responseList.Add(response);
            }

            // 9. 返回 AccessOrderResponseDto，包含所有待接单的订单信息
            return new AccessOrderResponseDto
            {
                Success = true,
                Msg = "成功获取已送达和已接受订单",
                Response = responseList
            };
        }
        public async Task<AccessOrderResponseDto> GetFinishedOrder(string accountId)
        {
            // 1. 查找所有 deliverStatus 为 "待接单" 的 DeliverOrder 记录
            var deliverOrders = await _deliverOrderRepository.FindByConditionAsync(d => d.DeliverStatus == "已送达");

            // 2. 筛选出 deliverId 在 deliverV 表中与 accountId 匹配的订单
            var filteredOrders = new List<DeliverOrder>();
            foreach (var order in deliverOrders)
            {
                var deliverV = await _deliverVRepository.GetByIdAsync(order.OrderId);  // 假设 DeliverV 中通过 OrderId 获取
                if (deliverV != null && deliverV.VolunteerId == accountId)
                {
                    filteredOrders.Add(order);
                }
            }

            // 3. 如果没有找到符合条件的订单，返回错误消息
            if (!filteredOrders.Any())
            {
                return new AccessOrderResponseDto
                {
                    Success = false,
                    Msg = "没有已送达的订单",
                    Response = new List<Response>()
                };
            }

            // 4. 创建一个 List 来存储所有的 Response 项
            var responseList = new List<Response>();

            // 5. 遍历所有找到的 DeliverOrder 记录
            foreach (var deliverOrder in filteredOrders)
            {
                // 通过 OrderId 获取订单信息
                var orderInfo = await _orderInfRepository.FindByConditionAsync(o => o.OrderId == deliverOrder.OrderId);
                if (orderInfo == null || !orderInfo.Any())
                {
                    continue; // 跳过没有找到的订单
                }

                var order = orderInfo.FirstOrDefault();

                // 通过 CartId 获取购物车信息
                var cart = await _cartRepository.GetByIdAsync(order.CartId);
                if (cart == null)
                {
                    continue; // 跳过没有购物车信息的订单
                }

                // 获取购物车中的所有项目
                var cartItems = await _cartItemRepository.FindByConditionAsync(ci => ci.CartId == cart.CartId);
                if (cartItems == null || !cartItems.Any())
                {
                    continue; // 跳过没有购物车项目的订单
                }

                // 6. 组装订单菜品信息
                var orderDishes = new List<OrderDish>();
                foreach (var cartItem in cartItems)
                {
                    var dish = await _dishRepository.GetByIdAsync(cartItem.DishId);  // 获取菜品信息
                    var disPrice = (await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(cartItem.DishId, cartItem.Week)).DisPrice;
                    if (dish != null)
                    {
                        orderDishes.Add(new OrderDish
                        {
                            DishName = dish.DishName,
                            Picture = dish.ImageUrl,
                            Price = disPrice == 0 ? dish.Price : disPrice,
                            Quantity = cartItem.Quantity
                        });
                    }
                }

                var finance = await _financeRepository.GetByIdAsync(order.FinanceId);

                // 7. 构建 Response
                var response = new Response
                {
                    OrderId = order.OrderId,
                    CusAddress = deliverOrder.CusAddress,  // 获取客户地址
                    DeliverOrDining = order.DeliverOrDining == "D",  // 处理外送或堂食
                    DeliverStatus = deliverOrder.DeliverStatus,
                    Money = finance.Price,  // 假设总金额为总价加补贴
                    OrderDishes = orderDishes,
                    Remark = order.Remark ?? "无备注",
                    Status = order.Status,
                    Subsidy = order.Bonus,  // 补贴
                    UpdatedTime = finance.FinanceDate
                };

                // 8. 将 Response 添加到 responseList 中
                responseList.Add(response);
            }

            // 9. 返回 AccessOrderResponseDto，包含所有待接单的订单信息
            return new AccessOrderResponseDto
            {
                Success = true,
                Msg = "成功获取已送达订单",
                Response = responseList
            };
        }
    }
}
