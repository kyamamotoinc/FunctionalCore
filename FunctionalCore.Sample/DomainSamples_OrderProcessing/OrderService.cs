using System;
using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.DomainSamples.OrderProcessing
{
    /// <summary>
    /// 注文番号の検証、検索、確定条件の確認、確定、保存を連結するサービス。
    /// 共通のOrderErrorを保持したまま成功値を変換し、成功時だけログを記録する。
    /// </summary>
    public sealed class OrderService
    {
        private readonly OrderRepository repository;
        private readonly Action<string> log;

        public OrderService(OrderRepository repository, Action<string> log)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (log == null) throw new ArgumentNullException(nameof(log));
            this.repository = repository;
            this.log = log;
        }

        public Result<OrderError, Unit> ConfirmOrder(int orderId)
        {
            // EはOrderErrorのまま、成功値がint → Order → Unitと変わる。
            // 各段階の失敗はBindが引き継ぎ、後続処理を実行しない。
            return Result<OrderError, int>.Ok(orderId)
                .Ensure(id => id > 0, id => new InvalidOrderIdError(id))
                .Bind(id => repository.Find(id)
                    // 検索の不在はOption。この業務では注文が必須なので失敗へ変換する。
                    .ToResult<OrderError, Order>(() => new OrderNotFoundError(id)))
                .Ensure(order => !order.IsConfirmed,
                    order => new OrderAlreadyConfirmedError(order.Id))
                .Map(order => order.Confirm())
                .Bind(repository.Save)
                .Tap(_ => log("注文を確定しました。"));
        }
    }
}
