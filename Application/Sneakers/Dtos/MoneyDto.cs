using Domain.Enums;
using FluentValidation;

namespace Application.Sneakers.Dtos;

public record MoneyDto(Currency Currency, decimal Amount);