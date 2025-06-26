using System;
using System.Threading;
using System.Threading.Tasks;
using TMS.Contract.Entities;
using TMS.Core.Interfaces;
using System.Linq;

namespace TMS.Application.Handlers.CustomHandlers.EmployeeHandlers
{
    public class DeleteEmployeeCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
        public DeleteEmployeeCommand(int employeeId) => EmployeeId = employeeId;
    }

    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
    {
        private readonly IEntityCommiter _commiter;

        public DeleteEmployeeCommandHandler(IEntityCommiter commiter)
        {
            _commiter = commiter;
        }

        public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            // Set CreatedByEmployeeId to null for all related tasks
            var tasksResult = await _commiter.Tasks.GetAsync(t => t.CreatedByEmployeeId == request.EmployeeId);
            if (tasksResult.Data != null)
            {
                if (tasksResult.Data is IEnumerable<WorkTask> tasks)
                {
                    foreach (var task in tasks)
                    {
                        task.CreatedByEmployeeId = null;
                    }
                }
                await _commiter.CommitAsync();
            }

            // Set TeamLeaderId to null for all departments where this employee is the team leader
            var departmentsResult = await _commiter.Departments.GetAsync(d => d.TeamLeaderId == request.EmployeeId);
            if (departmentsResult.Data != null)
            {
                if (departmentsResult.Data is IEnumerable<Department> departments)
                {
                    foreach (var dept in departments)
                    {
                        dept.TeamLeaderId = null;
                    }
                }
                await _commiter.CommitAsync();
            }

            // Delete the employee
            var employeeResult = await _commiter.Employees.GetAsync(e => e.Id == request.EmployeeId);
            if (employeeResult.Data == null)
                return false;
            await _commiter.Employees.RemoveAsync(x => x.Id == employeeResult.Data.Id);
            await _commiter.CommitAsync();
            return true;
        }
    }
} 