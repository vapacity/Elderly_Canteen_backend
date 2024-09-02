using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IEmployeeManagement
    {
        /// <summary>
        /// 异步获取所有员工信息
        /// </summary>
        /// <returns>返回一个包含所有员工信息的集合</returns>
        Task<IEnumerable<EmployeeInfoResponseDto>> GetAllEmployeesAsync();
        /// <summary>
        /// 异步根据员工ID获取员工信息
        /// </summary>
        /// <param name="id">员工ID</param>
        /// <returns>返回一个包含指定员工信息的响应对象</returns>
        Task<EmployeeInfoResponseDto> GetEmployeeByIdAsync(string id);
        /// <summary>
        /// 异步添加一个新员工
        /// </summary>
        /// <param name="employeeDto">员工信息传输对象</param>
        Task AddEmployeeAsync(EmployeeInfoRequestDto employeeDto);
        /// <summary>
        /// 异步更新现有员工的信息
        /// </summary>
        /// <param name="id">员工ID</param>
        /// <param name="employeeDto">更新后的员工信息传输对象</param>
        Task UpdateEmployeeAsync(string id, EmployeeInfoRequestDto employeeDto);
        /// <summary>
        /// 异步删除指定ID的员工
        /// </summary>
        /// <param name="id">员工ID</param>
        Task DeleteEmployeeAsync(string id);
        /// <summary>
        /// 发工资
        /// </summary>
        /// <param name="administratorId"></param>
        /// <param name="employeeIds"></param>
        /// <returns></returns>
        Task PaySalayByIdAsync(string administratorId, List<string> employeeIds);
    }

}
