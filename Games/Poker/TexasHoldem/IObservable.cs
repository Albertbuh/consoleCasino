using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IObservable
{
    public void AddObserver(IObserver o);
    public void RemoveObserver(IObserver o);
    public void NotifyObservers(object o);
}