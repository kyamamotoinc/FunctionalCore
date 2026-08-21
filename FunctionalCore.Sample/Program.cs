using FunctionalCore;
using FunctionalCore.Sample.Result;

namespace FunctionalCore.Sample;


public class Program
{
    public static void Main(string[] args)
    {
        // Result 基本
        ResultSamples.Sample_01_BasicResult.Run();
        ResultSamples.Sample_02_Match.Run();
        ResultSamples.Sample_03_Map.Run();
        ResultSamples.Sample_04_Bind.Run();
        ResultSamples.Sample_05_Ensure.Run();
        ResultSamples.Sample_06_Tap.Run();
        ResultSamples.Sample_07_TapError.Run();

        // Resultの合成
        ResultSamples.Sample_08_Combine.Run();
        ResultSamples.Sample_09_Sequence.Run();
        ResultSamples.Sample_10_Traverse.Run();

        // 失敗からの回復・変換
        ResultSamples.Sample_11_Recover.Run();
        ResultSamples.Sample_12_RecoverWith.Run();
        ResultSamples.Sample_14_MapError.Run();




        //
    }
}