using Dapper;
using ExamScheduleService.Application.DTOs;
using ExamScheduleService.Application.Interfaces;
using ExamScheduleService.Application.Mappings;
using ExamScheduleService.Domain.Entities;
using ExamScheduleService.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ExamScheduleService.Infrastructure.Repositories;

/// <summary>
/// Retrieves exam schedule rows from SQL Server through stored procedures.
/// </summary>
public sealed class ExamScheduleRepository : IExamScheduleRepository
{
    private const string StoredProcedureName = "PKG_GET_EXAM_SCHEDULE_FOR_STUDENT";
    private readonly SqlHelper _sqlHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExamScheduleRepository"/> class.
    /// </summary>
    /// <param name="sqlHelper">The SQL connection helper.</param>
    public ExamScheduleRepository(SqlHelper sqlHelper)
    {
        _sqlHelper = sqlHelper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ExamSchedule>> GetScheduleAsync(ExamScheduleRequest request,CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {

            var parameters = new DynamicParameters();
            parameters.Add("@P_STUDENT_ID", request.StudentId, DbType.Int32);
            parameters.Add("@P_ACD_SESS_ID", request.AcdSessId, DbType.Int32);
            parameters.Add(
                "@P_SUBJECT_SEARCH",
                string.IsNullOrWhiteSpace(request.SubjectSearch) ? null : request.SubjectSearch.Trim(),
                DbType.String,
                size: 100);
            using var connection = _sqlHelper.CreateConnection();

            return await connection.QueryAsync<ExamSchedule>(new CommandDefinition(SpNames.GetExamScheduleForStudent, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken)).ConfigureAwait(false);
        }
        catch (SqlException ex)
        {
            throw new ("An error occurred while retrieving exam schedule data.", ex);
        }

    }
}
