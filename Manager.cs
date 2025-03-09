using System;
using System.Text;
using System.Collections.Generic;
namespace MemSync {
    public struct meminfo {
        string name;
        int start;
        int size;
        bool memlock;
        string[] allowed;
    };
    public struct cmdinfo {
        List<byte[]> cmds;
        long elapsed;
    }
    static class Manager {
        static byte[] Memory = {};
        //static Dictionary<byte[]> commandHistory;
        static PriorityQueue<byte[],long> commandQueue;
        /*
            命令形式
            [1byte: 命令数] [8byte: 経過時間]
            [1byte: 命令]  [1byte: 引数の個数]
            [8byte: 引数の大きさ] [不定: 引数]...
            [1byte: 命令]...]
        */
        static cmdinfo ParseCommands(byte[] bin){
            List<List<byte[]>> cmdlist = new List<List<byte[]>>();
            long index = 1;
            byte[] _timelapse_bin = new byte[8];

            //経過時間を取得
            for(byte i = 0;i < 8;i++){
                _timelapse_bin[i] = bin[index++];
            }

            //命令ごとのループ   
            for(byte i = 0;i < bin[0];i++) {
                List<byte[]> cmd = new List<byte[]>();
                //命令をプッシュ
                cmd.Add([bin[index++]]);
                
                //引数ごとループ
                for(byte j = 0;j < bin[index++];j++) {
                    //引数のデータ長を取り出す
                    byte[] size_bin = new byte[8];
                    for(byte k = 0;k < 8;k++) {
                        size_bin[k] = bin[index++];
                    }
                    //シリアライズしたデータを元に戻す
                    long size = BitConverter.ToInt64(size_bin, 0);

                    //引数を抽出する
                    cmd.Add([]);
                    for(long k = 0;k < size;k++){
                        cmd[cmd.Count() - 1].Add();
                    }

                }  
                
            }
            return new cmdinfo();
        }
    }
}