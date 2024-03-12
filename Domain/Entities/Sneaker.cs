using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Sneaker
{
    private Sneaker(Guid id, Guid userId, string name, string brand, Money price, ShoeSize size, Rate rate)
        : this(id, userId, name, brand)
    {
        Price = price;
        Size = size;
        Rate = rate;
    }

    private Sneaker(Guid id, Guid userId, string name, string brand)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Brand = brand;
    }
    
    public Guid Id { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public string Name { get; private set; }
    
    public string Brand { get; private set; }
    
    public Money Price { get; private set; }
    
    public ShoeSize Size { get; private set; }
    
    public Rate Rate { get; private set; }

    public void Update(string name, string brand, Money price, ShoeSize size, Rate rate)
    {
        Name = name;
        Brand = brand;
        Price = price;
        Size = size;
        Rate = rate;
    }

    public static Sneaker Create(Guid userId, string name, string brand, Money price, ShoeSize size, Rate rate)
    {
        return new Sneaker(Guid.NewGuid(), userId, name, brand, price, size, rate);
    }
}
