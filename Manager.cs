using System;
using System.Text;
using System.Collections.Generic;
namespace MemSync {
    public struct meminfo {
        string name;
        int start;
        int size;
        bool memlock = false;
        string[] allowed;
    };
    public struct cmdinfo {
        List<byte[]> cmds;
        long elapsed;
    }
    static class Manager {
        static byte[] Memory = {};
        Dictionary<byte[]> commandHistory = {};
        PriorityQueue<byte[],long> commandQueue;
        /*
            命令形式
            [1byte: 命令数]
            [1byte: 命令] [8byte: 経過時間] [1byte: 引数の個数]
            [8byte: 引数の大きさ] [不定: 引数]...
            [1byte: 命令]...]
        */
        cmdinfo ParseCommands(byte[] cmds){
            List<List<byte[]>> cmdlist = new List<List<byte[]>>();
            long index = 1;
            for(byte i = 0;i < cmds[0];i++) {
                //index indecates the operate code

            }
        }
    }
}