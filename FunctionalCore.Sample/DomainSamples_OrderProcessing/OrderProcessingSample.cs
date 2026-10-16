using System;

namespace FunctionalCore.Sample.DomainSamples.OrderProcessing
{
    /// <summary>
    /// 独自エラー型を使った注文確定処理を実行するサンプル。
    /// 成功、入力不正、注文未検出、確定済み、保存失敗の5ケースについて、
    /// 結果の型、後続処理の呼び出し回数、保存状態を検証する。
    /// </summary>
    public static class OrderProcessingSample
    {
        public static void Run()
        {
            Console.WriteLine("=== Domain Sample : Order Processing ===");
            RunScenario("正常", 100, false, null, 1, 1);
            RunScenario("入力不正", 0, false, typeof(InvalidOrderIdError), 0, 0);
            RunScenario("注文未検出", 999, false, typeof(OrderNotFoundError), 1, 0);
            RunScenario("確定済み", 200, false, typeof(OrderAlreadyConfirmedError), 1, 0);
            RunScenario("保存失敗", 100, true, typeof(OrderSaveError), 1, 1);
            Console.WriteLine();
        }

        private static void RunScenario(string name, int id, bool failOnSave,
            Type? expectedError, int expectedFinds, int expectedSaves)
        {
            var repository = new OrderRepository(failOnSave);
            var logs = 0;
            var service = new OrderService(repository, message =>
            {
                logs++;
                Console.WriteLine("  ログ: " + message);
            });

            Console.WriteLine("--- " + name + " ---");
            var result = service.ConfirmOrder(id);
            // 通知方法の判断は最後の境界で行う。途中の処理は表示に依存しない。
            result.Match(
                onSuccess: _ => Console.WriteLine("  成功"),
                onFailure: error => Console.WriteLine("  " + error.Message + " / " + GetAdvice(error)));
            Console.WriteLine("  検索=" + repository.FindCallCount +
                ", 保存=" + repository.SaveCallCount + ", 成功ログ=" + logs);

            // サンプル自身でも、短絡処理とエラー型の期待を検証する。
            if (result.IsSuccess != (expectedError == null) ||
                (result.IsFailure && result.Error.GetType() != expectedError) ||
                repository.FindCallCount != expectedFinds ||
                repository.SaveCallCount != expectedSaves ||
                logs != (result.IsSuccess ? 1 : 0))
                throw new InvalidOperationException("シナリオの結果が不正です: " + name);

            if (id == 100)
            {
                var stored = repository.Find(id).Value;
                if (stored.IsConfirmed != result.IsSuccess)
                    throw new InvalidOperationException("保存状態が不正です: " + name);
            }
        }

        private static string GetAdvice(OrderError error)
        {
            // メッセージ文字列の比較ではなく、型に応じて対応を変える。
            if (error is InvalidOrderIdError) return "入力を修正してください。";
            if (error is OrderNotFoundError) return "注文番号を確認してください。";
            if (error is OrderAlreadyConfirmedError) return "再度確定する必要はありません。";
            if (error is OrderSaveError) return "保存先の状態を確認して再試行してください。";
            return "管理者に連絡してください。";
        }
    }
}
