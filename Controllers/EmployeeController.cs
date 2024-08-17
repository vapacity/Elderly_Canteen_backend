﻿using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeManagement _employeeManagement;

    public EmployeeController(IEmployeeManagement employeeManagement)
    {
        _employeeManagement = employeeManagement;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<EmployeeInfoResponseDto>>> GetAllEmployees()
    {
        var employees = await _employeeManagement.GetAllEmployeesAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeInfoResponseDto>> GetEmployeeById(string id)
    {
        var employee = await _employeeManagement.GetEmployeeByIdAsync(id);
        if (employee == null || !employee.Success)
        {
            return NotFound(employee);
        }
        return Ok(employee);
    }

    [HttpPost("add")]
    public async Task<ActionResult> AddEmployee(EmployeeInfoRequestDto employeeDto)
    {
        try
        {
            // 调用服务层方法，添加员工
            await _employeeManagement.AddEmployeeAsync(employeeDto);

            // 成功时，返回 success = true 和 msg = "添加成功"
            return Ok(new { success = true, msg = "添加成功" });
        }
        catch (InvalidOperationException ex)
        {
            // 捕获并处理业务逻辑中的 InvalidOperationException 异常
            return BadRequest(new { success = false, msg = ex.Message });
        }
        catch (Exception ex)
        {
            // 捕获其他未预期的异常
            return StatusCode(500, new { success = false, msg = $"内部服务器错误: {ex.Message}" });
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(string id, EmployeeInfoRequestDto employeeDto)
    {
        if (id.ToString() != employeeDto.EmployeeId)
        {
            return BadRequest();
        }
        await _employeeManagement.UpdateEmployeeAsync(id, employeeDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(string id)
    {
        try
        {
            await _employeeManagement.DeleteEmployeeAsync(id);
            return Ok(new { success = true, msg = "员工删除成功!" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { success = false, msg = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, msg = $"内部服务器错误: {ex.Message}" });
        }
    }

}
