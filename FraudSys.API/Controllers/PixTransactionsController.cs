using FluentValidation;
using FraudSys.Application.DTOs;
using FraudSys.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FraudSys.API.Controllers
{
    [ApiController]
    [Route("api/pix/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IPixTransactionService _service;
        private readonly IValidator<ProcessPixTransactionDto> _validator;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(
            IPixTransactionService service,
            IValidator<ProcessPixTransactionDto> validator,
            ILogger<TransactionsController> logger)
        {
            _service = service;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Processa uma transação PIX
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PixTransactionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessTransaction([FromBody] ProcessPixTransactionDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            _logger.LogInformation("Processando transação PIX para CPF: {Document}, Valor: {Amount}",
                dto.Document, dto.Amount);

            var result = await _service.ProcessTransactionAsync(dto);

            _logger.LogInformation("Transação processada - Status: {Status}, ID: {TransactionId}",
                result.Status, result.TransactionId);

            return Ok(result);
        }
    }
}
