using FluentValidation;
using FraudSys.Application.DTOs;
using FraudSys.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FraudSys.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountLimitsController : ControllerBase
    {
        private readonly IAccountLimitService _service;
        private readonly IValidator<CreateAccountLimitDto> _createValidator;
        private readonly IValidator<UpdateAccountLimitDto> _updateValidator;
        private readonly ILogger<AccountLimitsController> _logger;

        public AccountLimitsController(
            IAccountLimitService service,
            IValidator<CreateAccountLimitDto> createValidator,
            IValidator<UpdateAccountLimitDto> updateValidator,
            ILogger<AccountLimitsController> logger)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        /// <summary>
        /// Cadastra um novo limite de conta
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AccountLimitResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAccountLimitDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            _logger.LogInformation("Criando limite para CPF: {Document}", dto.Document);

            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetByDocument), new { document = result.Document }, result);
        }

        /// <summary>
        /// Busca limite por CPF
        /// </summary>
        [HttpGet("{document}")]
        [ProducesResponseType(typeof(AccountLimitResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByDocument(string document)
        {
            _logger.LogInformation("Buscando limite para CPF: {Document}", document);

            var result = await _service.GetByDocumentAsync(document);

            return Ok(result);
        }

        /// <summary>
        /// Atualiza o limite de uma conta
        /// </summary>
        [HttpPut("{document}")]
        [ProducesResponseType(typeof(AccountLimitResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string document, [FromBody] UpdateAccountLimitDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            _logger.LogInformation("Atualizando limite para CPF: {Document}", document);

            var result = await _service.UpdateAsync(document, dto);

            return Ok(result);
        }

        /// <summary>
        /// Remove um limite de conta
        /// </summary>
        [HttpDelete("{document}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string document)
        {
            _logger.LogInformation("Removendo limite para CPF: {Document}", document);

            await _service.DeleteAsync(document);

            return NoContent();
        }
    }
}
