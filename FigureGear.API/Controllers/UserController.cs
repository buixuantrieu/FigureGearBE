using Asp.Versioning;
using AutoMapper;
using FigureGear.Service.Interface;
using FigureGear.Service.Models;
using FigureGear.Service.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FigureGear.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserValidator _userValidator;
        private readonly IUserService _userService;

        public UserController(UserValidator userValidator, IUserService userService, IMapper mapper, ITokenService tokenService)
        {
            _userValidator = userValidator;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            var validationResult = await _userValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            var response = await _userService.RegisterAsync(model);

            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { error = "Token is required." });
            }

            var response = await _userService.ConfirmEmailAsync(token);

            return StatusCode(response.StatusCode, response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserModel model)
        {
            var response = await _userService.LoginAsync(model);

            return StatusCode(response.StatusCode, response);
        }
    }
}
