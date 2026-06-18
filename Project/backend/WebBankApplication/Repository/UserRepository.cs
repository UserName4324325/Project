using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebBankApplication.Data;
using WebBankApplication.DTOs;
using WebBankApplication.Models;

namespace WebBankApplication.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ElasticsearchClient _elasticClient;
    public UserRepository(AppDbContext context, ElasticsearchClient elasticsearchClient)
    {
        _context = context;
        _elasticClient = elasticsearchClient;
    }

    public async Task<UserResponseDtos?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        return MapToUserResponseDto(user);
    }   
    private UserResponseDtos? MapToUserResponseDto(User? user) =>
        user == null ? null : new UserResponseDtos(user.Id, user.FullName, user.Email, user.Balance);
    
    public async Task<List<AllUsersResponseDtos>> GetAllUsersAsync(Guid CurrentUserId)
    {
        return await _context.Users.Select(AsAllUsersResponseDtos).ToListAsync();
    }

    private static readonly Expression<Func<User, AllUsersResponseDtos>> AsAllUsersResponseDtos = u =>
    new AllUsersResponseDtos
    (
        u.Id,
        u.FullName,
        u.Email
    );

    // в разработке
    public async Task<List<AllUsersResponseDtos>> SearchUsersAsync(string query, Guid currentUserId)
    {        
        if (string.IsNullOrWhiteSpace(query)) return new List<AllUsersResponseDtos>();

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
                        .Term(t => t.Field("id").Value(FieldValue.String(currentUserId.ToString())))
                    )
                )
            )
            .Size(10)
        );

        if (!searchResponse.IsValidResponse)
        {
            Console.WriteLine($"[Elasticsearch Error]: {searchResponse.DebugInformation}");
            return new List<AllUsersResponseDtos>();
        }


        return searchResponse.Documents.Select(MapToResponseAllUsersDto).ToList();
    }

    private AllUsersResponseDtos MapToResponseAllUsersDto(User u) =>
        new AllUsersResponseDtos(u.Id, u.FullName, u.Email);
}
