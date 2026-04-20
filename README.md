# FunctionalCore

A lightweight functional library for C#, centered around `Result<E, T>` and `Option<T>`.  
C# 向け軽量関数型ライブラリ。`Result<E, T>` と `Option<T>` を中心に構成されています。

---

## Background / 設計の背景

This library originated from a real frustration.  
このライブラリは実務の違和感から生まれました。

The goal was simple: **always return a result**.  
目標はシンプルです。**必ず結果を返す**。

This led to a design that turned out to be identical to Haskell's `Either` and `Maybe` — arrived at independently, before knowing they existed.  
辿り着いた設計は、後から知った Haskell の `Either` / `Maybe` と全く同じ構造でした。このライブラリはそのアイデアを C# に自然に溶け込ませたものです。

---

## Design Philosophy / 設計方針

**Minimal / ミニマル**  
Core operations live in the types themselves. Convenience methods belong in Extensions.  
コアな操作のみを型本体に持ち、便利メソッドは Extensions に分離します。

**Null-safe / null 安全**  
`null` is forbidden. Absence is represented explicitly as `None`.  
`null` を禁止し、値の不在は `None` で明示的に表現します。

**Performance / パフォーマンス**  
`readonly struct` avoids heap allocation.  
`readonly struct` によりヒープ割り当てを回避します。

**Explicit over implicit / 暗黙より明示**  
Errors are values, not exceptions. The compiler forces you to handle them.  
エラーは例外ではなく値です。コンパイラがハンドリングを強制します。

---

## Option vs Result / Option と Result の使い分け

| | `Option<T>` | `Result<E, T>` |
|--|-------------|----------------|
| Use when / 使う場面 | 値があるかないか | 成功か失敗か、**なぜ**失敗したか |
| On absence / 不在時 | `None` | `Fail(error)` |
| Error info / エラー情報 | なし | あり（型付き） |

```csharp
// 「あるかないか」だけでよい → Option
// Just presence or absence → Option
Option<User> FindUser(UserId id);

// 「なぜ失敗したか」が必要 → Result
// Need to know why it failed → Result
Result<DomainError, User> GetUser(UserId id);
```

---

## Basic Usage / 基本的な使い方

### `Result<E, T>`

```csharp
// 成功と失敗を値として表現する
// Represent success and failure as values
Result<string, int> ok   = Result<string, int>.Ok(42);
Result<string, int> fail = Result<string, int>.Fail("値が不正です");

// 値の変換 / Transform the value
Result<string, string> mapped = ok.Map(x => x.ToString());

// 失敗しうる処理の連鎖 / Chain operations that may fail
Result<string, Order> result = GetUser(id)
    .Bind(user  => GetOrder(user))
    .Bind(order => Validate(order));

// 成功/失敗の分岐 / Handle both cases
string message = result.Match(
    onSuccess: order => $"注文確定: {order.Id}",
    onFailure: error => $"エラー: {error}"
);
```

### `Option<T>`

```csharp
// 値の存在/不在を明示的に表現する
// Explicitly represent presence or absence
Option<string> some = Option<string>.Some("hello");
Option<string> none = Option<string>.None;

// 値の変換（null は自動的に None に変換）
// Transform the value (null is automatically converted to None)
Option<int> length = some.Map(x => x.Length);

// 存在/不在の分岐 / Handle both cases
string message = some.Match(
    onSome: value => $"値あり: {value}",
    onNone: ()    => "値なし"
);
```

### LINQ query syntax / LINQ クエリ構文

Add `using FunctionalCore.Linq` to use LINQ query syntax.  
`using FunctionalCore.Linq` を追加することで LINQ クエリ構文が使えます。

```csharp
using FunctionalCore.Linq;

// 途中で失敗すると後続は自動的にスキップされる
// If any step fails, subsequent steps are automatically skipped
var result =
    from userId in ValidateUserId(input)
    from user   in GetUser(userId)
    from order  in CreateOrder(user)
    select order;
```

```csharp
// Option も同様
// Same applies to Option
var city =
    from user    in FindUser(id)
    from address in FindAddress(user)
    select address.City;
```

---

## Namespaces / 名前空間

| Namespace | Contents |
|-----------|----------|
| `FunctionalCore` | `Result<E, T>` / `Option<T>` / `Unit` |
| `FunctionalCore.Linq` | LINQ query syntax support / LINQ クエリ構文サポート |
| `FunctionalCore.Extensions` | Utility extension methods / 拡張メソッド群 |

---

## License

MIT
