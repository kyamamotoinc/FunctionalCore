namespace FunctionalCore.Sample.DomainSamples.OrderProcessing
{
    // 同じドメインの失敗を共通のEで表し、呼び出し側では具体的な型で判断する。
    /// <summary>
    /// 注文確定処理で発生する想定内の失敗を表す基底クラス。
    /// ドメイン共通のエラー型としてResultの型引数に指定し、具体的な失敗を派生型で区別する。
    /// </summary>
    public abstract class OrderError
    {
        public abstract string Message { get; }
    }

    /// <summary>
    /// 注文番号が1未満であり、入力検証に失敗したことを表す。
    /// </summary>
    public sealed class InvalidOrderIdError : OrderError
    {
        public int OrderId { get; private set; }
        public InvalidOrderIdError(int orderId) { OrderId = orderId; }
        public override string Message { get { return "注文番号は1以上で指定してください: " + OrderId; } }
    }

    /// <summary>
    /// 指定した注文番号に対応する注文が見つからなかったことを表す。
    /// </summary>
    public sealed class OrderNotFoundError : OrderError
    {
        public int OrderId { get; private set; }
        public OrderNotFoundError(int orderId) { OrderId = orderId; }
        public override string Message { get { return "注文が見つかりません: " + OrderId; } }
    }

    /// <summary>
    /// 注文がすでに確定済みであり、再度確定できないことを表す。
    /// </summary>
    public sealed class OrderAlreadyConfirmedError : OrderError
    {
        public int OrderId { get; private set; }
        public OrderAlreadyConfirmedError(int orderId) { OrderId = orderId; }
        public override string Message { get { return "注文は確定済みです: " + OrderId; } }
    }

    /// <summary>
    /// 注文の保存に失敗したことを表す。
    /// </summary>
    public sealed class OrderSaveError : OrderError
    {
        public int OrderId { get; private set; }
        public OrderSaveError(int orderId) { OrderId = orderId; }
        public override string Message { get { return "注文の保存に失敗しました: " + OrderId; } }
    }
}
