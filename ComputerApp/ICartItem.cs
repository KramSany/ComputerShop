namespace ComputerApp
{
    /// <summary>
    /// Интерфейс корзины каждого покупателя 
    /// </summary>
    public interface ICartItem
    {
        int ID { get; }
        string Name { get; }
        decimal Price { get; }
        bool IsInCart { get; set; }
    }
}
