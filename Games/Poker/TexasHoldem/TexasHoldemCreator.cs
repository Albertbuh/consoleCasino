using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TexasHoldemCreator : GameCreator
{
    public TexasHoldemCreator(string name) : base(name) { }
    public override TexasHoldem Create()
    {
        return new TexasHoldem();
    }
}
