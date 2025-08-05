using Asp.Versioning;
using AutoMapper;
using FigureGear.Service.Interface;
using FigureGear.Service.Models;
using FigureGear.Service.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace FigureGear.API.Controllers
{

    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[Controller]")]
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
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            try
            {
                var validationResult = await _userValidator.ValidateAsync(model);

                if (!validationResult.IsValid)
                {
                    validationResult.AddToModelState(ModelState);
                    return BadRequest(ModelState);
                }

                var result = await _userService.RegisterAsync(model);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var result = await _userService.ConfirmEmailAsync(token);
            return Ok(result);
        }

    }
}
