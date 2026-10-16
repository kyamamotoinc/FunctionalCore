namespace FunctionalCore.Sample.DomainSamples.OrderProcessing
{
    /// <summary>
    /// 注文番号と確定状態を保持するサンプル用の注文クラス。
    /// 確定操作では新しい注文を生成し、保存前に元の注文の状態を変更しない。
    /// </summary>
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
