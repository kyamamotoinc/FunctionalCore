namespace FunctionalCore.Sample.DomainSamples.OrderProcessing
{
    public sealed class Order
    {
        public int Id { get; private set; }
        public bool IsConfirmed { get; private set; }

        public Order(int id, bool isConfirmed)
        {
            Id = id;
            IsConfirmed = isConfirmed;
        }

        public Order Confirm()
        {
            // 元の注文を変更しない。保存失敗時にメモリ上だけ確定済みになるのを防ぐ。
            return new Order(Id, true);
        }
    }
}
