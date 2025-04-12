using System;

namespace _TERMINAL_
{
    public enum OptsB : byte
    {
        add,
        all,
        debug,
        force,
        interactive,
        ip,
        lifeTime,
        list,
        message,
        mid,
        nid,
        netpoint,
        open,
        path,
        pattern,
        player,
        port,
        random,
        regex,
        remove,
        removeAt,
        replace,
        reset,
        save,
        start,
        sync,
        target,
        verbose,
        yes,
        _last_,
    }

    [Flags]
    public enum OptsF : uint
    {
        _none_ = 0,
        all = 1 << OptsB.all,
        save = 1 << OptsB.save,
        start = 1 << OptsB.start,
        yes = 1 << OptsB.yes,
        list = 1 << OptsB.list,
        interactive = 1 << OptsB.interactive,
        sync = 1 << OptsB.sync,
        open = 1 << OptsB.open,
        verbose = 1 << OptsB.verbose,
        debug = 1 << OptsB.debug,
        force = 1 << OptsB.force,
        add = 1 << OptsB.add,
        player = 1 << OptsB.player,
        remove = 1 << OptsB.remove,
        removeAt = 1 << OptsB.removeAt,
        replace = 1 << OptsB.replace,
        pattern = 1 << OptsB.pattern,
        regex = 1 << OptsB.regex,
        message = 1 << OptsB.message,
        target = 1 << OptsB.target,
        nid = 1 << OptsB.nid,
        mid = 1 << OptsB.mid,
        path = 1 << OptsB.path,
        netpoint = 1 << OptsB.netpoint,
        ip = 1 << OptsB.ip,
        port = 1 << OptsB.port,
        lifeTime = 1 << OptsB.lifeTime,
        random = 1 << OptsB.random,
        reset = 1 << OptsB.reset,
        _all_ = uint.MaxValue,
    }

    [Flags]
    enum OptsF2 : uint
    {
        a = OptsF.all,
        y = OptsF.yes,
        o = OptsF.open,
        f = OptsF.force,
        l = OptsF.list,
        i = OptsF.interactive,
        s = OptsF.sync,
        v = OptsF.verbose,
        d = OptsF.debug,
        m = OptsF.message,
        n = OptsF.netpoint,
        p = OptsF.player,
        P = OptsF.path,
    }
}