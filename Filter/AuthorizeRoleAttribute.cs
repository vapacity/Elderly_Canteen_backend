using Elderly_Canteen.Data.Dtos.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace Elderly_Canteen.Filter
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 获取当前用户的角色信息
            var identity = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            // 检查用户的角色是否在允许的角色列表中
            if (identity == null || !_roles.Contains(identity))
            {
                // 如果不在允许的角色列表中，返回一个 BadRequest 响应
                context.Result = new BadRequestObjectResult(new RestockResponseDto
                {
                    Success = false,
                    Message = "你没有权限执行此操作",
                    Data = null
                });
            }
        }
    }
}
