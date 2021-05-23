using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ChangeDefenceBuff : ILevelChange
{
    private int _level;

    public ChangeDefenceBuff(IChess chess, int turns, int level) : base(chess, turns)
    {
        _level = level;
        if (level > 0)
            BuffType = BuffType.DefUp;
        else
            BuffType = BuffType.DefDown;
    }

    public override void OnBuffBegin()
    {
        _chess.Attribute.ChangeDefenceLevel(_level);
    }

    public override void OnBuffEnd()
    {
        _chess.Attribute.ChangeDefenceLevel(-_level);
        base.OnBuffEnd();
    }

    public override void OnTurnStart()
    {
        _turns--;
        if (_turns == 0)
        {
            OnBuffEnd();
        }
    }
}
