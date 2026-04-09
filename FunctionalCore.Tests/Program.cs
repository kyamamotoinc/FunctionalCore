using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionalCore;
using FunctionalCore.Linq;
using FunctionalCore.Extensions;

namespace FunctionalCore.Tests;

//internal class Program
//{
//    static void Main(string[] args)
//    {
//        ResultSample();
//    }

//    static Result<string, int> Divide(int a, int b)
//    {
//        if (b == 0)
//        {
//            //0で割られることを想定し、Result<string, int>.Failを返す。
//            return Result<string, int>.Fail("Cannot be divided by 0");
//        }
//        return Result<string, int>.Ok(a / b);
//    }

//    static void ResultSample()
//    {
//        //
//        var res = from x in Result<string, int>.Ok(3)
//                  from y in Result<string, int>.Ok(5)
//                  from z in Result<string, int>.Ok(7)
//                  select x + y + z;

//        var rs = Result<string, int>.Ok(11);
//        var re = Result<string, int>.Fail("ERROR! ERROR!");

//        //-- Map --
//        //エラーが無ければ次の処理を実行する。
//        //エラーがある場合は処理をそこで止めて次の処理を行わない。
//        var r1 = rs.Map(x => x + 1).Map(x => x * 2);
//        var r2 = re.Map(x => x + 1).Map(x => x * 2);

//        //-- MapError --
//        //エラーがある場合にErrorDataを変換する
//        //var r1e = rs.MapError(x => "");
//        //var r2e = re.MapError(x => 0);

//        //-- Bind --
//        //割り算を行うDivideメソッドを適用する。
//        //Divideメソッドは0で割る場合にエラーを表すErrorDataを含むResultを返す。
//        var r3 = r1.Bind(x => Divide(x, 2));
//        var r4 = r1.Bind(x => Divide(x, 0));

//        //-- Match --
//        //Matchにて値を返す。エラーが発生している場合には引数の0を返す。処理の最終地点で使用する
//        var r5 = r3.Match(x => x, _ => 0);
//        var r6 = r4.Match(x => x, _ => 0);

//        //-- GetValueOrDefault --
//        //ValueOrDefault()で値を取得する。
//        //Resultが失敗している場合は引数の値を返す。
//        var r7 = r3.GetValueOrDefault();
//        var r8 = r4.GetValueOrDefault();

//        //--ValueOrThrow
//        //ValueOrThrow()で値を取得する。
//        //Resultが失敗している場合は引数の例外を発生させる。
//        var r9 = r3.ValueOrThrow(err => new InvalidOperationException(err));
//        //var r10 = r4.ValueOrThrow(err => new InvalidOperationException(err));

//        //-- Tap --
//        //Tap()はResultが成功している時に副作用を実行する。
//        //失敗したときは実行しない。
//        int testNumber;
//        var r11 = r3.Tap(x => testNumber = x); //副作用(外側の変数に束縛)が実施される
//        var r12 = r4.Tap(x => testNumber = x + 100); //副作用(外側の変数に束縛)は実施されない

//        //-- TapError --
//        //TapError()はResultが失敗している時に副作用を実行する。
//        //成功したときは副作用を実行しない。
//        string testError;
//        //var r14 = r3.TapError(x => testError = x); //副作用(外側の変数に束縛)は実施されない
//        //var r15 = r4.TapError(x => testError = x); //副作用(外側の変数に束縛)が実施される

//        //-- TapAll --
//        //TapAll()はResultの成功・失敗に関わらず副作用を実行する。
//        //成功したときは副作用を実行しない。
//        int count = 0;
//        //var r16 = r3.TapBoth(_ => count += 1); //副作用(外側の変数に束縛)実施される
//        //var r17 = r4.TapBoth(_ => count += 1); //副作用(外側の変数に束縛)が実施される
//        var results = new[]
//        {
//            Result<string, int>.Ok(1),
//            Result<string, int>.Ok(2),
//            Result<string, int>.Fail("Error!")
//        };

//        var results1 = new[]
//        {
//            Result<string, int>.Ok(1),
//            Result<string, int>.Ok(2),
//            Result<string, int>.Ok(3)
//        };

//        //-- Ensure --
//        //
//        //
//        var r18 = Result<string, int>.Ok(10).Ensure(x => x > 9, x => "Number should be larger than 9");
//        var r19 = Result<string, int>.Ok(10).Ensure(x => x > 20, x => "Number should be larger than 20");

//        var sequenced = results.Sequence();
//        // 失敗があるので、Result<string, IList<int>> は Fail("Error!") になる
//        var sequenced1 = results1.Sequence();

//        var numbers = new[] { 1, 2, 3 };
//        var traversed = numbers.Traverse(n => n == 2 ? Result<string, int>.Fail("No 2 allowed") : Result<string, int>.Ok(n));
//        // 2 があるので Fail("No 2 allowed") になる
//        var numbers1 = new[] { 1, 3, 4 };
//        var traversed1 = numbers1.Traverse(n => n == 2 ? Result<string, int>.Fail("No 2 allowed") : Result<string, int>.Ok(n));
//    }

//}
