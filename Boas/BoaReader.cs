using System;
using System.IO;
using UnityEngine;

namespace _TERMINAL_
{
    public class BoaReader : Boa
    {
        [SerializeField] string path;

        //----------------------------------------------------------------------------------------------------------

        public static void OnCmdBoa(in LineParser line)
        {
            string path = line.ReadAsPath();
            if (line.isExec)
                new BoaReader(path);
        }

        //----------------------------------------------------------------------------------------------------------

        public BoaReader(in string path) : base(0)
        {
            this.path = path;
        }

        //----------------------------------------------------------------------------------------------------------

        public override void Init()
        {
            base.Init();
            try
            {
                using FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                foreach (string line in File.ReadLines(path))
                {
                    Debug.Log(line.ToSubLog());
                    terminal.OnCmdLine(new(terminal, line, CmdF.exec));
                }
                fs.Close();
                OnSuccess();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                OnFailure();
            }
        }
    }
}