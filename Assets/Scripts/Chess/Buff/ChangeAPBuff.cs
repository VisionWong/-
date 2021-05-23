using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ChangeAPBuff : ILevelChange
{
    private int _num;

    public ChangeAPBuff(IChess chess, int turns, int level) : base(chess, turns)
    {
        _num = level;
    }

    public override void OnBuffBegin()
    {
        _chess.Attribute.ChangeAP(_num);
    }

    public override void OnBuffEnd()
    {
        _chess.Attribute.ChangeAP(-_num);
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
