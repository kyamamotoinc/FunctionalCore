using FunctionalCore;

namespace FunctionalCore.Sample;


public class Program
{
    public static async Task Main(string[] args)
    {
        await ResultSampleRun();
        await OptionSampleRun();

        PipelineSamples.ResultPipeline.Run();
        PipelineSamples.OptionPipeline.Run();
    }

    static async Task ResultSampleRun()
    {
        // 基本
        ResultSamples.Sample_01_BasicResult.Run();
        ResultSamples.Sample_02_Match.Run();
        ResultSamples.Sample_03_Map.Run();
        ResultSamples.Sample_04_Bind.Run();
        ResultSamples.Sample_05_Ensure.Run();
        ResultSamples.Sample_06_Tap.Run();
        ResultSamples.Sample_07_TapError.Run();

        // 合成
        ResultSamples.Sample_08_Combine.Run();
        ResultSamples.Sample_09_Sequence.Run();
        ResultSamples.Sample_10_Traverse.Run();

        // 回復・代替
        ResultSamples.Sample_11_Recover.Run();
        ResultSamples.Sample_12_RecoverWith.Run();
        ResultSamples.Sample_13_Or.Run();

        // エラー変換
        ResultSamples.Sample_14_MapError.Run();

        // 値の取り出し
        ResultSamples.Sample_15_GetValueOr.Run();
        ResultSamples.Sample_16_ValueOrThrow.Run();

        // 型変換
        ResultSamples.Sample_17_ToOption.Run();

        // LINQ
        ResultSamples.Sample_18_Select.Run();
        ResultSamples.Sample_19_SelectMany.Run();

        // 非同期処理
        await ResultSamples.Sample_20_AsTask.RunAsync();
        await ResultSamples.Sample_21_MapAsync.RunAsync();
        await ResultSamples.Sample_22_BindAsync.RunAsync();
        await ResultSamples.Sample_23_TapAsync.RunAsync();
        await ResultSamples.Sample_24_TapErrorAsync.RunAsync();
        await ResultSamples.Sample_25_MatchAsync.RunAsync();
        await ResultSamples.Sample_26_EnsureAsync.RunAsync();
    }

    static async Task OptionSampleRun()
    {
        // 基本
        OptionSamples.Sample_01_BasicOption.Run();
        OptionSamples.Sample_02_Match.Run();
        OptionSamples.Sample_03_Map.Run();
        OptionSamples.Sample_04_Bind.Run();
        OptionSamples.Sample_05_Ensure.Run();
        OptionSamples.Sample_06_Tap.Run();
        OptionSamples.Sample_07_TapNone.Run();

        // 合成
        OptionSamples.Sample_08_Combine.Run();
        OptionSamples.Sample_09_Sequence.Run();
        OptionSamples.Sample_10_Traverse.Run();

        // 代替
        OptionSamples.Sample_11_Or.Run();

        // 値の取り出し
        OptionSamples.Sample_12_GetValueOr.Run();
        OptionSamples.Sample_13_ValueOrThrow.Run();

        // 型変換
        OptionSamples.Sample_14_ToResult.Run();

        // LINQ
        OptionSamples.Sample_15_Select.Run();
        OptionSamples.Sample_16_SelectMany.Run();
        OptionSamples.Sample_17_Where.Run();

        // 非同期処理
        await OptionSamples.Sample_18_AsTask.RunAsync();
        await OptionSamples.Sample_19_MapAsync.RunAsync();
        await OptionSamples.Sample_20_BindAsync.RunAsync();
        await OptionSamples.Sample_21_TapAsync.RunAsync();
        await OptionSamples.Sample_22_TapNoneAsync.RunAsync();
        await OptionSamples.Sample_23_MatchAsync.RunAsync();
        await OptionSamples.Sample_24_EnsureAsync.RunAsync();
    }
}