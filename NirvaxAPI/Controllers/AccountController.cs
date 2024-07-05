﻿using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess.DAOs;
using DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Service;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;

        public AccountController(IAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var accounts = await _repository.GetAllAccountAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var account = await _repository.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/ban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanAccount(int id)
        {
            try
            {
                var account = await _repository.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }

                await _repository.BanAccountAsync(account);
                return Ok(new { message = "Account banned successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/unban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnbanAccount(int id)
        {
            try
            {
                var account = await _repository.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }

                await _repository.UnbanAccountAsync(account);
                return Ok(new { message = "Account unbanned successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchAccounts([FromQuery] string keyword)
        {
            try
            {
                var accounts = await _repository.SearchAccountAsync(keyword);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/change-password")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePassword changePassword)
        {
            try
            {
                var account = await _repository.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }
                if(!PasswordHasher.VerifyPassword(changePassword.OldPassword, account.Password))
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable, new { message = "The old password is incorrect." });
                }
                if (changePassword.NewPassword != changePassword.ConfirmPassword)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable, new { message = "The new password and confim password do not match." });
                }
                account.Password = PasswordHasher.HashPassword(changePassword.NewPassword);
                await _repository.UpdateAccountAsync(account);
                return Ok(new { message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/update-profile")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAccount(int id,UpdateUserDTO model)
        {
            try
            {
                var account = await _repository.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }
                _mapper.Map(model, account);
                await _repository.UpdateAccountAsync(account);
                return Ok(new { message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/{avatar}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAvatar(int id, string avatar)
        {
            try
            {
                var account = await _repository.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found." });
                }
                account.Image = avatar;
                await _repository.UpdateAccountAsync(account);
                return Ok(new { message = "Avatar updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }

}
