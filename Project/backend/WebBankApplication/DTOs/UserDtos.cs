using System;

namespace WebBankApplication.DTOs;

public record UserUpdateDtos
(
    Guid Id,
    string FullName,
    string Email,
    string CurrentPassword,
    string NewPassword
);

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