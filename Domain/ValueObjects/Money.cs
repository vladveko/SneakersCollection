using Domain.Enums;

namespace Domain.ValueObjects;

public record Money(Currency Currency, decimal Amount);