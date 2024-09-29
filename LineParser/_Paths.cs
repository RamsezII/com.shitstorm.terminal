using System.IO;
using System.Linq;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class LineParser
    {
        public string ReadAsPath()
        {
            if (TryReadAsPath(out string result))
                return result;
            else
                return string.Empty;
        }

        public bool TryReadAsPath(out string result)
        {
            bool notempty = TryRead(out result);
            if (IsCplThis && CplPath(this, result, out result))
                ReplaceSplit(result);
            return notempty;
        }

        public bool CplPath(in LineParser line, string currentPath, out string result)
        {
            //  - parse
            //  - export
            //  - completion
            //  - import

            bool
                rooted = false,
                apply = false,
                empty = string.IsNullOrWhiteSpace(currentPath),
                delims = str_delimiters.Contains(line.rawtext[line.ichar_a - 1]),
                endSlash = !empty && currentPath.EndsWith(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            DirectoryInfo dir;

            if (empty)
            {
                dir = Util.app_path.GetDir(false);
                endSlash = true;
            }
            else
            {
                rooted = Path.IsPathRooted(currentPath);
                if (rooted)
                    dir = new(currentPath);
                else
                    dir = new(Path.Combine(Util.app_path, currentPath));
            }

            result = dir.FullName;

            try
            {
                // completion
                if (line.cmdM.HasFlag(CmdM.Cpl))
                    if (line.cmdM.HasFlag(CmdM.AltW))
                    {
                        apply = true;
                        if (empty || dir.Parent == null)
                        {
                            result = string.Empty;
                            goto end;
                        }
                        else
                            dir = dir.Parent;
                    }
                    else if (line.cmdM.HasFlag(CmdM.AltE))
                    {
                        if (dir.Exists)
                        {
                            tab_last = 0;
                            foreach (var info in dir.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly))
                            {
                                dir = new(info.FullName);
                                apply = true;
                                break;
                            }
                        }
                    }
                    else if ((line.cmdM & (CmdM.Alt | CmdM.AltN | CmdM.AltS)) != 0)
                    {
                        FileSystemInfo[] files = (
                            from info
                            in (endSlash ? dir : dir.Parent).EnumerateFileSystemInfos(line.cmdM.HasFlag(CmdM.Tab) ? dir.Name + "*" : "*", SearchOption.TopDirectoryOnly)
                            orderby info.Name
                            orderby info.Attributes.HasFlag(FileAttributes.Directory) descending
                            select info
                            ).ToArray();

                        if (files.Length == 0)
                            tab_last = 0;
                        else
                        {
                            if (line.cmdM.HasFlag(CmdM.Tab) || line.cmdM.HasFlag(CmdM.AltS))
                                ++tab_last;
                            else if (line.cmdM.HasFlag(CmdM.AltN))
                                --tab_last;
                            while (tab_last < 0)
                                tab_last += files.Length;
                            tab_last %= files.Length;
                            dir = new(files[tab_last].FullName);
                        }
                    }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            if (rooted)
                result = dir.FullName;
            else
                result = Path.GetRelativePath(Util.app_path, dir.FullName);

            end:
            if (apply)
                line.cmdM |= CmdM._applyCpl;

            if (delims)
                ++line.sel_move;
            else if (result.Contains(' '))
            {
                if (!result.Contains2(LineParser.str_delimiters))
                    result = $"\"{result}\"";
            }

            return currentPath != result;
        }
    }
}