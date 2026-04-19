using System;

namespace WebBankApplication.DTOs;

//public record UserUpdateDtos
//(
//    string FullName,
//    string Email,
//    string Password
//);

public record UserResponseDtos
(
    Guid Id,
    string FullName,
    string Email,
    decimal Balance
);
    
public record AllUsersResponseDtos 
(
    Guid Id,
    string FullName,
    string Email
);