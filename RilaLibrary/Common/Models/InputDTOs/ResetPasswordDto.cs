﻿namespace Common.Models.InputDTOs
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
