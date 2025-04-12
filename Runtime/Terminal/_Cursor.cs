using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        int move_cursor;
        int move_frame;

        //----------------------------------------------------------------------------------------------------------

        public void RequestCursorMove(in int move, in bool absolute)
        {
            if (absolute)
                move_cursor = move;
            else
                move_cursor += move;
            move_frame = Time.frameCount;
        }

        void CheckCursorMoveRequest()
        {
            if (move_cursor != 0 && Time.frameCount != move_frame)
            {
                Util.IncrementTE(move_cursor);
                move_cursor = 0;
            }
        }
    }
}