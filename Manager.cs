using System;
using System.Collections.Generic;
using System.Text;

namespace MemSync
{
    public struct meminfo
    {
        string name;
        int start;
        int size;
        bool memlock;
        string[] allowed;
    };

    //次回
    //cmdinfoを、1命令ずつ保存にする
    //Parseの返す関数をcmdinfo[]にするかを検討
    public class cmdinfo
    {
        public List<byte[]> cmd;
        public long elapsed;
        public long? changed_index = null;
        public long? changed_size = null;
        public byte[] originalValue;
    }

    static class Manager
    {
        static byte[] Memory = new byte[2 ^ 16];

        //static Dictionary<byte[]> commandHistory;

        // スタックの作成
        static Stack<cmdinfo> cmdHistory = new Stack<cmdinfo>();

        //実行を待っている命令を保存する変数
        static PriorityQueue<byte[], long> commandQueue;

        //指定した位置から、sizeバイトコピーします。
        static byte[] GetBytes(byte[] mem, int index, int size)
        {
            byte[] result = new byte[size];
            Buffer.BlockCopy(mem, index, result, 0, size);
            return result;
        }

        //第一引数のindexから、binで上書きします。
        static void OverwriteArray(byte[] memory, int index, byte[] bin)
        {
            // インデックスが配列の範囲内か確認
            if (index < 0 || index + bin.Length > memory.Length)
            {
                Console.WriteLine("Error: 指定されたインデックスが無効です。");
                return;
            }

            // 上書き処理
            Array.Copy(bin, 0, memory, index, bin.Length);
        }

        //命令の引数タイプ
        enum ParameterType
        {
            point,
            size,
            ipaddress,
        }

        static bool ValidateArgFormat(ParameterType type, byte[] bin)
        {
            switch (type)
            {
                case ParameterType.point:
                case ParameterType.size:
                    if (bin.Count() == 8)
                        return true;
                    else
                        return false;
                    break;
                case ParameterType.ipaddress:
                    if (bin.Count() == 16)
                        return true;
                    else
                        return false;
                    break;
            }
            return true;
        }
        /*
        cmdinfoに保存された命令を実行します。実行したコマンドは、履歴に保存されます。
        cmdinfoは参照渡しで、代入後編集されます。
        */
        public static void run(ref cmdinfo info)
        {
            //00: write  [point: long] [value: byte[]]
            //01: fill   [point: long] [size: long] [value:byte[]]
            //02: del    [point: long] [size: long]
            //03: join-notification [ipaddress: 16byte]
            //04: quit-notification [ipaddress: 16byte]
            //05: ping [ipaddress: 16byte] [count: byte]
            //06: ping [result: number]
            //07:
            List<byte[]> cmd = info.cmd;
            //実行
            switch (cmd[0][0])
            {
                case 00:
                    //引数の個数をチェック
                    if (cmd.Count() != 3)
                    {
                        Console.WriteLine("write命令の引数は2つである必要があります。");
                        return;
                    }
                    //入力された引数のフォーマットを確認
                    if (!ValidateArgFormat(ParameterType.point, cmd[1]))
                    {
                        Console.WriteLine("入力された引数のフォーマットが正しくありません。");
                        return;
                    }

                    //引数を数値に変換
                    long point = BitConverter.ToInt64(cmd[1], 0);
                    //指定した位置がメモリからはみ出ていないかを確認
                    if (point >= Memory.Count && point + cmd[2].Count() - 1 >= Memory.Count())
                        return;
                    //編集前の内容を保存
                    info.originalValue = GetBytes(Memory, point, cmd[2].Count());
                    //書き込み
                    OverwriteArray(Memory, point, cmd[2]);
                    break;
                case 01:
                    //引数の個数をチェック
                    if (cmd.Count() != 4)
                    {
                        Console.WriteLine("fill命令の引数は3つである必要があります。");
                        return;
                    }
                    //入力された引数のフォーマットをチェック
                    if (
                        !ValidateArgFormat(ParameterType.point, cmd[1])
                        || !ValidateArgFormat(ParameterType.size, cmd[2])
                    )
                    {
                        Console.WriteLine("入力された引数のフォーマットが正しくありません。");
                        return;
                    }
                    //引数を数値に変換
                    long point = BitConverter.ToInt64(cmd[1], 0);
                    long size = BitConverter.ToInt64(cmd[2], 0);
                    //指定した位置がメモリからはみ出ていないかを確認
                    if (point >= Memory.Count && point + size - 1 >= Memory.Count())
                    {
                        Console.WriteLine("ポインタ、またはサイズの指定が不適切です。");
                        return;
                    }
                    //編集前の内容を保存
                    info.originalValue = GetBytes(Memory, point, size);
                    //書き込み
                    OverwriteArray(Memory, point, cmd[2]);
                    break;
            }
            cmdHistory.Push(cmdHistory);
            return;
        }

        //最後に実行したコマンドを巻き戻す
        public static void undo() 
        {
            
        }

        /*
            命令形式
            [1byte: 命令数] [8byte: 経過時間]
            [1byte: 命令]  [1byte: 引数の個数]
            [8byte: 引数の大きさ] [不定: 引数]...
            [1byte: 命令]...]

            取得したバイナリをcmdinfo形式に変換します。
        */
        public static cmdinfo[] ParseCommands(byte[] bin)
        {
            /*
                List: 命令の集合
                    List: オペコード、引数など集合
                        byte[]: オペコード、引数などの個別の塊
                [
                    [
                        [オペコード],
                        [引数1]
                    ]
                ]
            */

            //命令数
            byte cmd_count = bin[0];

            //返す値
            cmdinfo[] ret = new cmdinfo[cmd_count];

            long index = 1;
            byte[] _timelapse_bin = new byte[8];

            //経過時間を取得
            for (byte i = 0; i < 8; i++)
            {
                _timelapse_bin[i] = bin[index++];
            }

            long elapsed = BitConverter.ToInt64(_timelapse_bin, 0);

            //初期化して経過時間をretに代入していく(PriorityQueueで順序が保たれるように+1ずつしていく)
            for (byte i = 0; i < cmd_count; i++)
            {
                ret[i] = new cmdinfo();
                ret[i].elapsed = elapsed + i;
            }

            //命令ごとのループ
            for (byte i = 0; i < cmd_count; i++)
            {
                List<byte[]> cmd = new List<byte[]>();
                //オペコードをプッシュ
                cmd.Add([bin[index++]]);
                byte arg_count = bin[index++];
                //引数ごとループ
                for (byte j = 0; j < arg_count; j++)
                {
                    //引数のデータ長を取り出す
                    byte[] size_bin = new byte[8];
                    for (byte k = 0; k < 8; k++)
                    {
                        size_bin[k] = bin[index++];
                    }
                    //シリアライズしたデータを元に戻す
                    long size = BitConverter.ToInt64(size_bin, 0);

                    //引数を抽出する
                    cmd.Add(new byte[size]);
                    for (long k = 0; k < size; k++)
                    {
                        cmd[cmd.Count() - 1][k] = bin[index++];
                    }
                }

                //1つの命令のパースした結果を代入
                ret[i].cmd = cmd;
            }

            return ret;
        }

        public static void outCmdInfo(cmdinfo[] info)
        {
            //デバッグ用の結果出力
            for (int j = 0; j < info.Count(); j++)
            {
                Console.WriteLine("+-----------------+");
                info[j]
                    .cmd.ForEach(list =>
                    {
                        Console.WriteLine("#######################");
                        for (int i = 0; i < list.Count(); i++)
                        {
                            Console.Write($"{list[i]} ");
                        }
                        Console.Write("\r\n");
                    });
            }
        }
    }
}
