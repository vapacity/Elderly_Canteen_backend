﻿using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EmployeeManagement : IEmployeeManagement
{
    private readonly IGenericRepository<Employee> _employeeRepository;
    private readonly IGenericRepository<Finance> _financeRepository;
    private readonly IGenericRepository<PayWage> _payWageRepository;

    public EmployeeManagement(IGenericRepository<Employee> employeeRepository, IGenericRepository<Finance> financeRepository, IGenericRepository<PayWage> payWageRepository)
    {
        _employeeRepository = employeeRepository;
        _financeRepository = financeRepository;
        _payWageRepository = payWageRepository;
    }
    // 辅助函数：根据当前员工最大ID生成新的员工ID
    private async Task<string> GenerateEmployeeIdAsync()
    {
        // 获取数据库中当前员工的最大ID
        var maxEmployeeId = await _employeeRepository.GetAll()
            .OrderByDescending(e => e.EmployeeId)
            .Select(e => e.EmployeeId)
            .FirstOrDefaultAsync();

        // 如果没有员工记录，返回第一个ID
        if (maxEmployeeId == null)
        {
            return "E0001";
        }

        // 提取数字部分，并加1
        int numericPart = int.Parse(maxEmployeeId.Substring(1));
        int newNumericPart = numericPart + 1;

        // 新的员工ID为E + (最大数字 + 1), 且ID是4位数字，不足时左侧填充0
        return $"E{newNumericPart.ToString("D4")}";
    }
    //获取所有员工信息
    public async Task<IEnumerable<EmployeeInfoResponseDto>> GetAllEmployeesAsync()
    {
        //获取所有员工数据
        var employees = await _employeeRepository.GetAllAsync();

        // 验证操作
        if (employees == null || !employees.Any())
        {
            return new List<EmployeeInfoResponseDto>
        {
            new EmployeeInfoResponseDto
            {
                success = false,
                msg = "数据库中无员工信息",
                response = new List<EmployeeResponseData>()
            }
        };
        }

        //转换员工实体为DTO对象，并将所有员工信息放入一个列表中
        var employeeList = employees.Select(emp => new EmployeeResponseData
        {
            EmployeeId = emp.EmployeeId,
            EmployeeName = emp.EmployeeName,
            PhoneNum = emp.PhoneNum,
            Address = emp.Address,
            EmployeePosition = emp.EmployeePosition,
            Salary = emp.Salary,
            IdCard = emp.IdCard,
            IsPaidThisMonth = emp.Ispaidthismonth
        }).ToList();

        //返回一个包含所有员工信息的单个EmployeeInfoResponseDto对象
        return new List<EmployeeInfoResponseDto>
        {
            new EmployeeInfoResponseDto
            {
                success = true,
                msg = "员工检索成功",
                response = employeeList
            }
    };
    }

    //通过ID获取员工信息
    public async Task<EmployeeInfoResponseDto> GetEmployeeByIdAsync(string id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
         
            return new EmployeeInfoResponseDto
            {
                success = false,
                msg = $"员工ID为 {id} 的员工不存在。",
                response = null
            };
        }

        return new EmployeeInfoResponseDto
        {
            success = true,
            msg = "员工查找信息如下",
            response = new List<EmployeeResponseData>
            {
                new EmployeeResponseData
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeName = employee.EmployeeName,
                    PhoneNum = employee.PhoneNum,
                    Address = employee.Address,
                    EmployeePosition = employee.EmployeePosition,
                    Salary = employee.Salary,
                    IdCard = employee.IdCard,
                    IsPaidThisMonth = employee.Ispaidthismonth
                }
            }
        };
    }

    //添加员工
    public async Task AddEmployeeAsync(EmployeeInfoRequestDto employeeDto)
    {
        // 检查是否超过9999个员工
        var employeeCount = await _employeeRepository.GetAll().CountAsync();
        if (employeeCount >= 9999)
        {
            throw new InvalidOperationException("员工数量已达到上限，无法添加更多员工！");
        }
        // 检查是否和已有员工重复 (通过IdCard)
        var existingEmployee = await _employeeRepository.FindByConditionAsync(e => e.IdCard == employeeDto.IdCard);
        if (existingEmployee.Any())
        {
            throw new InvalidOperationException("员工信息已存在，无法重复添加！");
        }
        // 调用生成员工ID的方法
        var newEmployeeId = await GenerateEmployeeIdAsync();

        //将 DTO 转换为实体对象
        var employee = new Employee
        {
            EmployeeId = newEmployeeId,  // 设置生成的员工ID
            EmployeeName = employeeDto.EmployeeName,
            PhoneNum = employeeDto.PhoneNum,
            Address = employeeDto.Address,
            EmployeePosition = employeeDto.EmployeePosition,
            Salary = employeeDto.Salary,
            IdCard = employeeDto.IdCard,
            Ispaidthismonth = false
        };
        //调用仓储模式的AddAsync方法
        await _employeeRepository.AddAsync(employee);
    }
    //更新ID对应的员工信息
    public async Task UpdateEmployeeAsync(string id, EmployeeInfoRequestDto employeeDto)
    {
        // 查找员工信息，根据 EmployeeId 属性
        var employee = await _employeeRepository.FindByConditionAsync(e => e.EmployeeId == id);

        if (employee == null || !employee.Any())
        {
            // 如果员工不存在，抛出一个异常或返回错误信息
            throw new InvalidOperationException($"员工ID为 {id} 的员工不存在。");
        }
        // 获取找到的第一个员工实体（假设 EmployeeId 是唯一的）
        var employeeToModify = employee.First();

        // 更新员工信息
        if (!string.IsNullOrEmpty(employeeDto.EmployeeName))
        {
            employeeToModify.EmployeeName = employeeDto.EmployeeName;
        }
        if (!string.IsNullOrEmpty(employeeDto.PhoneNum))
        {
            employeeToModify.PhoneNum = employeeDto.PhoneNum;
        }
        if (!string.IsNullOrEmpty(employeeDto.Address))
        {
            employeeToModify.Address = employeeDto.Address;
        }
        if (!string.IsNullOrEmpty(employeeDto.EmployeePosition))
        {
            employeeToModify.EmployeePosition = employeeDto.EmployeePosition;
        }
        //这里薪水为0默认是没修改
        if (employeeDto.Salary>0)
        {
            employeeToModify.Salary = employeeDto.Salary;
        }
        if (!string.IsNullOrEmpty(employeeDto.IdCard))
        {
            employeeToModify.IdCard = employeeDto.IdCard;
        }

        // employeeToModify.Ispaidthismonth = employeeDto.IsPaidThisMonth;

        // 保存更改
        await _employeeRepository.UpdateAsync(employeeToModify);
    }

    //删除ID对应的员工
    public async Task DeleteEmployeeAsync(string id)
    {
        // 查找员工信息，根据 EmployeeId 属性
        var employee = await _employeeRepository.FindByConditionAsync(e => e.EmployeeId == id);

        if (employee == null || !employee.Any())
        {
            // 如果员工不存在，抛出一个异常或返回错误信息
            throw new InvalidOperationException($"员工ID为 {id} 的员工不存在。");
        }

        // 获取找到的第一个员工实体（假设 EmployeeId 是唯一的）
        var employeeToDelete = employee.First();

        // 如果存在，执行删除操作
        await _employeeRepository.DeleteAsync(employeeToDelete.EmployeeId);
    }


    [Authorize]
    //发工资
    public async Task PaySalayByIdAsync(string administratorId,List<string> employeeIds)
    {
        if (employeeIds == null || !employeeIds.Any())
        {
            throw new ArgumentException("员工 ID 数组不能为空");
        }

        foreach (var id in employeeIds)
        {
            // 查找员工信息，根据 EmployeeId 属性
            var employee = await _employeeRepository.FindByConditionAsync(e => e.EmployeeId == id);

            if (employee == null || !employee.Any())
            {
                // 如果员工不存在，抛出一个异常或返回错误信息
                throw new InvalidOperationException($"员工ID为 {id} 的员工不存在。");
            }
            // 获取找到的第一个员工实体（假设 EmployeeId 是唯一的）
            var employeeToModify = employee.First();

            // 更新员工信息
            if(employeeToModify.Ispaidthismonth == true)
            {
                throw new InvalidOperationException($"不能重复发放员工ID为 {id} 的员工工资。");
            }           
                
            employeeToModify.Ispaidthismonth = true;

            // 保存更改
            await _employeeRepository.UpdateAsync(employeeToModify);

            // 生成 FinanceId
            var financeId = await GenerateFinanceIdAsync();

            // 插入财务信息
            var finance = new Finance
            {
                FinanceId = financeId,
                FinanceType = "工资",
                FinanceDate = DateTime.Now,
                Price = employeeToModify.Salary,
                InOrOut = "1",
                AccountId = administratorId,
                AdministratorId = null, // 可空
                Proof = null,           // 可空
                Status = "待审核"
            };

            await _financeRepository.AddAsync(finance);

            //插入工资表
            var pay = new PayWage
            {
                FinanceId = financeId,
                EmployeeId = employeeToModify.EmployeeId,
                AdministratorId = administratorId
            };
            await _payWageRepository.AddAsync(pay);
        }

    }
    private async Task<string> GenerateFinanceIdAsync()
    {
        const string prefix = "200";

        var maxFinanceId = await _financeRepository.GetAll()
            .Where(f => f.FinanceId.StartsWith(prefix))
            .OrderByDescending(f => f.FinanceId)
            .Select(f => f.FinanceId)
            .FirstOrDefaultAsync();

        if (maxFinanceId == null)
        {
            return prefix + "00001";
        }

        // 提取数字部分并加1
        var numericalPart = int.Parse(maxFinanceId.Substring(prefix.Length));
        var newFinanceId = numericalPart + 1;

        // 使用前导零格式化新的Finance ID
        return prefix + newFinanceId.ToString("D5");
    }
}

