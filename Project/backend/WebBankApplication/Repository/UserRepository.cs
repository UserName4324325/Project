using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebBankApplication.Data;
using WebBankApplication.DTOs;
using WebBankApplication.Models;

namespace WebBankApplication.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ElasticsearchClient _elasticClient;
    private readonly ILogger<UserRepository> _logger;
    public UserRepository(AppDbContext context, ElasticsearchClient elasticsearchClient,  ILogger<UserRepository> logger)
    {
        _context = context;
        _elasticClient = elasticsearchClient;
        _logger = logger;
    }

    public async Task<UserResponseDtos?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        return MapToUserResponseDto(user);
    }   
    private UserResponseDtos? MapToUserResponseDto(User? user) =>
        user == null ? null : new UserResponseDtos(user.Id, user.FullName, user.Email, user.Balance);
    
    public async Task<List<UsersResponseDtos>> GetAllUsersAsync()
    {
        return await _context.Users.Select(AsAllUsersResponseDtos).ToListAsync();
    }

    private static readonly Expression<Func<User, UsersResponseDtos>> AsAllUsersResponseDtos = u =>
    new UsersResponseDtos
    (
        u.Id,
        u.FullName,
        u.Email
    );
    
    public async Task<List<UsersResponseDtos>> SearchUsersAsync(string query, Guid currentUserId)
    {        
        if (string.IsNullOrWhiteSpace(query)) return new List<UsersResponseDtos>();

        var searchResponse = await _elasticClient.SearchAsync<User>(s => s
            .Indices("users")
            .Query(q => q
                .Bool(b => b
                    .Must(must => must
                        .MultiMatch(mm => mm
                            .Query(query)
                            .Fields(new[] { "fullName", "email" })
                            .Fuzziness(new Fuzziness("AUTO")) 
                        )
                    )
                    .MustNot(mn => mn
                        .Term(t => t.Field("id.keyword").Value(FieldValue.String(currentUserId.ToString())))
                    )
                )
            )
            .Size(10)
        );

        if (!searchResponse.IsValidResponse)
        {
            _logger.LogError($"[Elasticsearch Error]: {searchResponse.DebugInformation}");
            return new List<UsersResponseDtos>();
        }


        return searchResponse.Documents.Select(MapToResponseAllUsersDto).ToList();
    }

    private UsersResponseDtos MapToResponseAllUsersDto(User u) =>
        new UsersResponseDtos(u.Id, u.FullName, u.Email);
}
