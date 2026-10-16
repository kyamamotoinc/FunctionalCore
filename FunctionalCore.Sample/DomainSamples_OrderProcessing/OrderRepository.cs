using System.Collections.Generic;

namespace FunctionalCore.Sample.DomainSamples.OrderProcessing
{
    // DB不要のサンプル用リポジトリ。保存失敗も明示的に再現できる。
    /// <summary>
    /// 注文の検索と保存をメモリ上で行うサンプル用リポジトリ。
    /// 検索結果の不在をOption、保存の成否をResultで表し、呼び出し回数で短絡処理を確認できる。
    /// 保存失敗を明示的に再現でき、DBや外部サービスへの接続を必要としない。
    /// </summary>
    public sealed class OrderRepository
    {
        private readonly Dictionary<int, Order> orders = new Dictionary<int, Order>();
        private readonly bool failOnSave;
        public int FindCallCount { get; private set; }
        public int SaveCallCount { get; private set; }

        public OrderRepository(bool failOnSave)
        {
            this.failOnSave = failOnSave;
            orders.Add(100, new Order(100, false));
            orders.Add(200, new Order(200, true));
        }

        public Option<Order> Find(int id)
        {
            FindCallCount++;
            Order? order;
            return orders.TryGetValue(id, out order)
                ? Option<Order>.Some(order)
                : Option<Order>.None;
        }

        public Result<OrderError, Unit> Save(Order order)
        {
            SaveCallCount++;
            if (failOnSave)
                return Result<OrderError, Unit>.Fail(new OrderSaveError(order.Id));

            orders[order.Id] = order;
            // 成功時に返すデータは不要だが、成否は必要なのでUnitを使う。
            return Result<OrderError, Unit>.Ok(Unit.Value);
        }
    }
}
