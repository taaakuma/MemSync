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
            [1byte: 命令数]
            [1byte: 命令] [8byte: 経過時間] [1byte: 引数の個数]
            [8byte: 引数の大きさ] [不定: 引数]...
            [1byte: 命令]...]
        */
        static cmdinfo ParseCommands(byte[] bin){
            List<List<byte[]>> cmdlist = new List<List<byte[]>>();
            long index = 1;
            for(byte i = 0;i < bin[0];i++) {
                //index indecates the operate code
                List<byte[]> cmd = new List<byte[]>();
                cmd.Add([bin[index++]]);
                //
                cmd.Add([]);
                
            }
            return new cmdinfo();
        }
    }
}