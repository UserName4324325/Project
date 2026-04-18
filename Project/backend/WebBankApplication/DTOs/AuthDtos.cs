using System;

namespace WebBankApplication.DTOs;


public record UserRegistrationDto
(
    string FullName,
    string Email,
    string Password
);

public record UserLoginDto
(
    string Email,
    string Password
);

public record UserAuthResponseDto
(
    Guid Id,
    string Token, 
    string FullName,
    decimal Balance
);
