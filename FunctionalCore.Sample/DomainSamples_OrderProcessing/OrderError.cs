namespace FunctionalCore.Sample.DomainSamples.OrderProcessing
{
    // 同じドメインの失敗を共通のEで表し、呼び出し側では具体的な型で判断する。
    public abstract class OrderError
    {
        public abstract string Message { get; }
    }

    public sealed class InvalidOrderIdError : OrderError
    {
        public int OrderId { get; private set; }
        public InvalidOrderIdError(int orderId) { OrderId = orderId; }
        public override string Message { get { return "注文番号は1以上で指定してください: " + OrderId; } }
    }

    public sealed class OrderNotFoundError : OrderError
    {
        public int OrderId { get; private set; }
        public OrderNotFoundError(int orderId) { OrderId = orderId; }
        public override string Message { get { return "注文が見つかりません: " + OrderId; } }
    }

    public sealed class OrderAlreadyConfirmedError : OrderError
    {
        public int OrderId { get; private set; }
        public OrderAlreadyConfirmedError(int orderId) { OrderId = orderId; }
        public override string Message { get { return "注文は確定済みです: " + OrderId; } }
    }

    public sealed class OrderSaveError : OrderError
    {
        public int OrderId { get; private set; }
        public OrderSaveError(int orderId) { OrderId = orderId; }
        public override string Message { get { return "注文の保存に失敗しました: " + OrderId; } }
    }
}
